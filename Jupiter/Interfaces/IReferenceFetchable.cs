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
        ReferenceDescriptionCollection FetchReferences(ExpandedNodeId nodeid, bool onlyVariable = false);

        ReferenceDescriptionCollection FetchRootReferences();

        INode FindNode(ExpandedNodeId id);

        ITypeTable TypeTable { get; }
    }
}
