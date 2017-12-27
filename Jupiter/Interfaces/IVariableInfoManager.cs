using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace Jupiter.Interfaces
{
    public interface IVariableInfoManager
    {
        IList<VariableInfoBase> NewVariableInfo(IList objs);
    }
}
