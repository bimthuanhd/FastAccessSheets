using SingleData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using SingleData;
using Autodesk.Revit.UI;


namespace FastAccessSheets.View
{
    /// <summary>
    /// Interaction logic for FastAccessView.xaml
    /// </summary>
    public partial class FastAccessView : Window
    {
        public FastAccessView()
        {
            InitializeComponent();
            Closing += Window_Closing;
        }
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            FormData.Instance = null;
            Data.Instance = null;
        }
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            WatermarkTextBlock.Visibility = Visibility.Collapsed;
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(WatermarkTextBox.Text))
            {
                WatermarkTextBlock.Visibility = Visibility.Visible;
            }
        }
    }
}
