using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
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
        /// <summary>
        /// 例外発生時のエラーコード
        /// </summary>
        private const int ERROR_EXIT_CODE = 1;

        private IUnityContainer Container { get; } = new UnityContainer();

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            this.RegisterErrorHandler();

            Jupiter.Properties.Settings.Default.Upgrade();

            ViewModelLocationProvider.SetDefaultViewModelFactory(x => this.Container.Resolve(x));

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

            var c = new Client(ea);
            this.Container.RegisterInstance<Interfaces.IConnection>(c);
            this.Container.RegisterInstance<Interfaces.IReferenceFetchable>(c);
            this.Container.RegisterInstance<Interfaces.INodeInfoGetter>(c);
            this.Container.RegisterInstance<Interfaces.ISubscriptionOperatable>(c);
            this.Container.RegisterInstance<Interfaces.IOneTimeAccessOperator>(c);

            var references = new OPCUAReference(c, null, ea);
            this.Container.RegisterInstance<Interfaces.IReference>(references);

            this.Container.Resolve<Views.MainWindow>().Show();
        }

        /// <summary>
        /// 例外発生時に補足するための処理を登録する。
        /// </summary>
        private void RegisterErrorHandler()
        {
            this.DispatcherUnhandledException +=
                new DispatcherUnhandledExceptionEventHandler(App_DispatcherUnhandledException);
            AppDomain.CurrentDomain.UnhandledException +=
                new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
        }

        /// <summary>
        /// UIスレッドにて例外が発生した時の処理。
        /// </summary>
        /// <param name="sender">発生元</param>
        /// <param name="e">例外情報</param>
        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            var result = this.ShowErrorInfo(e.Exception);
            if (result == MessageBoxResult.OK)
            {
                this.Shutdown(ERROR_EXIT_CODE);
            }
        }

        /// <summary>
        /// UIスレッド以外で例外が発生した時の処理。
        /// </summary>
        /// <param name="sender">発生元</param>
        /// <param name="e">例外情報</param>
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var result = this.ShowErrorInfo(e.ExceptionObject as Exception);
            if (result == MessageBoxResult.OK)
            {
                Environment.Exit(ERROR_EXIT_CODE);
            }
        }


        /// <summary>
        /// 指定の例外情報をメッセージボックスとして表示します。
        /// </summary>
        /// <param name="ex">例外</param>
        /// <returns>メッセージボックス結果</returns>
        private MessageBoxResult ShowErrorInfo(Exception ex)
        {
            return MessageBox.Show(
                String.Format("{0}\n{1}", ex.Message, ex.StackTrace),
                "エラーが発生しました。",
                MessageBoxButton.OK,
                MessageBoxImage.Stop);
        }
    }
}
