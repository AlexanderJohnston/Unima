﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Editing;

namespace Cama.Core.Models.Mutation
{
    public class MutatedDocument
    {
        private readonly Document _orginalDocument;
        private readonly IList<UnitTestInformation> _tests;

        public MutatedDocument(Document orginalDocument, Replacer replacer, IList<UnitTestInformation> tests)
        {
            Replacer = replacer;
            Id = Guid.NewGuid();
            FileName = orginalDocument?.Name;
            _orginalDocument = orginalDocument;
            _tests = tests;
        }

        public Guid Id { get; }

        public string FileName { get;  }

        public string Where => $"{FileName} - {Replacer.Where}";

        public Replacer Replacer { get; }

        public IList<string> Tests => _tests.Select(t => t.TestName).ToList();

        public Document CreateMutatedDocument()
        {
            var editor = DocumentEditor.CreateAsync(_orginalDocument).Result;
            editor.ReplaceNode(Replacer.Orginal, Replacer.Mutation);
            return _orginalDocument.WithText(editor.GetChangedDocument().GetSyntaxRootAsync().Result.GetText());
        }
    }
}