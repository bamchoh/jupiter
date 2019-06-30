using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Collections;

using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Jupiter.ViewModels
{
    public class NodeTreeViewModel : BindableBase
    {
        private Interfaces.INodeTreeModel model;

        public NodeTreeViewModel(Interfaces.INodeTreeModel model)
        {
            this.model = model;

            ReloadCommand = model.ReloadCommand;
            MouseDoubleClickedCommand = model.MouseDoubleClickedCommand;
            AddToReadWriteCommand = model.AddToReadWriteCommand;
            NodeSelectedCommand = model.NodeSelectedCommand;
            UpdateVariableNodeListCommand = model.UpdateVariableNodeListCommand;

            References = model.ToReactivePropertyAsSynchronized(x => x.References);
            VariableNodes = model.ToReactivePropertyAsSynchronized(x => x.VariableNodes);
            IsEnabled = model.ToReactivePropertyAsSynchronized(x => x.IsEnabled);
        }

        public ICommand ReloadCommand { get; set; }
        public ICommand MouseDoubleClickedCommand { get; set; }
        public ICommand AddToReadWriteCommand { get; set; }
        public ICommand NodeSelectedCommand { get; set; }
        public ICommand UpdateVariableNodeListCommand { get; set; }

        public ReactiveProperty<Interfaces.IReference> References { get; }
        public ReactiveProperty<IList> VariableNodes { get; }
        public ReactiveProperty<bool> IsEnabled { get; }
    }
}
