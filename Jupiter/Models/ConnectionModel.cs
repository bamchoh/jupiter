﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Prism.Mvvm;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;


namespace Jupiter.Models
{
    public class ConnectionModel : BindableBase
    {
        private Interfaces.IConnection client;
        private string connectButtonContent = "Connect";

        public ConnectionModel(Interfaces.IConnection client)
        {
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
        }
        public string Endpoint
        {
            get { return client.Endpoint; }
            set { client.Endpoint = value; }
        }

        public string ConnectButtonContent
        {
            get { return connectButtonContent; }
            set { this.SetProperty(ref connectButtonContent, value); }
        }

        public void CreateSession(object param)
        {
            if (!client.Connected)
            {
                try
                {
                    client.CreateSession().Wait();
                }
                catch (Exception ex)
                {
                    var msgbox = Commands.ShowMessageCommand.Command;
                    if (ex.InnerException != null)
                    {
                        msgbox.Execute(ex.InnerException.Message);
                    }
                    else
                    {
                        msgbox.Execute(ex.Message);
                    }
                }
            }
            else
            {
                client.Close();
            }
        }
    }
}
