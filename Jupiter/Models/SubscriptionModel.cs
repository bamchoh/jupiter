﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections;
using System.Windows.Input;
using System.Windows.Data;

using Prism.Mvvm;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Jupiter.Models
{
    public class SubscriptionModel : BindableBase, Interfaces.ISubscriptionModel
    {
        private Interfaces.ISubscriptionOperatable subscriptionOperator;
        private Interfaces.IVariableInfoManager variableInfoManager;
        private ObservableCollection<VariableInfoBase> monitoredItemList = new ObservableCollection<VariableInfoBase>();

        public ICommand DeleteMonitoredItemsCommand { get; set; }

        public SubscriptionModel(
            Interfaces.IConnection connector,
            Interfaces.ISubscriptionOperatable subscriptionOperator,
            Interfaces.IVariableInfoManager variableInfoManager)
        {
            this.subscriptionOperator = subscriptionOperator;
            this.variableInfoManager = variableInfoManager;

            BindingOperations.EnableCollectionSynchronization(monitoredItemList, new object());

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

            foreach(VariableInfoBase mi in MonitoredItemList)
            {
                mi.IsSelected = false;
            }

            foreach (var vi in items)
            {
                vi.IsSelected = true;
                MonitoredItemList.Add(vi);
            }
        }

        private void Session_Notificaiton(object sender, ClientNotificationEventArgs e)
        {
            foreach(var change in e.Items)
            {
                for(int i = 0; i < this.monitoredItemList.Count; i++)
                {
                    var vi = this.monitoredItemList[i];
                    if (vi.ClientHandle == change.ClientHandle)
                    {
                        vi.Update(change);
                    }
                }
            }
        }

        private void DeleteMonitoredItems()
        {
            var tempList = new ObservableCollection<VariableInfoBase>();

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
        }

        private void Close()
        {
            MonitoredItemList.Clear();
        }
    }
}
