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
        public IList SelectedMonitoredItems
        {
            get { return model.SelectedMonitoredItems; }
            set { model.SelectedMonitoredItems = value; }
        }

        public ReactiveProperty<IList> MonitoredItems
        {
            get;
        }
            

        public SubscriptionViewModel(Interfaces.ISubscriptionModel model)
        {
            this.model = (Models.SubscriptionModel)model;

            MonitoredItems = this.model.ToReactivePropertyAsSynchronized(
                x => x.MonitoredItems);

            this.DeleteMonitoredItemsCommand = this.model.DeleteMonitoredItemsCommand;
        }

        public ICommand DeleteMonitoredItemsCommand { get; set; }
    }
}
