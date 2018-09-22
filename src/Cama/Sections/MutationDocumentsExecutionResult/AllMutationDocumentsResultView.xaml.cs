﻿using System.Collections.Generic;
using System.Windows.Controls;
using Cama.Core;

namespace Cama.Sections.MutationDocumentsExecutionResult
{
    /// <summary>
    /// Interaction logic for AllCompletedMutationDocumentTestResult.xaml
    /// </summary>
    public partial class AllMutationDocumentsResultView : TabItem
    {
        public AllMutationDocumentsResultView(IEnumerable<MutationDocumentResult> completedMutations)
        {
            InitializeComponent();

            var viewModel = DataContext as AllMutationDocumentsResultViewModel;
            viewModel.Initialize(completedMutations);
        }
    }
}
