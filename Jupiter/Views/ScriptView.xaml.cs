using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Xml;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;

namespace Jupiter.Views
{
    /// <summary>
    /// ScriptPage.xaml の相互作用ロジック
    /// </summary>
    public partial class ScriptView : UserControl
    {
        public ScriptView()
        {
            InitializeComponent();
            using (MemoryStream ms = new MemoryStream())
            {
                var src = Properties.Resources.JavaScript_Mode;
                ms.Write(src, 0, src.Length);
                ms.Seek(0, SeekOrigin.Begin);
                using (var reader = new XmlTextReader(ms))
                {
                    avalonEdit_TextEditor.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }
        }

        private void CopyToClipboard_Click(object sender, RoutedEventArgs e)
        {
            var s = "";
            foreach(var i in scriptOutputListBox.SelectedItems)
            {
                s += i.ToString() + "\n";
            }
            Clipboard.SetText(s);
        }
    }
}
