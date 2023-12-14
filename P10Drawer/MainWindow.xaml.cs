using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace P10Drawer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Initialisation of the MainWindow
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        // Variables of the P10Drawer region
        #region Variables of the P10Drawer
        /// <summary>
        /// Rows counter
        /// </summary>
        public static int Rows { get; set; } = 16;
        /// <summary>
        /// Columns counter
        /// </summary>
        public static int Cols { get; set; } = 32;
        /// <summary>
        /// Pixel actual size
        /// </summary>
        private int Size { get; set; } = 5;
        /// <summary>
        /// Inversion
        /// </summary>
        private bool inversion = false;
        // Cursor state region
        #region Cursor states
        /// <summary>
        /// Cursor state enum | Variation of the cursour
        /// </summary>
        private enum CURSOR
        {
            Brush,
            Eraser
        }
        /// <summary>
        /// Cursor actual stat
        /// </summary>
        private CURSOR cursor = CURSOR.Brush;
        #endregion // EO Cursor state region
        /// <summary>
        /// Is nedded to upload previous action
        /// </summary>
        private string Save { get; set; } = string.Empty;
        #endregion // EO Variables of the P10Drawer

        // Methods of the P10Drawer region
        #region Methods of the P10Drawer

        // Helper methods region
        #region Helper methods region
        /// <summary>
        /// Creates and resizing drawing grid
        /// </summary>
        /// <param name="resizeWindow">
        /// Should be used if it resizing
        /// </param>
        /// <param name="sizeChange">
        /// Should be used if it save load when size of pixels needed ot be changed
        /// </param>
        public void CreateGridBySize(bool resizeWindow = false, bool sizeChange = false)
        {
            if (resizeWindow)
            {
                drawCanvas.Children.Clear();
                SuperGrid.ColumnDefinitions.Clear();
                SuperGrid.RowDefinitions.Clear();
            }

            if (sizeChange)
            {
                SuperGrid.ColumnDefinitions.Clear();
                SuperGrid.RowDefinitions.Clear();
            }

            for (int i = 0; i < Cols; i++)
            {
                ColumnDefinition columnDefinition = new ColumnDefinition() { Width = new GridLength(Size, GridUnitType.Pixel) };
                SuperGrid.ColumnDefinitions.Add(columnDefinition);
            }

            for (int i = 0; i < Rows; i++)
            {
                RowDefinition rowDefinition = new RowDefinition() { Height = new GridLength(Size, GridUnitType.Pixel) };
                SuperGrid.RowDefinitions.Add(rowDefinition);
            }

            if (sizeChange)
                RecreateFromSave(Save);
        }
        /// <summary>
        /// Parse canvas to binary array
        /// 0 - black
        /// 1 - white
        /// </summary>
        private string GetCanvasInfo()
        {
            List<string> keys = new List<string>();

            foreach (Rectangle rec in drawCanvas.Children)
            {
                int x = (int)Canvas.GetTop(rec) / Size;
                int y = (int)Canvas.GetLeft(rec) / Size;
                if (!keys.Contains($"{x}" + "#" + $"{y}")) keys.Add($"{x}" + "#" + $"{y}");
            }

            string answer = string.Empty;

            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    if (!inversion)
                    {
                        if (keys.Contains($"{i}" + "#" + $"{j}")) answer += "1";
                        else answer += "0";
                    }
                    else if (inversion)
                    {
                        if (keys.Contains($"{i}" + "#" + $"{j}")) answer += "0";
                        else answer += "1";
                    }
                }
            }

            return answer;
        }
        #endregion // EO Helper methods region

        // Drawing region
        #region Drawing region
        /// <summary>
        /// Drawing lines
        /// </summary>
        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {

                Point mousePosition = e.GetPosition(drawCanvas);
                HitTestResult hitTestResult = VisualTreeHelper.HitTest(drawCanvas, mousePosition);

                if ((mousePosition.X >= 0 && mousePosition.X <= drawCanvas.ActualWidth - 1) &&
                    (mousePosition.Y >= 0 && mousePosition.Y <= drawCanvas.ActualHeight - 1))
                {
                    int column = (int)(mousePosition.X / (drawCanvas.ActualWidth / Cols));
                    int row = (int)(mousePosition.Y / (drawCanvas.ActualHeight / Rows));

                    Rectangle rectangle;

                    if ((cursor == CURSOR.Brush) && (hitTestResult != null && !(hitTestResult.VisualHit is Rectangle)))
                    {

                        if (!inversion)
                            rectangle = new Rectangle
                            {
                                Width = Size,
                                Height = Size,
                                Fill = Brushes.White,
                                Stroke = Brushes.White,
                                StrokeThickness = 1
                            };
                        else
                            rectangle = new Rectangle
                            {
                                Width = Size,
                                Height = Size,
                                Fill = Brushes.Black,
                                Stroke = Brushes.Black,
                                StrokeThickness = 1
                            };

                        Canvas.SetLeft(rectangle, column * rectangle.Width);
                        Canvas.SetTop(rectangle, row * rectangle.Height);

                        drawCanvas.Children.Add(rectangle);

                    }
                    else if (cursor == CURSOR.Eraser)
                    {

                        if (drawCanvas.InputHitTest(mousePosition) is UIElement element)
                        {
                            drawCanvas.Children.Remove(element);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Drawing dots
        /// </summary>
        private void Canvas_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {

                Point mousePosition = e.GetPosition(drawCanvas);
                HitTestResult hitTestResult = VisualTreeHelper.HitTest(drawCanvas, mousePosition);

                if ((mousePosition.X >= 0 && mousePosition.X <= drawCanvas.ActualWidth - 1) &&
                    (mousePosition.Y >= 0 && mousePosition.Y <= drawCanvas.ActualHeight - 1))
                {
                    int column = (int)(mousePosition.X / (drawCanvas.ActualWidth / Cols));
                    int row = (int)(mousePosition.Y / (drawCanvas.ActualHeight / Rows));

                    Rectangle rectangle;

                    if ((cursor == CURSOR.Brush) && (hitTestResult != null && !(hitTestResult.VisualHit is Rectangle)))
                    {

                        if (!inversion)
                            rectangle = new Rectangle
                            {
                                Width = Size,
                                Height = Size,
                                Fill = Brushes.White,
                                Stroke = Brushes.White,
                                StrokeThickness = 1
                            };
                        else
                            rectangle = new Rectangle
                            {
                                Width = Size,
                                Height = Size,
                                Fill = Brushes.Black,
                                Stroke = Brushes.Black,
                                StrokeThickness = 1
                            };

                        Canvas.SetLeft(rectangle, column * rectangle.Width);
                        Canvas.SetTop(rectangle, row * rectangle.Height);

                        drawCanvas.Children.Add(rectangle);

                    }
                    else if (cursor == CURSOR.Eraser)
                    {

                        if (drawCanvas.InputHitTest(mousePosition) is UIElement element)
                        {
                            drawCanvas.Children.Remove(element);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Begining of the mous capture
        /// </summary>
        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            drawCanvas.CaptureMouse();
        }
        /// <summary>
        /// Release mouse capture
        /// </summary>
        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            drawCanvas.ReleaseMouseCapture();
        }
        /// <summary>
        /// Setts for the cursor status
        /// </summary>
        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is ToggleButton clickedButton)) return;

            if (clickedButton == BrushButton && BrushButton.IsChecked == true)
            {
                // Включить режим "Кисть"
                cursor = CURSOR.Brush;
                EraserButton.IsChecked = false; // Выключить режим "Ластик"
                                                // Дополнительные действия для включения режима "Кисть"
            }
            else if (clickedButton == EraserButton && EraserButton.IsChecked == true)
            {
                // Включить режим "Ластик"
                cursor = CURSOR.Eraser;
                BrushButton.IsChecked = false; // Выключить режим "Кисть"
                                               // Дополнительные действия для включения режима "Ластик"
            }
            else
            {
                // Если кнопка отжата, выполните соответствующие действия для выключения выбранного режима
                cursor = CURSOR.Brush;
                BrushButton.IsChecked = true;
            }
        }
        #endregion // EO Drawing region

        // Visual cursor chagings region
        #region Visual cursor chages
        /// <summary>
        /// Changes cursor visualisation
        /// </summary>
        private void Canvas_MouseEnter(object sender, MouseEventArgs e)
        {
            drawCanvas.Cursor = Cursors.Pen;
        }
        /// <summary>
        /// Changes cursor visualisation
        /// </summary>
        private void Canvas_MouseLeave(object sender, MouseEventArgs e)
        {
            drawCanvas.Cursor = Cursors.Arrow;
        }
        #endregion // EO Visual cursor chages

        // Indirect impact on canvas region
        #region Indirect impact on canvas
        /// <summary>
        /// Copies data to the clipboard
        /// </summary>
        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            Save = GetCanvasInfo();
            Clipboard.SetData(DataFormats.Text, (Object)Save);
        }
        /// <summary>
        /// Inverts all pixels black to white and white to black
        /// </summary>
        private void InversionButton_Click(object sender, RoutedEventArgs e)
        {
            inversion = !inversion;
            if (inversion)
            {
                foreach (var child in drawCanvas.Children)
                {
                    if (child is Rectangle rectangle)
                    {
                        rectangle.Fill = Brushes.Black;
                        rectangle.Stroke = Brushes.Black;
                    }
                }
                drawCanvas.Background = Brushes.White;
            }
            else
            {
                foreach (var child in drawCanvas.Children)
                {
                    if (child is Rectangle rectangle)
                    {
                        rectangle.Fill = Brushes.White;
                        rectangle.Stroke = Brushes.White;
                    }
                }
                drawCanvas.Background = Brushes.Black;
            }
        }
        /// <summary>
        /// Clears field to zero
        /// </summary>
        private void DeleteAllButoon_Click(object sender, RoutedEventArgs e)
        {
            drawCanvas.Children.Clear();
        }
        #endregion // EO Indirect impact on canvas region

        // Resize canvas region
        #region Resize canvas
        /// <summary>
        /// Resizes canvas | Unites modules
        /// </summary>
        private void ResizeButton_Click(object sender, RoutedEventArgs e)
        {
            ResizeWindow inputWindow = new ResizeWindow(); // Создание экземпляра нового окна
            bool? result = inputWindow.ShowDialog(); // Отображение нового окна как диалогового

            if (result.HasValue && result.Value)
            {
                // Код, который будет выполнен при подтверждении данных
                CreateGridBySize(resizeWindow: true);
            }
        }
        /// <summary>
        /// Recreates save
        /// </summary>
        private void RecreateFromSave(string str)
        {
            if (drawCanvas != null)
            {
                drawCanvas.Children.Clear();
                int iter = 0;
                for (int i = 0; i < Rows; i++)
                {
                    for (int j = 0; j < Cols; j++)
                    {
                        if (str[iter] == '1')
                        {
                            Rectangle rectangle;

                            if (!inversion)
                                rectangle = new Rectangle
                                {
                                    Width = Size,
                                    Height = Size,
                                    Fill = Brushes.White,
                                    Stroke = Brushes.White,
                                    StrokeThickness = 1
                                };
                            else
                                rectangle = new Rectangle
                                {
                                    Width = Size,
                                    Height = Size,
                                    Fill = Brushes.Black,
                                    Stroke = Brushes.Black,
                                    StrokeThickness = 1
                                };

                            Canvas.SetLeft(rectangle, j * Size);
                            Canvas.SetTop(rectangle, i * Size);

                            drawCanvas.Children.Add(rectangle);
                        }
                        if ((iter + 1) < Rows * Cols) iter++;
                    }
                }
            }
        }
        /// <summary>
        /// Resize slider value changed
        /// </summary>
        private void SizeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Save = GetCanvasInfo();
            Size = (int)SizeSlider.Value;

            CreateGridBySize(sizeChange: true);
        }
        #endregion // EO Resize canvas region

        // ToolBar methods region
        #region ToolBar methods
        /// <summary>
        /// Drag and move
        /// </summary>
        private void ToolBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }
        /// <summary>
        /// Close button
        /// </summary>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        /// <summary>
        /// Minimization button
        /// </summary>
        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        /// <summary>
        /// Maximization button
        /// </summary>
        private void MaximizationButton_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
                MainGrid.Margin = new Thickness(0);
            }
            else
            { 
                WindowState = WindowState.Maximized;
                MainGrid.Margin = new Thickness(5, 5, 5, 50);
            }
        }
        #endregion // EO ToolBar methods

        #endregion // EO Methods of the P10Drawer
    }
}
