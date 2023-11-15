﻿using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
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
        private int Rows { get; } = 112;
        // Колличество строк
        private int Cols { get; } = 128;
        // Размер пикселя
        private int Size { get; } = 5;
        // Вулючена ли инверсия
        private bool inversion = false;
        // Состояние курсора
        private enum CURSOR
        {
            Brush,
            Eraser
        }
        private CURSOR cursor = CURSOR.Brush;

        public MainWindow()
        {
            InitializeComponent();
            CreateGridBySize(Cols, Rows);
        }

        public void CreateGridBySize(int cols, int rows)
        {
            for (int i = 0; i < cols; i++)
            {
                ColumnDefinition columnDefinition = new ColumnDefinition() { Width = new GridLength(Size, GridUnitType.Pixel) };
                SuperGrid.ColumnDefinitions.Add(columnDefinition);
            }

            for (int i = 0; i < rows; i++)
            {
                RowDefinition rowDefinition = new RowDefinition() { Height = new GridLength(Size, GridUnitType.Pixel) };
                SuperGrid.RowDefinitions.Add(rowDefinition);
            }
        }

        // Движение мышью в канвасе
        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {

                Point mousePosition = e.GetPosition(drawCanvas);

                if (mousePosition.X >= 0 && mousePosition.X <= drawCanvas.ActualWidth - 1 &&
                    mousePosition.Y >= 0 && mousePosition.Y <= drawCanvas.ActualHeight - 1)
                {
                    int column = (int)(mousePosition.X / (drawCanvas.ActualWidth / Cols));
                    int row = (int)(mousePosition.Y / (drawCanvas.ActualHeight / Rows));

                    Rectangle rectangle;

                    if (cursor == CURSOR.Brush)
                    {

                        if (!inversion)
                            rectangle = new Rectangle
                            {
                                Width = drawCanvas.ActualWidth / Cols,
                                Height = drawCanvas.ActualHeight / Rows,
                                Fill = Brushes.White,
                                Stroke = Brushes.White,
                                StrokeThickness = 1
                            };
                        else
                            rectangle = new Rectangle
                            {
                                Width = drawCanvas.ActualWidth / Cols,
                                Height = drawCanvas.ActualHeight / Rows,
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
            List<int> keys = new List<int>();

            foreach (Rectangle rec in drawCanvas.Children) 
            {
                int x = (int)Canvas.GetTop(rec) / Size;
                int y = (int)Canvas.GetLeft(rec) / Size;
                if (!keys.Contains((x * 1000) + y)) keys.Add((x * 1000) + y);
            }

            string answer = string.Empty;

            for (int i = 0; i < Cols; i++) 
            {
                for (int j = 0; j < Rows; j++) 
                {
                    if (keys.Contains((i * 1000) + j)) answer += "1";
                    else answer += "0";
                }
            }

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

        // ластик
        private void EraserButton_Click(object sender, RoutedEventArgs e)
        {
            cursor = CURSOR.Eraser;
        }
    }
}
