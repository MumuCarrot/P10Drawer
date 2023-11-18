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
        /// <summary>
        /// Initialization of window dependencies
        /// </summary>
        public ResizeWindow()
        {
            InitializeComponent();

            InitVisualGrid();
        }

        /// <summary>
        /// Initialisation of external dependencies
        /// </summary>
        private void InitVisualGrid() 
        {
            HeightSlider.Value = MainWindow.Rows / 16;
            WidthSlider.Value = MainWindow.Cols / 32;
        }

        /// <summary>
        /// Call for Slider_ValueChanged after Canvas was loaded, to avert bugs
        /// </summary>
        /// <param name="sender">Sender element</param>
        /// <param name="e">Sended event by sender</param>
        private void VisualizeCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            Slider_ValueChanged(null, null);
        }

        /// <summary>
        /// Logic of Sliders
        /// Initializes rectangles depending on slider values
        /// </summary>
        /// <param name="sender">Sender element</param>
        /// <param name="e">Sended event by sender</param>
        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (WidthSlider != null) WidthCount.Text = Math.Round(WidthSlider.Value).ToString();
            if (HeightSlider != null) HeightCount.Text = Math.Round(HeightSlider.Value).ToString();

            if (visualizeCanvas != null && visualizeCanvas.Children.Count > 0) visualizeCanvas.Children.Clear();

            if (WidthSlider != null && HeightSlider != null && visualizeCanvas != null)
            {
                double rectWidth = 32;
                double rectHeight = 16;
                double canvasWidth = visualizeCanvas.ActualWidth;
                double canvasHeight = visualizeCanvas.ActualHeight;

                double totalRectsWidth = Math.Round(WidthSlider.Value) * rectWidth;
                double totalRectsHeight = Math.Round(HeightSlider.Value) * rectHeight;

                double startX = (canvasWidth - totalRectsWidth) / 2;
                double startY = (canvasHeight - totalRectsHeight) / 2;

                for (int i = 0; i < (int)Math.Round(WidthSlider.Value); i++)
                {
                    for (int j = 0; j < (int)Math.Round(HeightSlider.Value); j++)
                    {
                        Rectangle newRect = new Rectangle()
                        {
                            Height = rectHeight,
                            Width = rectWidth,
                            Fill = Brushes.Black,
                            Margin = new Thickness(startX + i * (rectWidth + 3), startY + j * (rectHeight + 3), 0, 0)
                        };
                        visualizeCanvas.Children.Add(newRect);
                    }
                }
            }
        }

        /// <summary>
        /// Confirmation
        /// </summary>
        /// <param name="sender">Sender element</param>
        /// <param name="e">Sended event by sender</param>
        private void ButtonConfirm_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Cols = (int)(Math.Round(WidthSlider.Value) * 32);
            MainWindow.Rows = (int)(Math.Round(HeightSlider.Value) * 16);
            DialogResult = true;
            this.Close();
        }

        /// <summary>
        /// Rejecting just close window
        /// </summary>
        /// <param name="sender">Sender element</param>
        /// <param name="e">Sended event by sender</param>
        private void ButtonReject_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
