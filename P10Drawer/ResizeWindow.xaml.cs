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
using System.Windows.Shapes;

namespace P10Drawer
{
    /// <summary>
    /// Interaction logic for ResizeWindow.xaml
    /// </summary>
    public partial class ResizeWindow : Window
    {
        public ResizeWindow()
        {
            InitializeComponent();
        }

        private void WidhtSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            WidthCount.Text = Math.Round(WidhtSlider.Value).ToString();
        }

        private void HeightSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            HeightCount.Text = Math.Round(HeightSlider.Value).ToString();
        }

        private void ButtonConfirm_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Cols = (int)(Math.Round(WidhtSlider.Value) * 32);
            MainWindow.Rows = (int)(Math.Round(HeightSlider.Value) * 16);
            DialogResult = true;
            this.Close();
        }

        private void ButtonReject_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
