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
            ViewModelLocationProvider.SetDefaultViewModelFactory(x => this.Container.Resolve(x));

            var lifetimeManager = new ContainerControlledLifetimeManager();
            var lifetimeManager2 = new ContainerControlledLifetimeManager();
            var lifetimeManager3 = new ContainerControlledLifetimeManager();
            var lifetimeManager4 = new ContainerControlledLifetimeManager();

            this.Container.RegisterInstance<Prism.Events.IEventAggregator>(new Prism.Events.EventAggregator());
            this.Container.RegisterType<Interfaces.INodeTreeModel, Models.NodeTreeModel>(lifetimeManager);
            this.Container.RegisterType<Interfaces.INodeInfoDataGrid, Models.NodeInfoDataGridModel>(lifetimeManager2);
            this.Container.RegisterType<Interfaces.ISubscriptionModel, Models.SubscriptionModel>(lifetimeManager3);
            this.Container.RegisterType<Interfaces.IOneTimeAccessModel, Models.OneTimeAccessModel>(lifetimeManager4);

            this.Container.RegisterInstance<Interfaces.IConnection>(Client.Instance);
            this.Container.RegisterInstance<Interfaces.IReferenceFetchable>(Client.Instance);
            this.Container.RegisterInstance<Interfaces.INodeInfoGetter>(Client.Instance);
            this.Container.RegisterInstance<Interfaces.ISubscriptionOperatable>(Client.Instance);
            this.Container.RegisterInstance<Interfaces.IOneTimeAccessOperator>(Client.Instance);
            this.Container.RegisterInstance<Interfaces.IVariableInfoManager>(Client.Instance);

            var references = new OPCUAReference(Client.Instance, null);
            this.Container.RegisterInstance<Interfaces.IReference>(references);

            this.Container.Resolve<Views.MainWindow>().Show();
        }
    }
}
