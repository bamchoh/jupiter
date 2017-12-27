using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Collections;
using System.Collections.ObjectModel;

using Prism.Mvvm;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Jupiter.ViewModels
{
    class SubscriptionViewModel : BindableBase
    {
        private Models.SubscriptionModel model;
        public IList MonitoredSelectedItems
        {
            get { return model.MonitoredSelectedItems; }
            set { model.MonitoredSelectedItems = value; }
        }

        public IList MonitoredItemList { get; set; }

        public SubscriptionViewModel(Models.SubscriptionModel model)
        {
            this.model = model;

            this.MonitoredItemList = model.MonitoredItemList;

            this.DeleteMonitoredItemsCommand = model.DeleteMonitoredItemsCommand;
        }

        public ICommand DeleteMonitoredItemsCommand { get; set; }

    }
}
