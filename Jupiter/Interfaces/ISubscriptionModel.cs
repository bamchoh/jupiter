using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Windows.Input;
using Opc.Ua;
using System.Collections.ObjectModel;

namespace Jupiter.Interfaces
{
    public interface ISubscriptionModel
    {
        void AddToSubscription(IList objs);

        IList MonitoredItems { get; set; }

        ICommand DeleteMonitoredItemsCommand { get; set; }
    }
}
