using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace Jupiter.Interfaces
{
    public interface IOneTimeAccessOperator
    {
        void Read(IList<VariableInfoBase> items);

        void Write(IList<VariableInfoBase> items);
    }
}
