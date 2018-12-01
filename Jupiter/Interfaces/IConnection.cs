using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Opc.Ua;

namespace Jupiter.Interfaces
{
    public interface IConnection : INotifyPropertyChanged
    {
        bool Connected { get; set; }

        Task CreateSession(string endpointURI);

        void Close();

        event ClientNotificationEventHandler SessionNotification;
    }
}
