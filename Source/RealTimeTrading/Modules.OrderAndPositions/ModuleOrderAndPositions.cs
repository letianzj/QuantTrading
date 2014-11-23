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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Prism.Logging;
using log4net;
using TradingBase;
using Modules.Framework.Interfaces;


namespace Modules.OrderAndPositions
{
    [Module(ModuleName = "ModuleOrderAndPositions")]
    public class ModuleOrderAndPositions : IModule
    {
        private readonly IUnityContainer _container;
        private readonly IRegionManager _regionManager;
        private readonly ILoggerFacade _logger;

        public ModuleOrderAndPositions(IUnityContainer container, IRegionManager regionManager, ILoggerFacade logger)
        {
            this._container = container;
            this._regionManager = regionManager;
            this._logger = logger;
        }

        public void Initialize()
        {
            // register service

            // register region view
            this._regionManager.RegisterViewWithRegion("InfoRegion", () => this._container.Resolve<OrderAndPositionView>());

            _logger.Log("Module ModuleOrderAndPositions Loaded", Category.Info, Priority.None);
        }
    }
}
