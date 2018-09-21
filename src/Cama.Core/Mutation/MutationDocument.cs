﻿using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Editing;

namespace Cama.Core.Mutation.Models
{
    public class MutationDocument
    {
        private readonly Document _orginalDocument;

        public MutationDocument(Document orginalDocument, MutationDocumentDetails mutationDetails)
        {
            MutationDetails = mutationDetails;
            Id = Guid.NewGuid();
            FileName = orginalDocument?.Name;
            ProjectName = orginalDocument?.Project.Name;
            _orginalDocument = orginalDocument;
        }

        public Guid Id { get; }

        public string FileName { get;  }

        public string ProjectName { get; set; }

        public MutationDocumentDetails MutationDetails { get; }

        public string MutationName => $"Proj: {ProjectName}, File: {FileName}({MutationDetails.Location.Where} - {MutationDetails.Location.Line})";

        public Document CreateMutatedDocument()
        {
            var editor = DocumentEditor.CreateAsync(_orginalDocument).Result;
            editor.ReplaceNode(MutationDetails.Orginal, MutationDetails.Mutation);
            return _orginalDocument.WithText(editor.GetChangedDocument().GetSyntaxRootAsync().Result.GetText());
        }
    }
}