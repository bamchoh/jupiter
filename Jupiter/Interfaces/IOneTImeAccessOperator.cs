using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using Opc.Ua;

namespace Jupiter.Interfaces
{
    public interface IOneTimeAccessOperator
    {
        ResponseHeader Read(ReadValueIdCollection itemsToRead, out DataValueCollection values, out DiagnosticInfoCollection diagnosticInfos);

        void Write(IList<VariableInfoBase> items);

        VariableConfiguration NewVariableConfiguration(NodeId id);
    }
}
