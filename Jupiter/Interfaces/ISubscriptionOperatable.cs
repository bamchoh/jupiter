using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using Opc.Ua;

namespace Jupiter.Interfaces
{
    public interface ISubscriptionOperatable
    {
        event ClientNotificationEventHandler SessionNotification;

        Task<IList<VariableInfoBase>> AddToSubscription(IList objs);

        void RemoveMonitoredItem(List<uint> chList);
    }
}
