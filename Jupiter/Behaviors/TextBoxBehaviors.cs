using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Jupiter.Behaviors
{
    class TextBoxBehaviors
    {
        [AttachedPropertyBrowsableForType(typeof(TextBox))]
        public static bool GetIsFocusSelect(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsFocusSelectProperty);
        }

        [AttachedPropertyBrowsableForType(typeof(TextBox))]
        public static void SetIsFocusSelect(DependencyObject obj, bool value)
        {
            obj.SetValue(IsFocusSelectProperty, value);
        }

        // Trueだとフォーカス取得時にテキストの全選択を行う  
        public static readonly DependencyProperty IsFocusSelectProperty =
            DependencyProperty.RegisterAttached("IsFocusSelect", typeof(bool), typeof(TextBoxBehaviors), new UIPropertyMetadata(false, IsFocusSelectChanged));

        private static void IsFocusSelectChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null) return;

            // 設定された値を見てイベントを登録・削除  
            var newValue = (bool)e.NewValue;
            var oldValue = (bool)e.OldValue;
            if (oldValue)
            {
                textBox.Loaded -= textBox_Loaded;
                textBox.GotFocus -= textBox_GotFocus;
                textBox.MouseDoubleClick -= textBox_GotFocus;
                textBox.PreviewMouseLeftButtonDown -= textBox_SelectivelyIgnoreMouseButton;
            }
            if (newValue)
            {
                textBox.Loaded += textBox_Loaded;
                textBox.GotFocus += textBox_GotFocus;
                textBox.MouseDoubleClick += textBox_GotFocus;
                textBox.PreviewMouseLeftButtonDown += textBox_SelectivelyIgnoreMouseButton;
            }
        }

        static void textBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null) return;
            textBox.SelectAll();
        }

        static void textBox_Loaded(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null) return;
            textBox.Focus();
        }

        static void textBox_SelectivelyIgnoreMouseButton(object sender, MouseButtonEventArgs e)
        {
            var tb = sender as TextBox;
            if (tb == null) return;
            if(!tb.IsKeyboardFocusWithin)
            {
                e.Handled = true;
                tb.Focus();
            }
        }
    }
}
