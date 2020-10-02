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

using Opc.Ua;
using Prism.Events;
using Unity;

namespace Jupiter.Models
{
    public class OneTimeAccessModel : BindableBase, Interfaces.IOneTimeAccessModel
    {
        [Dependency]
        public IEventAggregator EventAggregator { get; set; }

        private Interfaces.IVariableInfoManager variableInfoManager;
        private ObservableCollection<VariableInfoBase2> _itemsToRead;
        private Interfaces.IOneTimeAccessOperator otaOperator;

        public OneTimeAccessModel(
            Interfaces.IConnection connector,
            Interfaces.IOneTimeAccessOperator otaOperator,
            Interfaces.IVariableInfoManager variableInfoManager
            )
        {
            this.variableInfoManager = variableInfoManager;
            this._itemsToRead = new ObservableCollection<VariableInfoBase2>();
            this.otaOperator = otaOperator;

            BindingOperations.EnableCollectionSynchronization(_itemsToRead, new object());

            DeleteOneTimeAccessItemsCommand = new Commands.DelegateCommand(
                (param) => { DeleteOneTimeAccessItems(); },
                (param) => { return OneTimeAccessItems.Count > 0; });

            ReadCommand = new Commands.DelegateCommand(
                (param) => { OneTimeRead(); },
                (param) => { return connector.Connected && OneTimeAccessItems.Count > 0; });

            WriteCommand = new Commands.DelegateCommand(
                (param) => { GroupWrite(this._itemsToRead); },
                (param) => { return connector.Connected && OneTimeAccessItems.Count > 0; });

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
                this.SetProperty(ref this._itemsToRead, (ObservableCollection<VariableInfoBase2>)value);
            }
        }

        public IList OneTimeAccessSelectedItems { get; set; }

        private void Close()
        {
            OneTimeAccessItems.Clear();
        }

        private void OneTimeRead()
        {
            try
            {
                var itemsToRead = new ReadValueIdCollection();

                foreach (var vi in _itemsToRead)
                {
                    var rv = new ReadValueId()
                    {
                        NodeId = vi.NodeId,
                        AttributeId = Attributes.Value,
                        IndexRange = null,
                        DataEncoding = null,
                    };

                    itemsToRead.Add(rv);
                }

                DataValueCollection values;
                DiagnosticInfoCollection diagnosticInfos;

                ResponseHeader responseHeader = otaOperator.Read(
                    itemsToRead,
                    out values,
                    out diagnosticInfos);

                ClientBase.ValidateResponse(values, itemsToRead);
                ClientBase.ValidateDiagnosticInfos(diagnosticInfos, itemsToRead);

                for (int i = 0; i < values.Count; i++)
                {
                    if(_itemsToRead[i].Type == values[i]?.WrappedValue.TypeInfo?.BuiltInType)
                    {
                        _itemsToRead[i].UpdateDataValue(values[i]);
                    }
                    else
                    {
                        _itemsToRead[i].NewDataValue(values[i]);
                    }
                }

                return;
            }
            catch (Exception ex)
            {
                this.EventAggregator
                    .GetEvent<Events.ErrorNotificationEvent>()
                    .Publish(new Events.ErrorNotification(ex));
                Close();
                return;
            }
        }

        private void GroupWrite(IList<VariableInfoBase2> items)
        {
            otaOperator.Write(items);
        }

        private void DeleteOneTimeAccessItems()
        {
            var tempList = new ObservableCollection<VariableInfoBase2>();

            int lastSelectedIndex;
            if (OneTimeAccessSelectedItems == null || OneTimeAccessItems.Count == 0)
            { // No items are selected
                if (this._itemsToRead.Count == 0)
                { // No items are registerd
                    // Thus, there are no any items should be deleted
                    return;
                }

                var idx = this._itemsToRead.Count - 1;
                this._itemsToRead[idx].IsSelected = true;
            }

            var lastSelectedItem = OneTimeAccessSelectedItems[OneTimeAccessSelectedItems.Count - 1];
            lastSelectedIndex = this.OneTimeAccessItems.IndexOf(lastSelectedItem);

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

        public void AddToReadWrite(IList refs)
        {
            if (refs == null || refs.Count == 0)
                return;

            List<BuiltInType> types = null;

            otaOperator.ReadBuiltInType(refs, out types);

            for(int i = 0;i < refs.Count;i++)
            {
                var vi = (VariableInfoBase2)refs[i];
                var newVI = new VariableInfoBase2(vi.NodeId, vi.DisplayName, types[i]);
                this.OneTimeAccessItems.Add(newVI);
            }
        }
    }
}
