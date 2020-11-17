using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Data;
using System.Threading;
using System.Threading.Tasks;

using ICSharpCode.AvalonEdit.Document;
using Prism.Mvvm;
using Prism.Commands;
using System.Windows.Forms;

namespace Jupiter.ViewModels
{
    class ScriptViewModel : BindableBase
    {
        public TextDocument Script
        {
            get { return _script; }
            set { SetProperty(ref _script, value); }
        }

        public ObservableCollection<string> ScriptOutput
        {
            get { return log.Log; }
        }

        public bool IsRepeat
        {
            get { return _isRepeat; }
            set { SetProperty(ref _isRepeat, value); }
        }

        public bool IsRunning
        {
            get { return _isRunning; }
            set { SetProperty(ref _isRunning, value); }
        }

        public ICommand RunCommand { get; private set; }

        public ICommand StopCommand { get; private set; }

        public ICommand RepeatCommand { get; private set; }

        public ICommand LoadCommand { get; private set; }

        private TextDocument _script;

        private ScriptEngine.V8Engine engine;

        private LogStream log;

        private CancellationTokenSource tokenSource;

        private bool _isRepeat = false;

        private bool _isRunning = false;

        public ScriptViewModel(Interfaces.IConnection client)
        {
            Script = new TextDocument();

            log = new LogStream();

            log.PropertyChanged += Log_PropertyChanged;

            this.engine = new ScriptEngine.V8Engine((Client)client);

            RunCommand = new DelegateCommand(() => 
            {
                Task.Run(() =>
                {
                    IsRunning = true;

                    this.tokenSource = new CancellationTokenSource();

                    var cancel = tokenSource.Token;

                    Task.Run(() => {
                        do
                        {
                            engine.Run(Script.ToString(), log, cancel);
                        } while (_isRepeat && !cancel.IsCancellationRequested);
                        IsRunning = false;
                    }, cancel);

                });
            });

            StopCommand = new DelegateCommand(() =>
            {
                engine.Stop();
                this.tokenSource.Cancel();
                IsRunning = false;
            });

            RepeatCommand = new DelegateCommand(() =>
            {
                IsRepeat = !_isRepeat;
            });

            LoadCommand = new DelegateCommand(() =>
            {
                using(var ofd = new OpenFileDialog())
                {
                    ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    ofd.Filter = "Javascript files (*.js)|*.js|All files (*.*)|*.*";
                    ofd.FilterIndex = 1;
                    ofd.RestoreDirectory = true;

                    if(ofd.ShowDialog() == DialogResult.OK)
                    {
                        Script = new TextDocument(File.ReadAllText(ofd.FileName, System.Text.Encoding.UTF8));
                    }
                }
            });
        }

        private void Log_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            RaisePropertyChanged("ScriptOutput");
        }
    }

    public class LogStream : BindableBase
    {
        private ObservableCollection<string> _log;

        public ObservableCollection<string> Log
        {
            get
            {
                return _log;
            }

            set
            {
                _log = value;
            }
        }

        public LogStream()
        {
            _log = new ObservableCollection<string>();

            BindingOperations.EnableCollectionSynchronization(_log, new object());
        }

        public void Write(string text)
        {
            _log.Add(text);
            RaisePropertyChanged("Log");
        }
    }
}
