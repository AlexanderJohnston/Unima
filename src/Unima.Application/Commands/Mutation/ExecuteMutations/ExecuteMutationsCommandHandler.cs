﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Anotar.Log4Net;
using MediatR;
using Unima.Core;
using Unima.Core.Config;
using Unima.Core.Execution;
using Unima.Core.Loggers;

namespace Unima.Application.Commands.Mutation.ExecuteMutations
{
    public class ExecuteMutationsCommandHandler : IRequestHandler<ExecuteMutationsCommand, IList<MutationDocumentResult>>
    {
        private readonly MutationDocumentExecutor _mutationDocumentExecutor;
        private readonly MutationRunLoggerFactory _mutationRunLoggerFactory;

        public ExecuteMutationsCommandHandler(MutationDocumentExecutor mutationDocumentExecutor, MutationRunLoggerFactory mutationRunLoggerFactory)
        {
            _mutationDocumentExecutor = mutationDocumentExecutor;
            _mutationRunLoggerFactory = mutationRunLoggerFactory;
        }

        public async Task<IList<MutationDocumentResult>> Handle(ExecuteMutationsCommand command, CancellationToken cancellationToken)
        {
            var semaphoreSlim = new SemaphoreSlim(command.Config.NumberOfTestRunInstances, command.Config.NumberOfTestRunInstances);
            var results = new List<MutationDocumentResult>();
            var mutationDocuments = new Queue<MutationDocument>(command.MutationDocuments);
            var currentRunningDocuments = new List<Task>();
            var numberOfMutationsLeft = command.MutationDocuments.Count;
            var mutationRunLoggers = command.Config.MutationRunLoggers?.Select(m => _mutationRunLoggerFactory.GetMutationRunLogger(m)).ToList() ?? new List<IMutationRunLogger>();

            LogTo.Info($"Total number of mutations generated: {numberOfMutationsLeft}");
            mutationRunLoggers.ForEach(m => m.LogBeforeRun(command.MutationDocuments));

            await Task.Run(() =>
            {
                while (mutationDocuments.Any())
                {
                    semaphoreSlim.Wait();
                    var document = mutationDocuments.Dequeue();

                    currentRunningDocuments.Add(Task.Run(async () =>
                    {
                        MutationDocumentResult result = null;

                        try
                        {
                            mutationRunLoggers.ForEach(m => m.LogBeforeMutation(document));
                            command.MutationDocumentStartedCallback?.Invoke(document);

                            var resultTask = _mutationDocumentExecutor.ExecuteMutationAsync(command.Config, document);
                            result = await resultTask;
                        }
                        catch (Exception ex)
                        {
                            LogTo.WarnException($"Unexpected exception when running {document.MutationName}", ex);

                            result = new MutationDocumentResult
                            {
                                Id = document.Id,
                                UnexpectedError = ex.Message
                            };
                        }
                        finally
                        {
                            lock (results)
                            {
                                results.Add(result);

                                var survived = results.Count(r => r.Survived && (r.CompilationResult != null && r.CompilationResult.IsSuccess));
                                var killed = results.Count(r => !r.Survived && (r.CompilationResult != null && r.CompilationResult.IsSuccess));
                                var compileErrors = results.Count(r => r.CompilationResult != null && !r.CompilationResult.IsSuccess);
                                var unknownErrors = results.Count(r => r.UnexpectedError != null);

                                Interlocked.Decrement(ref numberOfMutationsLeft);
                                LogTo.Info($"Current progress: {{ Survived: {survived}, Killed: {killed}, CompileErrors: {compileErrors}, UnknownErrors: {unknownErrors}, MutationsLeft: {numberOfMutationsLeft} }}");
                                mutationRunLoggers.ForEach(m => m.LogAfterMutation(document, results, numberOfMutationsLeft));
                            }

                            semaphoreSlim.Release();
                            command.MutationDocumentCompledtedCallback?.Invoke(result);
                        }
                    }));

                }
            });

            // Wait for the final ones
            await Task.WhenAll(currentRunningDocuments);

            return results;
        }

        private TimeSpan GetTimeout(UnimaConfig config)
        {
            if (config.BaselineInfos != null && config.BaselineInfos.Any())
            {
                return new TimeSpan(config.BaselineInfos.Sum(b => b.ExecutionTime.Ticks * 4));
            }

            return TimeSpan.FromMinutes(config.MaxTestTimeMin);
        }
    }
}
