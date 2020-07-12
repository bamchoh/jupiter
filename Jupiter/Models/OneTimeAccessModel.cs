﻿using System;
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
using Unity.Attributes;

namespace Jupiter.Models
{
    public class OneTimeAccessModel : BindableBase, Interfaces.IOneTimeAccessModel
    {
        [Dependency]
        public IEventAggregator EventAggregator { get; set; }

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
                    var conf = _itemsToRead[i].VariableConfiguration;
                    var vi = variableInfoManager.NewVariableInfo(conf);
                    vi.SetItem(_itemsToRead[i].NodeId, _itemsToRead[i].DisplayName, _itemsToRead[i].ClientHandle, values[i]);
                    var isSelected = _itemsToRead[i].IsSelected;
                    vi.SetPrepareValue(_itemsToRead[i].GetPrepareValue());
                    _itemsToRead[i] = vi;
                    vi.IsSelected = isSelected;
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

        private void GroupWrite(IList<VariableInfoBase> items)
        {
            otaOperator.Write(items);
        }

        private void DeleteOneTimeAccessItems()
        {
            var tempList = new ObservableCollection<VariableInfoBase>();

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
            var items = variableInfoManager.GenerateVariableInfoList(refs);
            if (items == null || items.Count == 0)
                return;

            foreach (var vi in items)
            {
                this.OneTimeAccessItems.Add(vi);
            }
        }
    }
}
