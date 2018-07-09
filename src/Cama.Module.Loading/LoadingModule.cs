﻿using Cama.Common;
using Cama.Module.Loading.Sections.Loading;
using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Regions;

namespace Cama.Module.Loading
{
    public class LoadingModule : IModule
    {
        private readonly IUnityContainer _container;
        private readonly IRegionManager _regionManager;

        public LoadingModule(IUnityContainer container, IRegionManager regionManager)
        {
            _container = container;
            _regionManager = regionManager;
        }

        public void Initialize()
        {
            _regionManager.RegisterViewWithRegion(RegionNames.LoadRegion, typeof(LoadingView));
        }
    }
}
