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

        NodeClass Type { get; set; }
    }

    public interface IVariableInfoManager
    {
        IList<VariableInfoBase> GenerateVariableInfoList(IList objs);

        VariableInfoBase NewVariableInfo(IVariableConfiguration reference);
    }
}
