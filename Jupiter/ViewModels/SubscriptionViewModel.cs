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
        public IList SelectedMonitoredItems { get; set; }

        public IList MonitoredItems { get; set; }

        public SubscriptionViewModel(Interfaces.ISubscriptionModel model)
        {
            this.model = (Models.SubscriptionModel)model;

            this.MonitoredItems = this.model.MonitoredItems;

            this.SelectedMonitoredItems = this.model.SelectedMonitoredItems;

            this.DeleteMonitoredItemsCommand = this.model.DeleteMonitoredItemsCommand;
        }

        public ICommand DeleteMonitoredItemsCommand { get; set; }
    }
}
