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
using Opc.Ua;

namespace Jupiter.Models
{
    public class SubscriptionModel : BindableBase, Interfaces.ISubscriptionModel
    {
        private Interfaces.ISubscriptionOperatable subscriptionOperator;
        private ObservableCollection<VariableInfo> monitoredItems = new ObservableCollection<VariableInfo>();
        private ObservableCollection<VariableInfo> selectedMonitoredItems = new ObservableCollection<VariableInfo>();

        public ICommand DeleteMonitoredItemsCommand { get; set; }

        public SubscriptionModel(
            Interfaces.IConnection connector,
            Interfaces.ISubscriptionOperatable subscriptionOperator)
        {
            this.subscriptionOperator = subscriptionOperator;

            BindingOperations.EnableCollectionSynchronization(monitoredItems, new object());

            subscriptionOperator.SessionNotification -= Session_Notificaiton;
            subscriptionOperator.SessionNotification += Session_Notificaiton;

            DeleteMonitoredItemsCommand = new Commands.DelegateCommand(
                (param) => { DeleteMonitoredItems(); },
                (param) => { return connector.Connected && monitoredItems.Count > 0; });

            connector.ObserveProperty(x => x.Connected).Subscribe(c => { if (!c) Close(); });
        }

        public IList SelectedMonitoredItems { get; set; }

        public IList MonitoredItems
        {
            get { return monitoredItems; }
            set { this.SetProperty(ref monitoredItems, (ObservableCollection<VariableInfo>)value); }
        }

        public void AddToSubscription(IList objs)
        {
            var items = subscriptionOperator.AddToSubscription(objs);
            if (items == null || items.Count == 0)
                return;

            foreach(VariableInfo mi in MonitoredItems)
            {
                mi.IsSelected = false;
            }

            foreach (var vi in items)
            {
                vi.IsSelected = true;
                MonitoredItems.Add(vi);
            }
        }

        private void Session_Notificaiton(object sender, ClientNotificationEventArgs e)
        {
            foreach(var change in e.Items)
            {
                for(int i = 0; i < monitoredItems.Count; i++)
                {
                    var vi = monitoredItems[i];
                    if (vi.ClientHandle == change.ClientHandle)
                    {
                        var builtInType = BuiltInType.Null;
                        if (change?.Value?.WrappedValue != null && change.Value.WrappedValue.TypeInfo != null)
                        {
                            builtInType = change.Value.WrappedValue.TypeInfo.BuiltInType;
                        }

                        if (builtInType == vi.Type)
                        {
                            vi.UpdateDataValue(change?.Value);
                        }
                        else
                        {
                            vi.NewDataValue(change?.Value);
                        }
                        break;
                    }
                }
            }
        }

        private void DeleteMonitoredItems()
        {
            var tempList = new ObservableCollection<VariableInfo>();

            var delItems = new List<uint>();
            foreach (var vi in (ObservableCollection<VariableInfo>)MonitoredItems)
            {
                if (SelectedMonitoredItems.Contains(vi))
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
            MonitoredItems.Clear();
        }
    }
}
