using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jupiter;
using Jupiter.Interfaces;
using Opc.Ua;
using Prism.Mvvm;

namespace UnitTestJupiter
{
    public class TestConnection : BindableBase, IConnection
    {
        public bool Exception { get; set; }

        private bool connected;
        public bool Connected
        {
            get { return connected; }
            set { SetProperty(ref connected, value); }
        }

        public event ClientNotificationEventHandler SessionNotification;

        void IConnection.Close()
        {
            Connected = false;
        }

        Task IConnection.CreateSession(string endpointURI)
        {
            if (Exception)
                throw new Exception("CreateSessionException");

            Connected = true;
            return Task.Delay(100);
        }
    }
}
