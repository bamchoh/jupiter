using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.ComponentModel;
using Jupiter.Interfaces;

using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Jupiter.ViewModels
{
    public class ConnectionViewModel : BindableBase
    {
        private Models.ConnectionModel model;

        public ConnectionViewModel(Models.ConnectionModel model)
        {
            this.model = model;

            Endpoint = model.ToReactivePropertyAsSynchronized(x => x.Endpoint);

            ConnectButtonContent = model.ToReactivePropertyAsSynchronized(x => x.ConnectButtonContent);

            this.ConnectCommand = new Commands.DelegateCommand(
                (param) => { model.CreateSession(param); },
                (param) => true);
        }

        public ICommand ConnectCommand { get; private set; }

        public ReactiveProperty<string> Endpoint { get; set; }

        public ReactiveProperty<string> ConnectButtonContent { get; set; }

    }
}
