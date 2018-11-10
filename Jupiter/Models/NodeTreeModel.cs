using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Data;
using System.Collections;
using System.Collections.ObjectModel;

using Prism.Mvvm;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Jupiter.Models
{
    public class NodeTreeModel : BindableBase, Interfaces.INodeTreeModel
    {
        private Interfaces.IConnection connector;
        private Interfaces.IReference references;
        private ObservableCollection<OPCUAReference> variableNodes = new ObservableCollection<OPCUAReference>();

        public NodeTreeModel(
            Interfaces.IConnection connector, 
            Interfaces.IReference references,
            Interfaces.INodeInfoDataGrid nodeInfoDataGrid,
            Interfaces.ISubscriptionModel subscriptionM,
            Interfaces.IOneTimeAccessModel oneTimeAccessM)
        {
            this.connector = connector;
            this.references = references;

            ReloadCommand = new Commands.DelegateCommand(
                (param) => { Load(); },
                (param) => connector.Connected);

            MouseDoubleClickedCommand = new Commands.DelegateCommand(
                (param) => { subscriptionM.AddToSubscription(param as IList); },
                (param) => true);

            AddToReadWriteCommand = new Commands.DelegateCommand(
                (param) => { oneTimeAccessM.AddToReadWrite(param as IList); },
                (param) => connector.Connected);

            NodeSelectedCommand = new Commands.DelegateCommand(
                (param) => { nodeInfoDataGrid.Update((Interfaces.IReference)param); },
                (param) => true);

            UpdateVariableNodeListCommand = new Commands.DelegateCommand(
                (param) => { UpdateVariableNodes((Interfaces.IReference)param); },
                (param) => true);

            this.connector.ObserveProperty(x => x.Connected).Subscribe(c => Update(c));
        }

        public Interfaces.IReference References
        {
            get { return references; }
            set { this.SetProperty(ref references, value); }
        }

        public IList VariableNodes
        {
            get { return variableNodes; }
            set { this.SetProperty(ref variableNodes, (ObservableCollection<OPCUAReference>)value); }
        }

        public ICommand ReloadCommand { get; set; }
        public ICommand MouseDoubleClickedCommand { get; set; }
        public ICommand AddToReadWriteCommand { get; set; }
        public ICommand NodeSelectedCommand { get; set; }
        public ICommand UpdateVariableNodeListCommand { get; set; }

        public void Dispose()
        {
            Close();
        }

        private void Load()
        {
            Close();

            References.UpdateReferences();
        }

        private void Update(bool c)
        {
            if (c)
                Load();
            else
                Close();
        }

        private void Close()
        {
            References?.Children.Clear();

            VariableNodes = new ObservableCollection<OPCUAReference>();
        }

        private void SelectionChanged(Interfaces.IReference reference, Interfaces.INodeInfoDataGrid nodeInfoDataGrid)
        {
            nodeInfoDataGrid.Update(reference);
        }

        private void UpdateVariableNodes(Interfaces.IReference obj)
        {
            var refs = obj.FetchVariableReferences();
            var tempList = new ObservableCollection<OPCUAReference>();

            if (refs == null || refs.Count == 0)
            {
                VariableNodes = tempList;
                return;
            }

            foreach (var r in refs)
            {
                var child = obj.NewReference(r.DisplayName.ToString());
                child.NodeId = r.NodeId;
                child.Type = r.NodeClass;
                tempList.Add((OPCUAReference)child);
            }
            VariableNodes = tempList;
        }

    }
}
