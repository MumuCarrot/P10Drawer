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
        // Колличество рядов
        public static int Rows { get; set; } = 16;
        // Колличество строк
        public static int Cols { get; set; } = 32;
        // Размер пикселя
        private int Size { get; set; } = 5;
        // Включена ли инверсия
        private bool inversion = false;
        // Состояние курсора
        private enum CURSOR
        {
            Brush,
            Eraser
        }
        private CURSOR cursor = CURSOR.Brush;

        // Переменная для сохранения месседжа
        private string Save { get; set; } = string.Empty;

        public MainWindow()
        {
            InitializeComponent();
        }

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

        // Движение мышью в канвасе
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

        // захват курсора (вход)
        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            drawCanvas.CaptureMouse();
        }

        //захват курсора (выход)
        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            drawCanvas.ReleaseMouseCapture();
        }

        // смена курсора (вход)
        private void Canvas_MouseEnter(object sender, MouseEventArgs e)
        {
            drawCanvas.Cursor = Cursors.Pen;
        }

        // смена курсора (выход)
        private void Canvas_MouseLeave(object sender, MouseEventArgs e)
        {
            drawCanvas.Cursor = Cursors.Arrow;
        }

        // отправка данных
        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            Save = GetCanvasInfo();
            Clipboard.SetData(DataFormats.Text, (Object)Save);
        }

        // информаци про текущее состояние объектов на канвасе
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

        // инверсия цветов
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

        // удаление всего
        private void DeleteAllButoon_Click(object sender, RoutedEventArgs e)
        {
            drawCanvas.Children.Clear();
        }

        // состояние мыши
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

        // ресайз канваса
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

        private void SizeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Save = GetCanvasInfo();
            Size = (int)SizeSlider.Value;

            CreateGridBySize(sizeChange: true);
        }

        private void ToolBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void MaxemizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized) WindowState = WindowState.Normal;
            else WindowState = WindowState.Maximized;
        }
    }
}
