using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Events;
using System.Threading;

namespace Jupiter.Events
{
    public class NowLoadingEvent : PubSubEvent<NowLoading>
    {
    }

    public class NowLoading
    {
        public SemaphoreSlim Semaphore;

        public List<string> SecurityList;

        public string Endpoint;

        public int SelectedIndex;
    }
}
