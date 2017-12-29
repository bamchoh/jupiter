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
    public class MainWindowViewModel
    {
        #region Private Fields
        private Interfaces.IConnection client;
        #endregion

        #region Commands
        public ICommand ClosingCommand { get; set; }
        #endregion

        #region Constructor
        public MainWindowViewModel(Interfaces.IConnection client)
        {
            this.client = client;

            ClosingCommand = new Commands.DelegateCommand(
                (param) => { client.Close(); },
                (param) => true);
        }

        #endregion
    }

}
