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
        private Interfaces.IReferenceFetchable reffetcher;
        private Interfaces.IReference references;
        private ObservableCollection<OPCUAReference> variableNodes = new ObservableCollection<OPCUAReference>();

        public NodeTreeModel(
            Interfaces.IConnection connector, 
            Interfaces.IReferenceFetchable reffetcher, 
            Interfaces.IReference references,
            Interfaces.INodeInfoDataGrid nodeInfoDataGrid,
            Interfaces.ISubscriptionModel subscriptionM,
            Interfaces.IOneTimeAccessModel oneTimeAccessM)
        {
            this.connector = connector;
            this.reffetcher = reffetcher;
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
                (param) => { SelectionChanged((Interfaces.IReference)param, nodeInfoDataGrid); },
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

            UpdateVariableNodes(reference);
        }

        private void UpdateVariableNodes(Interfaces.IReference obj)
        {
            var refs = reffetcher.FetchReferences(obj.NodeId, true);
            if (refs == null || refs.Count == 0)
                return;

            var tempList = new ObservableCollection<OPCUAReference>();
            foreach (var r in refs)
            {
                var child = new OPCUAReference(this.reffetcher, null)
                {
                    DisplayName = r.DisplayName.ToString(),
                    NodeId = r.NodeId,
                    Type = r.NodeClass,
                };
                tempList.Add(child);
            }
            VariableNodes = tempList;
        }

    }
}
