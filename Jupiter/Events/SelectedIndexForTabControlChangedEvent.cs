using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Events;
using System.Threading;

namespace Jupiter.Events
{
    public class SelectedIndexForTabControlChangedEvent : PubSubEvent<SelectedIndexForTabControlChangedArgs>
    {
    }

    public class SelectedIndexForTabControlChangedArgs
    {
        public int SelectedIndex;
    }
}
