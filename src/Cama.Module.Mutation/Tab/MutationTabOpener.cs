﻿using System.Collections.Generic;
using Cama.Common.Tabs;
using Cama.Core.Models.Mutation;
using Cama.Module.Mutation.Sections.Details;
using Cama.Module.Mutation.Sections.Overview;
using Cama.Module.Mutation.Sections.TestRun;

namespace Cama.Module.Mutation.Tab
{
    public class MutationTabOpener : IMutationModuleTabOpener
    {
        private readonly IMainTabContainer _mainTabContainer;

        public MutationTabOpener(IMainTabContainer mainTabContainer)
        {
            _mainTabContainer = mainTabContainer;
        }

        public void OpenOverviewTab()
        {
            _mainTabContainer.AddTab(new MutationOverviewView());
        }

        public void OpenDocumentDetailsTab(MutatedDocument document)
        {
            _mainTabContainer.AddTab(new MutationDetailsView(document));
        }

        public void OpenTestRunTab(IList<MutatedDocument> documents)
        {
            _mainTabContainer.AddTab(new TestRunView(documents));
        }
    }
}
