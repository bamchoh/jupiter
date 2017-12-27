using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Opc.Ua;

namespace Jupiter.Interfaces
{
    public interface INodeInfoGetter
    {
        List<NodeInfo> GetNodeInfoList(ExpandedNodeId nodeid);
    }
}
