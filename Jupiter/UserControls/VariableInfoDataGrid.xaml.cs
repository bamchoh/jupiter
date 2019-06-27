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
            if (!isEdit)
            {
                switch(e.Key)
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

        bool isEdit = false;

        private void DataGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            isEdit = true;
        }

        private void DataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            isEdit = false;
        }
    }
}