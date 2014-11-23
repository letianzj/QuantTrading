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

using System.Windows;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.UnityExtensions;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Prism.PubSubEvents;
using Microsoft.Practices.Prism.Logging;
using Modules.Framework.Events;
using Modules.Framework.Interfaces;
using Modules.Framework.Services;
using log4net;
using TradingBase;

namespace QTShell
{
    public class Bootstrapper : UnityBootstrapper, IDisposable
    {
        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<Shell>();
        }

        protected override void InitializeShell()
        {
            base.InitializeShell();

            App.Current.MainWindow = (Window)this.Shell;
            App.Current.MainWindow.Show();
        }

        protected override void ConfigureModuleCatalog()
        {
            base.ConfigureModuleCatalog();

            ModuleCatalog moduleCatelog = (ModuleCatalog)this.ModuleCatalog;
            moduleCatelog.AddModule(typeof(Modules.OrderAndPositions.ModuleOrderAndPositions));
            moduleCatelog.AddModule(typeof(Modules.RealTimeQuotePresentation.ModuleRealTimeQuotePresentation));
            moduleCatelog.AddModule(typeof(Modules.StrategyManager.ModuleStrategyManager));
            moduleCatelog.AddModule(typeof(Modules.OrderTicket.ModuleOrderTicket));
            moduleCatelog.AddModule(typeof(Modules.Framework.ModuleFramework));
        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

            //this.RegisterTypeIfMissing(typeof(IModuleTracker), typeof(ModuleTracker), true);
            string _rootpath = Util.GetRootPath();
            ConfigManager _configmanagerservice = ConfigManager.Deserialize(_rootpath + @"\Config\mainsettings.xml");
            this.Container.RegisterInstance<IConfigManager>(_configmanagerservice);        // register

            // _container.RegisterType<IGlobalIdService, GlobalIdService>(new ContainerControlledLifetimeManager());
            GlobalIdService _globalidservice = new GlobalIdService();
            this.Container.RegisterInstance<IGlobalIdService>(_globalidservice);

            // Ensure we properly dispose of objects in the container at application exit
            Application.Current.Exit += (sender, e) => this.Dispose();
        }

        protected override Microsoft.Practices.Prism.Logging.ILoggerFacade CreateLogger()
        {
            //return base.CreateLogger();
            log4net.Config.XmlConfigurator.Configure();
            return new Log4NetLoggerAdapter();
        }

        public void Dispose()
        {
            var eventAggregator = Container.Resolve<IEventAggregator>();
            if (eventAggregator != null)
            {
                eventAggregator.GetEvent<ApplicationExitEvent>().Publish("Exit");
            }  
        }
    }
}
