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

using Opc.Ua;

namespace Jupiter.Models
{
    public class NodeTreeModel : BindableBase, Interfaces.INodeTreeModel
    {
        private Interfaces.IConnection connector;
        private Interfaces.IReference references;
        private bool isEnabled;
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
                (param) => { ForceUpdate(); },
                (param) => connector.Connected);

            MouseDoubleClickedCommand = new Commands.DelegateCommand(
                (param) => { subscriptionM.AddToSubscription(param as IList); },
                (param) => true);

            AddToReadWriteCommand = new Commands.DelegateCommand(
                (param) => {
                    AddToReadWrite(oneTimeAccessM, param);
                },
                (param) => connector.Connected);

            NodeSelectedCommand = new Commands.DelegateCommand(
                (param) => { nodeInfoDataGrid.Update((Interfaces.IReference)param); },
                (param) => true);

            UpdateVariableNodeListCommand = new Commands.DelegateCommand(
                (param) => { UpdateVariableNodes((Interfaces.IReference)param); },
                (param) => true);

            this.connector.ObserveProperty(x => x.Connected).Subscribe(c => Update(c));

            Initialize();
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
                var child = (OPCUAReference)obj.NewReference(r.DisplayName.ToString());
                child.NodeId = r.NodeId;
                child.Type = r.NodeClass;
                tempList.Add(child);
            }
            VariableNodes = tempList;
        }

        private void AddToReadWrite(Interfaces.IOneTimeAccessModel oneTimeAccessM, object param)
        {
            var client = connector as Client;

            var itemsToRead = new ReadValueIdCollection();

            var refs = param as IList;

            foreach (OPCUAReference r in refs)
            {
                var rv = new ReadValueId()
                {
                    NodeId = client.ToNodeId(r.NodeId),
                    AttributeId = Attributes.DataType
                };

                itemsToRead.Add(rv);
            }

            DataValueCollection values;
            DiagnosticInfoCollection diagnosticInfos;

            ResponseHeader responseHeader = client.Read(
                itemsToRead,
                out values,
                out diagnosticInfos);

            var varconfs = new List<VariableConfiguration>();

            for(int i = 0;i < values.Count;i++)
            {
                var id = client.ToNodeId(((OPCUAReference)refs[i]).NodeId);
                var datatype = values[i].Value as NodeId;
                var typeinfo = TypeInfo.GetBuiltInType(datatype, client.TypeTable);

                varconfs.Add(new VariableConfiguration(id, typeinfo)
                {
                    Type = NodeClass.Variable
                });
            }

            oneTimeAccessM.AddToReadWrite(varconfs as IList);
        }
    }
}
