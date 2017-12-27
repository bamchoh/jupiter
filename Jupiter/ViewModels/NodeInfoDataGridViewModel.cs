using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

using Prism.Mvvm;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Jupiter.ViewModels
{
    public class NodeInfoDataGridViewModel
    {
        public ReadOnlyReactiveCollection<NodeInfo> NodeInfoList { get; private set; }

        public Models.NodeInfoDataGridModel model;

        public NodeInfoDataGridViewModel(Models.NodeInfoDataGridModel model)
        {
            this.model = model;

            this.NodeInfoList = model.NodeInfoList.ToReadOnlyReactiveCollection();
        }
    }
}
