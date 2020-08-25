using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading;
using System.Security;

using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System.Windows.Input;
using System.Windows.Documents;

namespace Jupiter.ViewModels
{
    public class LoadingViewModel : BindableBase
    {
        private Events.NowLoading model;

        public IList SecurityList {
            get
            {
                var s = ServerAndEndpointsPair.SecurityList(model.ServerList, SelectedServerIndex);
                if (s.Count != 0)
                    SelectedIndex = 0;
                return s;
            }
        }

        public IList Endpoints {
            get
            {
                return model.ServerList;
            }
        }

        public string UserName
        {
            get
            {
                return model.UserName;
            }

            set
            {
                model.UserName = value;
            }
        }

        public SecureString Password
        {
            get
            {
                return model.Password;
            }

            set
            {
                model.Password = value;
            }
        }

        public int SelectedIndex {
            get
            {
                return model.SelectedIndex;
            }

            set
            {
                model.SelectedIndex = value;
                this.RaisePropertyChanged("SelectedIndex");
            }
        }

        public int SelectedServerIndex {
            get
            {
                return model.SelectedServerIndex;
            }

            set
            {
                model.SelectedServerIndex = value;
                this.RaisePropertyChanged("SecurityList");
                this.RaisePropertyChanged("SecurityListIsNotZero");
                BrowseMessage = "";
                this.RaisePropertyChanged("BrowseMessage");
                this.RaisePropertyChanged("ExistBrowseMessage");
            }
        }

        public bool SecurityListIsNotZero
        {
            get
            {
                return SecurityList.Count != 0;
            }
        }

        public ICommand Browse { get; }

        public string BrowseMessage { get; set; }

        public object ExistBrowseMessage
        {
            get
            {
                if (string.IsNullOrEmpty(BrowseMessage))
                    return System.Windows.Visibility.Collapsed;
                else
                    return System.Windows.Visibility.Visible;
            }
        }

        public LoadingViewModel(Events.NowLoading model)
        {
            this.model = model;
            SelectedIndex = 0;
            SelectedServerIndex = 0;
            BrowseMessage = "";
            Browse = new Commands.DelegateCommand((param) =>
            {
                BrowseMessage = "";
                this.RaisePropertyChanged("BrowseMessage");
                this.RaisePropertyChanged("ExistBrowseMessage");

                try
                {
                    model.Client.BrowseSecurityList(model.ServerList[SelectedServerIndex]);
                }
                catch (Exception e)
                {
                    model.ServerList[SelectedServerIndex].Endpoints = null;
                    BrowseMessage = string.Format("{0}", e.Message);
                }
                finally
                {
                    this.RaisePropertyChanged("SecurityList");
                    this.RaisePropertyChanged("SecurityListIsNotZero");
                    this.RaisePropertyChanged("BrowseMessage");
                    this.RaisePropertyChanged("ExistBrowseMessage");
                }
            }, (param) => true);
        }
    }
}
