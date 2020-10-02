﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Data;
using System.Collections;
using System.Collections.ObjectModel;

using Prism.Mvvm;
using Prism.Commands;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using Opc.Ua;
using Prism.Events;
using Unity;

namespace Jupiter.Models
{
    public class NodeTreeModel : BindableBase, Interfaces.INodeTreeModel
    {
        [Dependency]
        public IEventAggregator EventAggregator { get; set; }

        private Interfaces.IConnection connector;
        private Interfaces.IReference references;
        private bool isEnabled;
        private ObservableCollection<VariableInfoBase2> variableNodes;

        public NodeTreeModel(
            Interfaces.IConnection connector, 
            Interfaces.IReference references,
            Interfaces.ISubscriptionModel subscriptionM,
            Interfaces.IOneTimeAccessModel oneTimeAccessM)
        {
            this.connector = connector;
            this.references = references;

            ReloadCommand = new Commands.DelegateCommand(
                (param) => { ForceUpdate(); },
                (param) => connector.Connected);

            MouseDoubleClickedCommand = new DelegateCommand<IList>((items) =>
            {
                subscriptionM.AddToSubscription(items);
                ChangeSelectedIndexForTabContorol(0);
            }, (param) => true);

            AddToReadWriteCommand = new Commands.DelegateCommand(
                (param) => {
                    oneTimeAccessM.AddToReadWrite((IList)param);
                    ChangeSelectedIndexForTabContorol(1);
                },
                (param) => connector.Connected);

            NodeSelectedCommand = new Commands.DelegateCommand(
                (param) => { }, //nodeInfoDataGrid.Update((VariableNode)param); },
                (param) => true);

            UpdateVariableNodeListCommand = new Commands.DelegateCommand(
                (param) => { UpdateVariableNodes((Interfaces.IReference)param); },
                (param) => true);

            this.connector.ObserveProperty(x => x.Connected).Subscribe(c => Update(c));

            Initialize();
        }

        public void ChangeSelectedIndexForTabContorol(int i)
        {
            this.EventAggregator
                .GetEvent<Events.SelectedIndexForTabControlChangedEvent>()
                .Publish(new Events.SelectedIndexForTabControlChangedArgs() { SelectedIndex = i });
        }

        public Interfaces.IReference References
        {
            get { return references; }
            set { this.SetProperty(ref references, value); }
        }

        public IList VariableNodes
        {
            get { return variableNodes; }
            set { this.SetProperty(ref variableNodes, (ObservableCollection<VariableInfoBase2>)value); }
        }

        public bool IsEnabled
        {
            get { return connector.Connected; }
            set { this.SetProperty(ref isEnabled, value); }
        }

        public ICommand ReloadCommand { get; set; }
        public ICommand MouseDoubleClickedCommand { get; set; }
        public ICommand AddToReadWriteCommand { get; set; }
        public ICommand NodeSelectedCommand { get; set; }
        public ICommand UpdateVariableNodeListCommand { get; set; }

        public void Dispose()
        {
            // Close();
        }

        private void Update(bool c)
        {
            IsEnabled = c;

            if (c)
            {
                if (References != null && References.Children.Count > 0)
                    return;

                ForceUpdate();
            }
        }

        private void ForceUpdate()
        {
            Initialize();

            References.UpdateReferences();
        }

        private void Initialize()
        {
            References?.Children.Clear();

            VariableNodes = new ObservableCollection<VariableInfoBase2>();
        }

        private void SelectionChanged(Interfaces.IReference reference, Interfaces.INodeInfoDataGrid nodeInfoDataGrid)
        {
            nodeInfoDataGrid.Update(reference);
        }

        private void UpdateVariableNodes(Interfaces.IReference obj)
        {
            try
            {
                var client = connector as Client;

                var refs = client.FetchVariableReferences(obj.NodeId);

                if(refs != null && refs.Count != 0)
                {
                    VariableNodes = refs;
                }
            }
            catch (Exception ex)
            {
                this.EventAggregator
                    .GetEvent<Events.ErrorNotificationEvent>()
                    .Publish(new Events.ErrorNotification(ex));
                Initialize();
            }
        }
    }
}
