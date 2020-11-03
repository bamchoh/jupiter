using System;
using System.Collections.Generic;
using System.Text;
using Opc.Ua;

namespace Jupiter.Interfaces
{
    public interface IVariableInfo
    {
        public NodeId NodeId { get; set; }
    }
}
