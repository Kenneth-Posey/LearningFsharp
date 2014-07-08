using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Metro;

namespace SilverlightApplication1
{
    public partial class MainPage : UserControl
    {
        public MainPage()
        {
            InitializeComponent();
        }

        void OnLostFocus(object sender, RoutedEventArgs e)
        {
            var editor = (TextBox)e.OriginalSource;
            var text = editor.Text;

            HideEditor(e);

            EditValue(editor.DataContext, text);
        }

        void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                HideEditor(e);
                e.Handled = true;
                return;
            }
            else if (e.Key == Key.Enter)
            {
                var editor = (TextBox)e.OriginalSource;
                var text = editor.Text;

                HideEditor(e);

                EditValue(editor.DataContext, text);
                e.Handled = true;
            }
        }

        private void EditValue(object dataContext, string newText)
        {
            var cvm = (CellViewModel)dataContext;
            cvm.SetCellValue(newText);
        }

        private void OnPointerPressed(object sender, RoutedEventArgs e)
        {
            var textBlock = (TextBlock)e.OriginalSource;
            var editor = (TextBox)textBlock.Tag;
            textBlock.Visibility = Visibility.Collapsed;
            editor.Visibility = Visibility.Visible;
            editor.Focus();
        }

        private void HideEditor(RoutedEventArgs e)
        {
            var editor = (TextBox)e.OriginalSource;
            var textBlock = (TextBlock)editor.Tag;
            editor.Visibility = Visibility.Collapsed;
            textBlock.Visibility = Visibility.Visible;
        }
    }
}
