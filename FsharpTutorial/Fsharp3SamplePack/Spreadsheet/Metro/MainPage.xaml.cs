using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Grouped Items Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234231

namespace Metro
{
    /// <summary>
    /// A page that displays a grouped collection of items.
    /// </summary>
    public sealed partial class MainPage : Metro.Common.LayoutAwarePage
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property provides the grouped collection of items to be displayed.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.DataContext = e.Parameter;
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
            if (e.Key == Windows.System.VirtualKey.Escape)
            {
                HideEditor(e);
                e.Handled = true;
                return;
            }
            else if (e.Key == Windows.System.VirtualKey.Enter)
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
            textBlock.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            editor.Visibility = Windows.UI.Xaml.Visibility.Visible;

            editor.Focus(FocusState.Programmatic);
        }

        private void HideEditor(RoutedEventArgs e)
        {
            var editor = (TextBox)e.OriginalSource;
            var textBlock = (TextBlock)editor.Tag;
            editor.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            textBlock.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }
    }
}
