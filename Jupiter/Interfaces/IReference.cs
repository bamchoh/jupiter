using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using Opc.Ua;
using Prism.Events;

namespace Jupiter.Interfaces
{
    public interface IReference
    {
        IReference Parent { get; }

        IList Children { get; }

        ExpandedNodeId NodeId { get; set; }

        void UpdateReferences();

        IReference NewReference(string name);

        IEventAggregator GetEventAggregator();
    }
}
