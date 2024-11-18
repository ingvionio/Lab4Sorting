using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Lab4Sorting
{
    public partial class SortingTask1Window : Window
    {
        private int[] array;
        private StringBuilder log;
        private int delay;
        private Rectangle[] rectangles;
        private int arraySize = 20;
        private SolidColorBrush defaultColor = Brushes.LightBlue;
        private SolidColorBrush compareColor = Brushes.Yellow;
        private SolidColorBrush swapColor = Brushes.Red;


        public SortingTask1Window()
        {
            InitializeComponent();
            log = new StringBuilder();
            delay = 100;
        }

        private async void StartSorting_Click(object sender, RoutedEventArgs e)
        {
            Random rand = new Random();
            array = Enumerable.Range(1, arraySize).OrderBy(x => rand.Next()).ToArray();
            log.Clear();
            CreateRectangles(array);

            if (BubbleSortRadioButton.IsChecked == true)
            {
                await BubbleSort(array);
            }
            else if (SelectionSortRadioButton.IsChecked == true)
            {
                await SelectionSort(array);
            }

            LogTextBox.Text = log.ToString();
        }


        private void CreateRectangles(int[] arr)
        {
            SortCanvas.Children.Clear();
            rectangles = new Rectangle[arr.Length];

            double canvasWidth = SortCanvas.ActualWidth;
            double rectWidth = canvasWidth / arr.Length;

            for (int i = 0; i < arr.Length; i++)
            {
                rectangles[i] = new Rectangle
                {
                    Width = rectWidth,
                    Height = arr[i] * 20, // Масштабируем высоту (можно настроить)
                    Fill = Brushes.LightBlue,
                    Stroke = Brushes.Black,
                    StrokeThickness = 1
                };

                Canvas.SetLeft(rectangles[i], i * rectWidth);
                Canvas.SetBottom(rectangles[i], 0);
                SortCanvas.Children.Add(rectangles[i]);


                // Добавляем номер столбца
                TextBlock textBlock = new TextBlock
                {
                    Text = (i + 1).ToString(),  // Номер столбца (начиная с 1)
                    Foreground = Brushes.Black,
                    FontSize = 10, // Размер шрифта
                    TextAlignment = TextAlignment.Center
                };
                Canvas.SetLeft(textBlock, i * rectWidth + rectWidth / 2 - textBlock.ActualWidth / 2);
                Canvas.SetBottom(textBlock, rectangles[i].Height + 2);
                SortCanvas.Children.Add(textBlock);

            }
        }


        private async Task BubbleSort(int[] arr)
        {
            int n = arr.Length;
            for (int i = 0; i < n - 1; i++)
            {
                for (int j = 0; j < n - i - 1; j++)
                {
                    // Выделяем сравниваемые столбцы
                    rectangles[j].Fill = compareColor;
                    rectangles[j + 1].Fill = compareColor;
                    Dispatcher.Invoke(() => UpdateRectangles(arr)); // Обновляем UI, чтобы увидеть выделение


                    log.AppendLine($"Сравнение: {arr[j]} и {arr[j + 1]}");


                    if (arr[j] > arr[j + 1])
                    {
                        log.AppendLine($"Перестановка: {arr[j]} и {arr[j + 1]}");
                        (arr[j], arr[j + 1]) = (arr[j + 1], arr[j]);


                        // Выделяем переставляемые столбцы
                        rectangles[j].Fill = swapColor;
                        rectangles[j + 1].Fill = swapColor;


                        await Task.Delay(delay);
                        Dispatcher.Invoke(() =>
                        {
                            UpdateRectangles(arr);
                            UpdateLogTextBox();

                            // Возвращаем цвет к дефолтному после перестановки
                            rectangles[j].Fill = defaultColor;
                            rectangles[j + 1].Fill = defaultColor;
                            UpdateRectangles(arr);
                        });

                    }
                    else
                    {

                        await Task.Delay(delay);
                        Dispatcher.Invoke(() =>
                        {
                            // Возвращаем цвет после сравнения, если не было перестановки
                            rectangles[j].Fill = defaultColor;
                            rectangles[j + 1].Fill = defaultColor;
                            UpdateRectangles(arr);
                        });
                    }

                }
            }
        }

        private async Task SelectionSort(int[] arr)
        {
            int n = arr.Length;
            for (int i = 0; i < n - 1; i++)
            {
                int min_idx = i;
                rectangles[i].Fill = compareColor; // Выделяем текущий минимальный элемент
                Dispatcher.Invoke(() => UpdateRectangles(arr));
                for (int j = i + 1; j < n; j++)
                {

                    rectangles[j].Fill = compareColor;// Выделяем сравниваемый элемент

                    log.AppendLine($"Сравнение: {arr[j]} и {arr[min_idx]}");


                    Dispatcher.Invoke(() => UpdateRectangles(arr)); // Обновляем UI, чтобы увидеть выделение

                    if (arr[j] < arr[min_idx])
                    {
                        rectangles[min_idx].Fill = defaultColor; // Сбрасываем выделение с предыдущего минимального

                        min_idx = j;
                        rectangles[min_idx].Fill = compareColor; // Выделяем новый минимальный


                    }
                    await Task.Delay(delay);
                    Dispatcher.Invoke(() =>
                    {
                        rectangles[j].Fill = defaultColor; // Сбрасываем выделение после сравнения

                        UpdateRectangles(arr);
                    });



                }


                (arr[i], arr[min_idx]) = (arr[min_idx], arr[i]);
                rectangles[min_idx].Fill = defaultColor;  // Возвращаем цвет после перестановки

                log.AppendLine($"Перестановка: {arr[i]} и {arr[min_idx]}");

                await Task.Delay(delay);
                Dispatcher.Invoke(() =>
                {

                    UpdateRectangles(arr);
                    UpdateLogTextBox();


                });
            }

        }

        private void UpdateRectangles(int[] arr)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                rectangles[i].Height = arr[i] * 20; // Обновляем высоту
                // Обновляем позицию текста
                TextBlock textBlock = (TextBlock)SortCanvas.Children[i * 2 + 1]; // Находим TextBlock (нечетные индексы)
                Canvas.SetLeft(textBlock, i * (SortCanvas.ActualWidth / arr.Length) + (SortCanvas.ActualWidth / arr.Length) / 2 - textBlock.ActualWidth / 2);
                Canvas.SetBottom(textBlock, rectangles[i].Height + 2);
            }
        }

        private void UpdateLogTextBox()
        {
            LogTextBox.Text = log.ToString();
            LogTextBox.ScrollToEnd(); // Прокрутка к концу
        }



        private void DelaySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            delay = (int)e.NewValue;
            DelayLabel.Content = $"Задержка: {delay} мс"; // Обновляем Label с задержкой
        }
    }
}
