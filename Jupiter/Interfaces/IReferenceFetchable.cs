using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Opc.Ua;

namespace Jupiter.Interfaces
{
    public interface IReferenceFetchable
    {
        ReferenceDescriptionCollection FetchReferences(NodeId id);

        ResponseHeader Browse(NodeId id, uint mask, out ReferenceDescriptionCollection refs);

        NodeId ToNodeId(ExpandedNodeId id);

        ITypeTable TypeTable { get; }
    }
}
