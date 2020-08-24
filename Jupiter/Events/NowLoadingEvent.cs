using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Events;
using System.Threading;
using System.Security;
using System.Windows.Input;

namespace Jupiter.Events
{
    public class NowLoadingEvent : PubSubEvent<NowLoading>
    {
    }

    public class NowLoading
    {
        public List<ServerAndEndpointsPair> ServerList;

        public SemaphoreSlim Semaphore;

        public string UserName;

        public SecureString Password;

        public int SelectedIndex;

        public int SelectedServerIndex;

        public Client Client;

        public bool Result;
    }
}
