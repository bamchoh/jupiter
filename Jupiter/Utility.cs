using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jupiter
{
    class Utility
    {
        public static void SelectDeletedLastIndex(IList<VariableInfo> items, int i)
        {
            if (items.Count == 0)
                return;

            if (i < items.Count)
            {
                items[i].IsSelected = true;
            }
            else
            {
                var j = items.Count - 1;
                items[j].IsSelected = true;
            }
        }
    }
}
