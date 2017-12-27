using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Windows.Input;

namespace Jupiter.Interfaces
{
    public interface ISubscriptionModel
    {
        void AddToSubscription(IList objs);

        ICommand DeleteMonitoredItemsCommand { get; set; }
    }
}
