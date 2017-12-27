using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

using Prism.Mvvm;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Jupiter.Models
{
    public class NodeInfoDataGridModel : BindableBase, Interfaces.INodeInfoDataGrid
    {
        private Interfaces.IConnection connector;
        private Interfaces.INodeInfoGetter client;

        public ObservableCollection<NodeInfo> NodeInfoList;

        public NodeInfoDataGridModel(
            Interfaces.IConnection connector,
            Interfaces.INodeInfoGetter client)
        {
            this.client = client;
            this.connector = connector;

            this.NodeInfoList = new ObservableCollection<NodeInfo>();

            this.connector.ObserveProperty(x => x.Connected).Subscribe(c => { if (!c) Close(); });
        }

        public void Update(Interfaces.IReference reference)
        {
            NodeInfoList.Clear();

            var nodeInfoList = client.GetNodeInfoList(reference.NodeId);

            if (nodeInfoList == null)
                return;

            foreach (var nodeinfo in nodeInfoList)
            {
                NodeInfoList.Add(nodeinfo);
            }
        }

        public void Dispose()
        {
            Close();
        }

        private void Close()
        {
            NodeInfoList?.Clear();
        }
    }
}
