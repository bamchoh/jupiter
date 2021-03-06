﻿using System;
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

        #region Constructor
        public MainWindowViewModel(Interfaces.IConnection client)
        {
            this.client = client;

            ClosingCommand = new Commands.DelegateCommand(
                (param) => { client.Close(); },
                (param) => true);

            SelectedIndexForTabControl = 1;
        }

        public string Title
        {
            get
            {
                var asm = Assembly.GetExecutingAssembly();
                var execDir = System.IO.Path.GetDirectoryName(asm.Location);

                var opcuaasm = Assembly.LoadFrom(System.IO.Path.Combine(execDir, "Opc.Ua.Core.dll"));
                string opcuaFileVersion = ((AssemblyFileVersionAttribute)Attribute.GetCustomAttribute(opcuaasm, typeof(AssemblyFileVersionAttribute))).Version;

                string assemblyTitle = ((AssemblyTitleAttribute)Attribute.GetCustomAttribute(asm, typeof(AssemblyTitleAttribute))).Title;
                string asmFileVersion = ((AssemblyFileVersionAttribute)Attribute.GetCustomAttribute(asm, typeof(AssemblyFileVersionAttribute))).Version;
                string title = string.Format("{0}(Version: {1}, Opc.Ua.Core.dll: {2})", assemblyTitle, asmFileVersion, opcuaFileVersion);
                return title;
            }
        }

        public int SelectedIndexForTabControl { get; set; }

        #endregion
    }

}
