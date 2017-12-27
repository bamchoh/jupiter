using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections;
using System.Windows.Input;

using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;

namespace Jupiter.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        #region Private Fields
        private Interfaces.IConnection client;
        #endregion

        #region Properties
        private object connectionViewModel;
        public object ConnectionViewModel
        {
            get
            {
                return connectionViewModel;
            }

            private set
            {
                this.SetProperty(ref connectionViewModel, value);
            }
        }

        #endregion

        #region Commands
        public ICommand ShowMessageCommand { get; set; }

        public ICommand ClosingCommand { get; set; }

        #endregion

        #region Constructor
        public MainWindowViewModel(Interfaces.IConnection client)
        {
            this.client = client;

            ClosingCommand = new Commands.DelegateCommand(
                (param) => { /* Close(); */ },
                (param) => true);

            // this.ConnectionViewModel = new ViewModels.ConnectionViewModel(client, ShowMessageCommand, references);
        }

        #endregion

        #region Private Methods

        private void SelectDeletedLastIndex(IList<VariableInfoBase> items, int i)
        {
            if (items.Count == 0)
                return;

            if (i < items.Count)
            {
                items[i].IsSelected = true;
            }
            else
            {
                var j = items.Count - 1;
                items[j].IsSelected = true;
            }
        }

        public void Dispose()
        {
            /* Close(); */
        }
        #endregion
    }

}
