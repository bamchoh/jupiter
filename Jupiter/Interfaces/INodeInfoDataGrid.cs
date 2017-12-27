using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jupiter.Interfaces
{
    public interface INodeInfoDataGrid : IDisposable
    {
        void Update(IReference reference);
    }
}
