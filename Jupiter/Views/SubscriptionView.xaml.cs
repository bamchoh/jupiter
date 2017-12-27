﻿using System;
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
using System.Windows.Interactivity;
using System.Collections;

namespace Jupiter.Views
{
    /// <summary>
    /// SubscriptionView.xaml の相互作用ロジック
    /// </summary>
    public partial class SubscriptionView : UserControl
    {
        public static DependencyProperty DeleteMonitoredItemsCommandProperty =
            DependencyProperty.Register(
                "DeleteMonitoredItemsCommand",
                typeof(ICommand),
                typeof(SubscriptionView));

        public ICommand DeleteMonitoredItemsCommand
        {
            get { return (ICommand)GetValue(DeleteMonitoredItemsCommandProperty); }
            set { SetValue(DeleteMonitoredItemsCommandProperty, value); }
        }

        public SubscriptionView()
        {
            InitializeComponent();

            DeleteMonitoredItemsCommand = new DelegateCommand(
                (param) => { return; },
                (param) => false);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if(DeleteMonitoredItemsCommand.CanExecute(null))
                DeleteMonitoredItemsCommand.Execute(null);
        }
    }
}
