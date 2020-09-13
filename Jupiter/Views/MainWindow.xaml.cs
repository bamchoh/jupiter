﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.ComponentModel;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace Jupiter.Views
{

    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow(Prism.Events.IEventAggregator eventAggregator)
        {
            InitializeComponent();

            eventAggregator.GetEvent<Events.ErrorNotificationEvent>()
                .Subscribe((x) => {
                    var dialogView = new ErrorMessageDialog();
                    dialogView.DataContext = x;
                    MaterialDesignThemes.Wpf.DialogHostEx.ShowDialog(this, dialogView);
                });


            eventAggregator.GetEvent<Events.NowLoadingEvent>()
                .Subscribe(async (x) => {
                    var dialogView = new Loading();
                    var vm = new ViewModels.LoadingViewModel(x);
                    dialogView.DataContext = vm;
                    await x.Semaphore.WaitAsync();
                    try
                    {
                        x.Result = (bool)await MaterialDesignThemes.Wpf.DialogHostEx.ShowDialog(this, dialogView);
                    }
                    finally
                    {
                        x.Semaphore.Release();
                    }
                });

            eventAggregator.GetEvent<Events.SelectedIndexForTabControlChangedEvent>()
                .Subscribe(async (x) =>
                {
                    tabCtrl.SelectedIndex = x.SelectedIndex;
                });
        }

    }
}
