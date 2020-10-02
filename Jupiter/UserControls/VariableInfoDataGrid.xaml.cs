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
using System.Collections;
using System.Windows.Controls.Primitives;

namespace Jupiter.UserControls
{
    /// <summary>
    /// VariableInfoDataGrid.xaml の相互作用ロジック
    /// </summary>
    public partial class VariableInfoDataGrid : UserControl
    {
        public VariableInfoDataGrid()
        {
            InitializeComponent();
        }

        public object ItemsSource
        {
            get { return (object)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(object), typeof(VariableInfoDataGrid), new PropertyMetadata(null));

        public IList SelectedItems
        {
            get { return (IList)GetValue(SelectedItemsProperty); }
            set { SetValue(SelectedItemsProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register("SelectedItems", typeof(object), typeof(VariableInfoDataGrid), new PropertyMetadata(null));

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.SelectedItems = ((DataGrid)sender).SelectedItems;
        }

        public ICommand DeleteCommand
        {
            get { return (ICommand)GetValue(DeleteCommandProperty); }
            set { SetValue(DeleteCommandProperty, value); }
        }

        public static readonly DependencyProperty DeleteCommandProperty =
            DependencyProperty.Register("DeleteCommand", typeof(object), typeof(VariableInfoDataGrid), new PropertyMetadata(null));

        private void DataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var dg = (DataGrid)sender;
            if (dg == null)
                return;

            var elem = dg.CurrentColumn.GetCellContent(dg.CurrentItem);
            var checkbox = GetElement<CheckBox>(elem);

            if (checkbox != null && checkbox.Visibility == Visibility.Visible)
            {
                switch(e.Key)
                {
                    case Key.Space:
                        {
                            checkbox.IsChecked = !checkbox.IsChecked;
                            e.Handled = true;
                            break;
                        }
                }
            }
            else
            {
                var cell = e.OriginalSource as DataGridCell;
                if (cell == null)
                    return;

                if (!cell.IsEditing)
                {
                    switch (e.Key)
                    {
                        case Key.Delete:
                            {
                                if (DeleteCommand != null && DeleteCommand.CanExecute(null))
                                {
                                    DeleteCommand.Execute(null);
                                }
                            }
                            break;
                        case Key.Enter:
                            {
                                dg.BeginEdit();
                                e.Handled = true;
                            }
                            break;
                    }
                }
            }
        }

        private void MoveCurrentItem(DataGrid dg, int oldIndex, int newIndex)
        {
            var item = dg.Items[oldIndex];
            var itemSource = dg.ItemsSource as IList;
            itemSource.RemoveAt(oldIndex);
            itemSource.Insert(newIndex, item);
            FocusDataGridCell(dg, newIndex);
        }

        private void FocusDataGridCell(DataGrid dg, int index)
        {
            dg.Focus();
            dg.ScrollIntoView(dg.Items[index]);
            dg.UpdateLayout();
            var dgr = dg.ItemContainerGenerator.ContainerFromIndex(index) as DataGridRow;
            var dgc = dg.CurrentCell.Column.GetCellContent(dgr).Parent as DataGridCell;
            dgc.Focus();
        }

        private T GetElement<T>(DependencyObject reference) where T : FrameworkElement
        {
            if (reference is T)
                return reference as T;

            DependencyObject elem = null;
            for(int i = 0;i < VisualTreeHelper.GetChildrenCount(reference);i++)
            {
                elem = GetChildElement<T>(reference, i);
                if (elem != null)
                    break;
            }

            return elem as T;
        }

        private T GetChildElement<T>(DependencyObject reference, int j) where T : FrameworkElement
        {
            var child = VisualTreeHelper.GetChild(reference, j);
            if (child == null)
                return null;

            if (child is T)
                return child as T;

            DependencyObject elem = reference;
            for(int i=0;i<VisualTreeHelper.GetChildrenCount(child);i++)
            {
                elem = GetChildElement<T>(child, i);
                if (elem != null)
                    break;
            }

            return elem as T;
        }
    }
}