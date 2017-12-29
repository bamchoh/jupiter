﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using Opc.Ua;

namespace Jupiter.Interfaces
{
    public interface IVariableInfoManager
    {
        IList<VariableInfoBase> NewVariableInfo(IList objs);

        VariableInfoBase NewVariableInfo(ExpandedNodeId id);
    }
}
