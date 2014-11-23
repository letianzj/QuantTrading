/****************************** Project Header ******************************\
Project:	      QuantTrading
Author:			  Letian_zj @ Codeplex
URL:			  https://quanttrading.codeplex.com/
Copyright 2014 Letian_zj

This file is part of QuantTrading Project.

QuantTrading is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, 
either version 3 of the License, or (at your option) any later version.

QuantTrading is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. 
See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with QuantTrading. 
If not, see http://www.gnu.org/licenses/.

\***************************************************************************/
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Prism.Logging;
using log4net;

using Modules.Framework.Interfaces;
using Modules.RealTimeQuotePresentation.Services;
using Modules.RealTimeQuotePresentation.Views;
using Modules.RealTimeQuotePresentation.ViewModels;

namespace Modules.RealTimeQuotePresentation
{
    [Module(ModuleName = "ModuleRealTimeQuotePresentation")]
    public class ModuleRealTimeQuotePresentation : IModule
    {
        private readonly IUnityContainer _container;
        private readonly IRegionManager _regionManager;
        private readonly ILoggerFacade _logger;

        public ModuleRealTimeQuotePresentation(IUnityContainer container, IRegionManager regionManager, ILoggerFacade logger)
        {
            this._container = container;
            this._regionManager = regionManager;
            this._logger = logger;
        }

        public void Initialize()
        {
            // register service
            this._container.RegisterType<IQuoteUpdateService, QuoteUpdateService>(new ContainerControlledLifetimeManager());
              
            // register region view
            this._regionManager.RegisterViewWithRegion("QuoteRegion", () => this._container.Resolve<QuoteGridView>());
            this._regionManager.RegisterViewWithRegion("PlotRegion", () => this._container.Resolve<QuotePlotView>());

            _logger.Log("Module RealTimeQuoteGrid Loaded", Category.Info, Priority.None);
        }
    }
}
