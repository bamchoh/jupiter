using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Windows.Input;
using Opc.Ua;

namespace Jupiter.Interfaces
{
    public interface ISubscriptionModel
    {
        Task AddToSubscription(IList objs);

        ICommand DeleteMonitoredItemsCommand { get; set; }
    }
}
