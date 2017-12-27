using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Jupiter.Commands;

namespace Jupiter.Views
{
    /// <summary>
    /// OneTimeAccessView.xaml の相互作用ロジック
    /// </summary>
    public partial class OneTimeAccessView : UserControl
    {
        public static DependencyProperty ReadCommandProperty =
            DependencyProperty.Register(
                "ReadCommand",
                typeof(ICommand),
                typeof(OneTimeAccessView));

        public ICommand ReadCommand
        {
            get { return (ICommand)GetValue(ReadCommandProperty); }
            set { SetValue(ReadCommandProperty, value); }
        }

        public static DependencyProperty WriteCommandProperty =
            DependencyProperty.Register(
                "WriteCommand",
                typeof(ICommand),
                typeof(OneTimeAccessView));

        public ICommand WriteCommand
        {
            get { return (ICommand)GetValue(WriteCommandProperty); }
            set { SetValue(WriteCommandProperty, value); }
        }

        public static DependencyProperty DeleteCommandProperty =
            DependencyProperty.Register(
                "DeleteCommand",
                typeof(ICommand),
                typeof(OneTimeAccessView));

        public ICommand DeleteCommand
        {
            get { return (ICommand)GetValue(DeleteCommandProperty); }
            set { SetValue(DeleteCommandProperty, value); }
        }


        public OneTimeAccessView()
        {
            InitializeComponent();

            ReadCommand = new DelegateCommand(
                (param) => { return; },
                (param) => false);

            WriteCommand = new DelegateCommand(
                (param) => { return; },
                (param) => false);

            DeleteCommand = new DelegateCommand(
                (param) => { return; },
                (param) => false);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(ReadCommand.CanExecute(null))
                ReadCommand.Execute(null);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if(DeleteCommand.CanExecute(null))
                DeleteCommand.Execute(null);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (WriteCommand.CanExecute(null))
                WriteCommand.Execute(null);
        }
    }
}
