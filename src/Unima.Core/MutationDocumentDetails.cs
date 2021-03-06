﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Unima.Core.Location;

namespace Unima.Core
{
    public class MutationDocumentDetails
    {
        public MutationDocumentDetails(SyntaxNode orginal, SyntaxNode mutation, MutationLocationInfo location)
        {
            Orginal = orginal;
            Mutation = mutation;
            FullOrginal = GetRoot(orginal);
            FullMutation = FullOrginal.ReplaceNode(Orginal, Mutation);
            Location = location;
        }

        public SyntaxNode Orginal { get; }

        public SyntaxNode Mutation { get; }

        public CompilationUnitSyntax FullOrginal { get; }

        public CompilationUnitSyntax FullMutation { get; }

        public MutationLocationInfo Location { get; }

        private CompilationUnitSyntax GetRoot(SyntaxNode syntaxNode)
        {
            return syntaxNode.FirstAncestorOrSelf<CompilationUnitSyntax>();
        }
    }
}
