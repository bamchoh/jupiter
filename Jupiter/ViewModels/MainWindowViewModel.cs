using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections;
using System.Windows.Input;
using System.Reflection;

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

        [Unity.Attributes.Dependency]
        public Prism.Events.IEventAggregator EventAggregator { get; set; }

        #region Constructor
        public MainWindowViewModel(Interfaces.IConnection client)
        {
            this.client = client;

            ClosingCommand = new Commands.DelegateCommand(
                (param) => { client.Close(); },
                (param) => true);
        }

        public string Title
        {
            get
            {
                var asm = Assembly.GetExecutingAssembly();
                string assemblyTitle = ((AssemblyTitleAttribute)Attribute.GetCustomAttribute(asm, typeof(AssemblyTitleAttribute))).Title;
                string asmFileVersion = ((AssemblyFileVersionAttribute)Attribute.GetCustomAttribute(asm, typeof(AssemblyFileVersionAttribute))).Version;
                string asmVersion = string.Format("{0}.{1}", asm.GetName().Version.Build, asm.GetName().Version.Revision);
                return string.Format("{0}(Version: {1}, Build: {2})", assemblyTitle, asmFileVersion, asmVersion);
            }
        }

        #endregion
    }

}
