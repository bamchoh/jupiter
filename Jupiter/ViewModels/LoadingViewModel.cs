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

namespace Jupiter.ViewModels
{
    public class LoadingViewModel : BindableBase
    {
        private Events.NowLoading model;

        public IList SecurityList {
            get
            {
                return ServerAndEndpointsPair.SecurityList(model.ServerList, SelectedServerIndex);
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
            }
        }

        public LoadingViewModel(Events.NowLoading model)
        {
            this.model = model;
            SelectedIndex = 0;
            SelectedServerIndex = 0;
        }
    }
}
