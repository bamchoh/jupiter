using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using Prism.Mvvm;
using Prism.Events;

using Unity.Attributes;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;


namespace Jupiter.Models
{
    public class ConnectionModel : BindableBase
    {
        [Dependency]
        public IEventAggregator EventAggregator { get; set; }

        private Interfaces.IConnection client;
        private string connectButtonContent = "Connect";

        public ConnectionModel(Interfaces.IConnection client)
        {
            this.Endpoint = "opc.tcp://127.0.0.1:62548";

            this.client = client;

            this.client.ObserveProperty(x => x.Connected).Subscribe(c => {
                if(c)
                {
                    ConnectButtonContent = "Disconnect";
                }
                else
                {
                    ConnectButtonContent = "Connect";
                }
            });

            this.ConnectCommand = new Commands.DelegateCommand(
                (param) => { CreateSession(param); },
                (param) => true);
        }

        public string Endpoint { get; set; }

        public string ConnectButtonContent
        {
            get { return connectButtonContent; }
            set { this.SetProperty(ref connectButtonContent, value); }
        }

        public ICommand ConnectCommand { get; private set; }

        private void CreateSession(object param)
        {
            if (!client.Connected)
            {
                try
                {
                    var config = ApplicationConfiguration.Load(null);
                    client.CreateSession(Endpoint, config).Wait();
                }
                catch (Exception ex)
                {
                    this.EventAggregator
                        .GetEvent<Events.ErrorNotificationEvent>()
                        .Publish(new Events.ErrorNotification(ex));
                }
            }
            else
            {
                client.Close();
            }
        }
    }
}
