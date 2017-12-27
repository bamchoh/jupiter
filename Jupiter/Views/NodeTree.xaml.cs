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
using System.Collections;

namespace Jupiter.Views
{
    /// <summary>
    /// NodeTreeView.xaml の相互作用ロジック
    /// </summary>
    public partial class NodeTree : UserControl
    {
        public static DependencyProperty NodeSelectedCommandProperty =
            DependencyProperty.Register(
                "NodeSelectedCommand",
                typeof(ICommand),
                typeof(NodeTree));

        public ICommand NodeSelectedCommand
        {
            get { return (ICommand)GetValue(NodeSelectedCommandProperty); }
            set { SetValue(NodeSelectedCommandProperty, value); }
        }

        public static DependencyProperty MouseDoubleClickedCommandProperty =
            DependencyProperty.Register(
                "MouseDoubleClickedCommand",
                typeof(ICommand),
                typeof(NodeTree));

        public ICommand MouseDoubleClickedCommand
        {
            get { return (ICommand)GetValue(MouseDoubleClickedCommandProperty); }
            set { SetValue(MouseDoubleClickedCommandProperty, value); }
        }

        public static DependencyProperty AddToReadWriteCommandProperty =
            DependencyProperty.Register(
                "AddToReadWriteCommand",
                typeof(ICommand),
                typeof(NodeTree));

        public ICommand AddToReadWriteCommand
        {
            get { return (ICommand)GetValue(AddToReadWriteCommandProperty); }
            set { SetValue(AddToReadWriteCommandProperty, value); }
        }

        public static DependencyProperty ReloadCommandProperty =
            DependencyProperty.Register(
                "ReloadCommand",
                typeof(ICommand),
                typeof(NodeTree));

        public ICommand ReloadCommand
        {
            get { return (ICommand)GetValue(ReloadCommandProperty); }
            set { SetValue(ReloadCommandProperty, value); }
        }

        public IList SelectedItems;

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var dg = (DataGrid)sender;
            this.SelectedItems = dg.SelectedItems;
            if(this.SelectedItems.Count == 1)
            {
                NodeSelectedCommand.Execute(dg.SelectedItem);
            }
        }

        public NodeTree()
        {
            NodeSelectedCommand = new DelegateCommand(
                (param) => { return; },
                (param) => false);

            MouseDoubleClickedCommand = new DelegateCommand(
                (param) => { return; },
                (param) => false);

            AddToReadWriteCommand = new DelegateCommand(
                (param) => { return; },
                (param) => false);

            ReloadCommand = new DelegateCommand(
                (param) => { return; },
                (param) => false);

            InitializeComponent();
        }

        private void OnItemMouseDoubleClick(Object sender, MouseButtonEventArgs e)
        {
            if(e.Source.Equals(sender))
            {
                MouseDoubleClickedCommand.Execute(SelectedItems);
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var mi = e.Source as MenuItem;
            var cm = mi.Parent as ContextMenu;
            var target = cm.PlacementTarget;
            if(target is DataGridRow)
            {
                AddToReadWriteCommand.Execute(SelectedItems);
            }
        }

        private void TreeViewItem_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is DataGridRow)
            {
                var item = sender as DataGridRow;
                item.IsSelected = true;
                e.Handled = true;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(ReloadCommand.CanExecute(null))
                ReloadCommand.Execute(null);
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            var mi = (MenuItem)e.Source;
            var cm = (ContextMenu)mi.Parent;
            var target = cm.PlacementTarget;
            if (target is DataGridRow)
            {
                MouseDoubleClickedCommand.Execute(SelectedItems);
            }
        }

        private void TreeViewItem_Selected(object sender, RoutedEventArgs e)
        {
            if(e.Source.Equals(sender))
            {
                var obj = ((TreeViewItem)sender).Header;
                NodeSelectedCommand.Execute(obj);
            }
        }
    }
}
