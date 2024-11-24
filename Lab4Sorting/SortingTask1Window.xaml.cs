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
            StartSorting.IsEnabled = false;
            Random rand = new Random();
            array = Enumerable.Range(1, arraySize).OrderBy(x => rand.Next()).ToArray();
            log.Clear();
            CreateRectangles(array);

            try
            {
                if (BubbleSortRadioButton.IsChecked == true)
                {
                    await BubbleSort(array);
                }
                else if (SelectionSortRadioButton.IsChecked == true)
                {
                    await SelectionSort(array);
                }
                else if (HeapSortRadioButton.IsChecked == true)
                {
                    await HeapSort(array);
                }
                else if (QuickSortRadioButton.IsChecked == true)
                {
                    await QuickSort(array, 0, array.Length - 1);
                }
            }
            finally
            {
                StartSorting.IsEnabled = true;
            }

            LogTextBox.Text = log.ToString();
        }

        private void SortingAlgorithm_Checked(object sender, RoutedEventArgs e)
        {
            // Обновляем описание алгоритма при выборе радиокнопки
            UpdateAlgorithmDescription();
        }

        private void UpdateAlgorithmDescription()
        {
            if (BubbleSortRadioButton.IsChecked == true)
            {
                AlgorithmDescriptionTextBlock.Text = "Сортировка пузырьком: Алгоритм проходит по массиву несколько раз. На каждом проходе сравниваются соседние элементы. Если они в неправильном порядке, то они меняются местами.  Таким образом, большие элементы \"всплывают\" к концу массива, как пузырьки.";

            }
            else if (SelectionSortRadioButton.IsChecked == true)
            {
                AlgorithmDescriptionTextBlock.Text = "Сортировка выбором: Алгоритм находит минимальный элемент в неотсортированной части массива и меняет его местами с первым элементом этой части. Затем процесс повторяется для оставшейся неотсортированной части.";
            }
            else if (HeapSortRadioButton.IsChecked == true)
            {
                AlgorithmDescriptionTextBlock.Text = "Пирамидальная сортировка (HeapSort):  Этот алгоритм использует структуру данных, называемую кучей (heap), для эффективной сортировки. Сначала массив преобразуется в кучу, а затем наибольший элемент извлекается из кучи и помещается в конец массива. Этот процесс повторяется до тех пор, пока весь массив не будет отсортирован.";
            }
            else if (QuickSortRadioButton.IsChecked == true)
            {
                AlgorithmDescriptionTextBlock.Text = "Быстрая сортировка (QuickSort):  Этот алгоритм основан на принципе \"разделяй и властвуй\". Он выбирает опорный элемент и переставляет элементы массива таким образом, чтобы все элементы меньше опорного находились слева от него, а все элементы больше - справа. Затем алгоритм рекурсивно сортирует левую и правую части массива.";
            }



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

        private async Task HeapSort(int[] arr)
        {
            int n = arr.Length;

            // Построение кучи (heapify)
            for (int i = n / 2 - 1; i >= 0; i--)
                await Heapify(arr, n, i);

            // Извлечение элементов из кучи по одному
            for (int i = n - 1; i > 0; i--)
            {
                // Выделяем переставляемые элементы
                rectangles[0].Fill = swapColor;
                rectangles[i].Fill = swapColor;
                Dispatcher.Invoke(() => UpdateRectangles(arr));

                (arr[0], arr[i]) = (arr[i], arr[0]);
                log.AppendLine($"Перестановка: {arr[0]} и {arr[i]}");

                await Task.Delay(delay);
                Dispatcher.Invoke(() =>
                {
                    UpdateRectangles(arr);
                    UpdateLogTextBox();

                    // Возвращаем цвет после перестановки
                    rectangles[0].Fill = defaultColor;
                    rectangles[i].Fill = defaultColor;
                    Dispatcher.Invoke(() => UpdateRectangles(arr)); // Сразу обновляем UI
                });

                // Вызываем heapify для уменьшенной кучи
                await Heapify(arr, i, 0);
            }
        }

        private async Task Heapify(int[] arr, int n, int i)
        {
            int largest = i; // Инициализируем наибольший элемент как корень
            int l = 2 * i + 1; // левый = 2*i + 1
            int r = 2 * i + 2; // правый = 2*i + 2


            // Если левый дочерний элемент больше корня
            if (l < n && arr[l] > arr[largest])
            {

                largest = l;
            }


            // Если правый дочерний элемент больше, чем самый большой элемент на данный момент
            if (r < n && arr[r] > arr[largest])
            {
                largest = r;
            }

            // Если самый большой элемент не корень
            if (largest != i)
            {

                // Выделяем переставляемые элементы
                rectangles[i].Fill = swapColor;
                rectangles[largest].Fill = swapColor;
                Dispatcher.Invoke(() => UpdateRectangles(arr));

                (arr[i], arr[largest]) = (arr[largest], arr[i]);
                log.AppendLine($"Перестановка: {arr[i]} и {arr[largest]}");

                await Task.Delay(delay);
                Dispatcher.Invoke(() =>
                {
                    UpdateRectangles(arr);
                    UpdateLogTextBox();

                    // Возвращаем цвет после перестановки
                    rectangles[i].Fill = defaultColor;
                    rectangles[largest].Fill = defaultColor;
                    Dispatcher.Invoke(() => UpdateRectangles(arr));
                });

                // Рекурсивно heapify затронутый поддерево
                await Heapify(arr, n, largest);
            }


        }



        private async Task QuickSort(int[] arr, int low, int high)
        {
            if (low < high)
            {


                int pi = await Partition(arr, low, high);

                await QuickSort(arr, low, pi - 1);
                await QuickSort(arr, pi + 1, high);

            }
        }



        private async Task<int> Partition(int[] arr, int low, int high)
        {
            int pivot = arr[high];
            rectangles[high].Fill = compareColor; // Выделяем опорный элемент
            int i = (low - 1); // индекс меньшего элемента

            for (int j = low; j < high; j++)
            {

                rectangles[j].Fill = compareColor;  // выделяем текущий элемент
                Dispatcher.Invoke(() => UpdateRectangles(arr));

                log.AppendLine($"Сравнение: {arr[j]} и {pivot}"); // Логируем сравнение

                if (arr[j] < pivot)
                {
                    i++;

                    (arr[i], arr[j]) = (arr[j], arr[i]);

                    log.AppendLine($"Перестановка: {arr[i]} и {arr[j]}");
                    rectangles[i].Fill = swapColor;
                    rectangles[j].Fill = swapColor;

                    await Task.Delay(delay);
                    Dispatcher.Invoke(() =>
                    {


                        UpdateRectangles(arr);

                        UpdateLogTextBox();
                        rectangles[i].Fill = defaultColor;  // Сбрасываем выделение
                        rectangles[j].Fill = defaultColor;  // Сбрасываем выделение
                        UpdateRectangles(arr);
                    });




                }
                else
                {

                    await Task.Delay(delay);
                    Dispatcher.Invoke(() =>
                    {

                        rectangles[j].Fill = defaultColor;  // Сбрасываем выделение

                        UpdateRectangles(arr);
                    });


                }
            }

            (arr[i + 1], arr[high]) = (arr[high], arr[i + 1]);
            rectangles[high].Fill = defaultColor;  // Сбрасываем выделение с опорного


            log.AppendLine($"Перестановка: {arr[i + 1]} и {arr[high]}");

            await Task.Delay(delay);
            Dispatcher.Invoke(() =>
            {


                UpdateRectangles(arr);

                UpdateLogTextBox();


            });




            return i + 1;



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
