using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections;
using System.Windows.Input;
using System.Windows.Data;

using Prism.Mvvm;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Jupiter.Models
{
    public class OneTimeAccessModel : BindableBase, Interfaces.IOneTimeAccessModel
    {
        private Interfaces.IVariableInfoManager variableInfoManager;
        private ObservableCollection<VariableInfoBase> _itemsToRead;
        private Interfaces.IOneTimeAccessOperator otaOperator;

        public OneTimeAccessModel(
            Interfaces.IConnection connector,
            Interfaces.IOneTimeAccessOperator otaOperator,
            Interfaces.IVariableInfoManager variableInfoManager
            )
        {
            this.variableInfoManager = variableInfoManager;
            this._itemsToRead = new ObservableCollection<VariableInfoBase>();
            this.otaOperator = otaOperator;

            BindingOperations.EnableCollectionSynchronization(_itemsToRead, new object());

            DeleteOneTimeAccessItemsCommand = new Commands.DelegateCommand(
                (param) => { DeleteOneTimeAccessItems(); },
                (param) => { return connector.Connected && (OneTimeAccessItems.Count > 0); });

            ReadCommand = new Commands.DelegateCommand(
                (param) => { OneTimeRead(); },
                (param) => connector.Connected);

            WriteCommand = new Commands.DelegateCommand(
                (param) => { GroupWrite(this._itemsToRead); },
                (param) => connector.Connected);

            connector.ObserveProperty(x => x.Connected).Subscribe(c => { if (!c) Close(); });

        }

        public ICommand DeleteOneTimeAccessItemsCommand { get; set; }
        public ICommand WriteCommand { get; set; }
        public ICommand ReadCommand { get; set; }

        public IList OneTimeAccessItems
        {
            get { return _itemsToRead; }
            private set
            {
                this.SetProperty(ref this._itemsToRead, (ObservableCollection<VariableInfoBase>)value);
            }
        }

        public IList OneTimeAccessSelectedItems { get; set; }

        private void Close()
        {
            OneTimeAccessItems.Clear();
        }

        private void OneTimeRead()
        {
            otaOperator.Read(_itemsToRead);
        }

        private void GroupWrite(IList<VariableInfoBase> items)
        {
            otaOperator.Write(items);
        }

        private void DeleteOneTimeAccessItems()
        {
            var tempList = new ObservableCollection<VariableInfoBase>();

            int lastSelectedIndex;
            if (OneTimeAccessSelectedItems == null)
            { // No items are selected
                if (this._itemsToRead.Count == 0)
                { // No items are registerd
                    // Thus, there are no any items should be deleted
                    return;
                }

                var idx = this._itemsToRead.Count - 1;
                this._itemsToRead[idx].IsSelected = true;
            }

            lastSelectedIndex = this.OneTimeAccessItems.IndexOf(OneTimeAccessSelectedItems[OneTimeAccessSelectedItems.Count - 1]);

            foreach (var vi in this._itemsToRead)
            {
                if (OneTimeAccessSelectedItems.Contains(vi))
                    continue;
                tempList.Add(vi);
            }

            Utility.SelectDeletedLastIndex(tempList, lastSelectedIndex);

            this.OneTimeAccessItems.Clear();
            foreach(var vi in tempList)
            {
                this.OneTimeAccessItems.Add(vi);
            }
        }

        public void AddToReadWrite(IList objs)
        {
            var items = variableInfoManager.NewVariableInfo(objs);
            if (items == null || items.Count == 0)
                return;

            foreach (var vi in items)
            {
                this.OneTimeAccessItems.Add(vi);
            }
        }
    }
}
