using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Windows.Input;
using System.ComponentModel;

namespace Jupiter.Interfaces
{
    public interface INodeTreeModel: IDisposable, INotifyPropertyChanged
    {
        IReference References { get; set; }

        IList VariableNodes { get; set; }

        bool IsEnabled { get; set; }

        ICommand ReloadCommand { get; set; }
        ICommand MouseDoubleClickedCommand { get; set; }
        ICommand AddToReadWriteCommand { get; set; }
        ICommand NodeSelectedCommand { get; set; }
        ICommand UpdateVariableNodeListCommand { get; set; }
    }
}
