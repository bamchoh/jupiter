using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using Opc.Ua;

namespace Jupiter.Interfaces
{
    public interface IVariableConfiguration
    {
        BuiltInType BuiltInType();

        NodeId VariableNodeId();

        string DisplayName { get; set; }

        NodeClass NodeClass { get; set; }
    }

    public interface IVariableInfoManager2
    {
        IList<VariableInfo> GenerateVariableInfoList(IList objs);

        VariableInfo NewVariableInfo(IVariableConfiguration reference);
    }
}
