using System.Collections;
using System.Windows.Input;

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

        public IList OneTimeAccessItems { get; private set; }

        public OneTimeAccessViewModel(Interfaces.IOneTimeAccessModel model)
        {
            this.model = (Models.OneTimeAccessModel)model;

            this.OneTimeAccessItems = this.model.OneTimeAccessItems;

            this.ReadCommand = this.model.ReadCommand;
            this.WriteCommand = this.model.WriteCommand;
            this.DeleteOneTimeAccessItemsCommand = this.model.DeleteOneTimeAccessItemsCommand;
        }

        public ICommand DeleteOneTimeAccessItemsCommand { get; private set; }
        public ICommand WriteCommand { get; private set; }
        public ICommand ReadCommand { get; private set; }
    }
}
