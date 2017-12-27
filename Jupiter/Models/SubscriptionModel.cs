using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections;
using System.Windows.Input;

using Prism.Mvvm;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Jupiter.Models
{
    public class SubscriptionModel : BindableBase, Interfaces.ISubscriptionModel
    {
        private Interfaces.ISubscriptionOperatable subscriptionOperator;
        private ObservableCollection<VariableInfoBase> monitoredItemList = new ObservableCollection<VariableInfoBase>();

        public ICommand DeleteMonitoredItemsCommand { get; set; }

        public SubscriptionModel(
            Interfaces.IConnection connector,
            Interfaces.ISubscriptionOperatable subscriptionOperator)
        {
            this.subscriptionOperator = subscriptionOperator;

            subscriptionOperator.SessionNotification -= Session_Notificaiton;
            subscriptionOperator.SessionNotification += Session_Notificaiton;

            DeleteMonitoredItemsCommand = new Commands.DelegateCommand(
                (param) => { DeleteMonitoredItems(); },
                (param) => { return connector.Connected && monitoredItemList.Count > 0; });

            connector.ObserveProperty(x => x.Connected).Subscribe(c => { if (!c) Close(); });
        }

        public IList MonitoredSelectedItems {
            get;
            set;
        }

        public IList MonitoredItemList
        {
            get { return monitoredItemList; }
            set { this.SetProperty(ref monitoredItemList, (ObservableCollection<VariableInfoBase>)value); }
        }

        public void AddToSubscription(IList objs)
        {
            var items = subscriptionOperator.AddToSubscription(objs);
            if (items == null || items.Count == 0)
                return;

            foreach (var vi in items)
            {
                MonitoredItemList.Add(vi);
            }
        }

        private void Session_Notificaiton(object sender, ClientNotificationEventArgs e)
        {
            var currentList = this.monitoredItemList;

            foreach(var change in e.Items)
            {
                for (int i = 0; i < currentList.Count(); i++)
                {
                    if (currentList[i].ClientHandle == change.ClientHandle)
                    {
                        currentList[i].SetItem(change.NodeId, change.ClientHandle, change.DataValue);
                        break;
                    }
                }
            }

            this.MonitoredItemList = currentList;
        }

        private void DeleteMonitoredItems()
        {
            var tempList = new ObservableCollection<VariableInfoBase>();

            var lastSelectedIndex = this.monitoredItemList.IndexOf(this.monitoredItemList[this.monitoredItemList.Count - 1]);

            var delItems = new List<uint>();
            foreach (var vi in this.monitoredItemList)
            {
                if (MonitoredSelectedItems.Contains(vi))
                {
                    delItems.Add(vi.ClientHandle);
                }
                else
                {
                    tempList.Add(vi);
                }
            }

            subscriptionOperator.RemoveMonitoredItem(delItems);

            Utility.SelectDeletedLastIndex(tempList, lastSelectedIndex);

            this.MonitoredItemList.Clear();
            foreach (var vi in tempList)
            {
                this.MonitoredItemList.Add(vi);
            }
        }

        private void Close()
        {
            MonitoredItemList.Clear();
        }
    }
}
