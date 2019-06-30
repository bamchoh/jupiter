using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Practices.Unity;
using Prism.Mvvm;
using Unity;
using Unity.Lifetime;

namespace Jupiter
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        private IUnityContainer Container { get; } = new UnityContainer();

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Jupiter.Properties.Settings.Default.Upgrade();

            ViewModelLocationProvider.SetDefaultViewModelFactory(x => this.Container.Resolve(x));

            var varinfo = new VariableInfo();
            this.Container.RegisterInstance<Interfaces.IVariableInfoManager>(varinfo);

            var lifetimeManager = new ContainerControlledLifetimeManager();
            var lifetimeManager2 = new ContainerControlledLifetimeManager();
            var lifetimeManager3 = new ContainerControlledLifetimeManager();
            var lifetimeManager4 = new ContainerControlledLifetimeManager();

            var ea = new Prism.Events.EventAggregator();

            this.Container.RegisterInstance<Prism.Events.IEventAggregator>(ea);
            this.Container.RegisterType<Interfaces.INodeTreeModel, Models.NodeTreeModel>(lifetimeManager);
            this.Container.RegisterType<Interfaces.INodeInfoDataGrid, Models.NodeInfoDataGridModel>(lifetimeManager2);
            this.Container.RegisterType<Interfaces.ISubscriptionModel, Models.SubscriptionModel>(lifetimeManager3);
            this.Container.RegisterType<Interfaces.IOneTimeAccessModel, Models.OneTimeAccessModel>(lifetimeManager4);

            var c = new Client(varinfo);
            this.Container.RegisterInstance<Interfaces.IConnection>(c);
            this.Container.RegisterInstance<Interfaces.IReferenceFetchable>(c);
            this.Container.RegisterInstance<Interfaces.INodeInfoGetter>(c);
            this.Container.RegisterInstance<Interfaces.ISubscriptionOperatable>(c);
            this.Container.RegisterInstance<Interfaces.IOneTimeAccessOperator>(c);

            var references = new OPCUAReference(c, null, ea);
            this.Container.RegisterInstance<Interfaces.IReference>(references);

            this.Container.Resolve<Views.MainWindow>().Show();
        }
    }
}
