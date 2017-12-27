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
    class OneTimeAccessViewModel
    {
        private Models.OneTimeAccessModel model;
        public IList OneTimeAccessSelectedItems
        {
            get { return model.OneTimeAccessSelectedItems; }
            set { model.OneTimeAccessSelectedItems = value; }
        }

        public IList OneTimeAccessItems { get; set; }

        public OneTimeAccessViewModel(Models.OneTimeAccessModel model)
        {
            this.model = model;

            this.OneTimeAccessItems = model.OneTimeAccessItems;

            this.DeleteOneTimeAccessItemsCommand = model.DeleteOneTimeAccessItemsCommand;

            this.ReadCommand = model.ReadCommand;
            this.WriteCommand = model.WriteCommand;
            this.DeleteOneTimeAccessItemsCommand = model.DeleteOneTimeAccessItemsCommand;
        }

        public ICommand DeleteOneTimeAccessItemsCommand { get; set; }
        public ICommand WriteCommand { get; set; }
        public ICommand ReadCommand { get; set; }
    }
}
