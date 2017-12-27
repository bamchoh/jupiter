using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Jupiter.Interfaces
{
    public interface IConnection : INotifyPropertyChanged
    {
        string Endpoint { get; set; }

        bool Connected { get; set; }

        Task CreateSession();

        void Close();

        event ClientNotificationEventHandler SessionNotification;
    }
}
