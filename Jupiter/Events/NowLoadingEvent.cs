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

        public SortedDictionary<string, List<string>> SecurityList;

        public List<string> Endpoints;

        public string UserName;

        public string Password;

        public int SelectedIndex;
    }
}
