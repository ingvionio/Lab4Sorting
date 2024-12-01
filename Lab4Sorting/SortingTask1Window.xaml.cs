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
            delay = 500;
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
            log.AppendLine("Начало сортировки пузырьком.");
            for (int i = 0; i < n - 1; i++)
            {
                for (int j = 0; j < n - i - 1; j++)
                {
                    rectangles[j].Fill = compareColor;
                    rectangles[j + 1].Fill = compareColor;
                    Dispatcher.Invoke(() => UpdateRectangles(arr));

                    log.AppendLine($"Сравнение элементов {arr[j]} и {arr[j + 1]}.");

                    if (arr[j] > arr[j + 1])
                    {
                        log.AppendLine($"Элемент {arr[j]} больше, перестановка элементов {arr[j]} и {arr[j + 1]}.");
                        (arr[j], arr[j + 1]) = (arr[j + 1], arr[j]);

                        rectangles[j].Fill = swapColor;
                        rectangles[j + 1].Fill = swapColor;

                        await Task.Delay(delay);
                        Dispatcher.Invoke(() =>
                        {
                            UpdateRectangles(arr);
                            UpdateLogTextBox();
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
                            rectangles[j].Fill = defaultColor;
                            rectangles[j + 1].Fill = defaultColor;
                            UpdateRectangles(arr);
                        });
                    }
                }

                // Подсветка отсортированных элементов после каждого прохода
                for (int k = n - i - 1; k < n; k++)  // От i до конца массива
                {
                    rectangles[k].Fill = Brushes.Green; // Зеленый для отсортированных элементов
                }

                await Task.Delay(delay);
                Dispatcher.Invoke(() =>
                {
                    UpdateRectangles(arr);
                    UpdateLogTextBox();
                });
            }
            rectangles[0].Fill = Brushes.Green;
            log.AppendLine("Сортировка пузырьком завершена.");
        }


        private async Task SelectionSort(int[] arr)
        {
            int n = arr.Length;
            log.AppendLine("Начало сортировки выбором.");
            for (int i = 0; i < n - 1; i++)
            {
                int min_idx = i;
                rectangles[i].Fill = compareColor;
                Dispatcher.Invoke(() => UpdateRectangles(arr));

                for (int j = i + 1; j < n; j++)
                {
                    rectangles[j].Fill = compareColor;

                    log.AppendLine($"Сравнение текущего минимума {arr[min_idx]} и элемента {arr[j]}.");

                    Dispatcher.Invoke(() =>
                    {
                        UpdateRectangles(arr);
                        UpdateLogTextBox();
                    });

                    if (arr[j] < arr[min_idx])
                    {
                        rectangles[min_idx].Fill = defaultColor;
                        min_idx = j;
                        rectangles[min_idx].Fill = compareColor;
                        log.AppendLine($"Найден новый минимум {arr[min_idx]}.");

                    }
                    await Task.Delay(delay);
                    Dispatcher.Invoke(() =>
                    {
                        rectangles[j].Fill = defaultColor;
                        UpdateRectangles(arr);
                    });
                }

                // Переставляем минимальный элемент с текущей позиции
                (arr[i], arr[min_idx]) = (arr[min_idx], arr[i]);
                rectangles[min_idx].Fill = defaultColor;

                log.AppendLine($"Перестановка элемента {arr[min_idx]} с {arr[i]}.");

                // Отображаем отсортированную часть массива зеленым цветом
                for (int k = 0; k <= i; k++)
                {
                    UpdateRectangles(arr);
                    rectangles[k].Fill = Brushes.Green; // Зеленый для отсортированных элементов
                }

                await Task.Delay(delay);
                Dispatcher.Invoke(() =>
                {
                    UpdateRectangles(arr);
                    UpdateLogTextBox();
                });
            }
            rectangles[19].Fill = Brushes.Green;
            log.AppendLine("Сортировка выбором завершена.");
        }


        private async Task HeapSort(int[] arr)
        {
            int n = arr.Length;
            log.AppendLine("Начало пирамидальной сортировки.");

            // Строим кучу
            for (int i = n / 2 - 1; i >= 0; i--)
            {
                log.AppendLine($"Построение кучи: обработка узла с индексом {i}.");
                await Heapify(arr, n, i);
            }

            // Извлекаем элементы из кучи по одному
            for (int i = n - 1; i > 0; i--)
            {
                log.AppendLine($"Пирамидальная сортировка: извлечение максимального элемента {arr[0]} и перемещение в конец массива.");

                // Выделяем текущие элементы для обмена
                rectangles[0].Fill = swapColor;
                rectangles[i].Fill = swapColor;
                Dispatcher.Invoke(() => UpdateRectangles(arr));

                // Перестановка
                (arr[0], arr[i]) = (arr[i], arr[0]);

                await Task.Delay(delay);
                Dispatcher.Invoke(() =>
                {
                    UpdateRectangles(arr);
                    UpdateLogTextBox();
                    rectangles[0].Fill = defaultColor;
                    rectangles[i].Fill = defaultColor;
                });

                // Восстановление кучи для оставшегося массива
                log.AppendLine($"Пирамидальная сортировка: восстановление кучи для оставшейся части массива (длина {i}).");
                await Heapify(arr, i, 0);

                // Подсветка отсортированной части массива
                for (int k = n - 1; k > i; k--)
                {
                    rectangles[k].Fill = Brushes.Green; // Зеленый для отсортированных элементов
                }

                await Task.Delay(delay);
                Dispatcher.Invoke(() =>
                {
                    UpdateRectangles(arr);
                    UpdateLogTextBox();
                });
            }
            rectangles[1].Fill = Brushes.Green;
            rectangles[0].Fill = Brushes.Green;
            log.AppendLine("Пирамидальная сортировка завершена.");
        }

        private async Task Heapify(int[] arr, int n, int i)
        {
            int largest = i;  // Инициализируем наибольший элемент как корень
            int l = 2 * i + 1; // Левый дочерний элемент
            int r = 2 * i + 2; // Правый дочерний элемент

            // Если левый дочерний элемент больше корня
            if (l < n && arr[l] > arr[largest])
            {
                log.AppendLine($"Построение кучи: левый дочерний элемент {arr[l]} больше текущего узла {arr[largest]}.");
                largest = l;
            }

            // Если правый дочерний элемент больше, чем наибольший элемент на данный момент
            if (r < n && arr[r] > arr[largest])
            {
                log.AppendLine($"Построение кучи: правый дочерний элемент {arr[r]} больше текущего узла {arr[largest]}.");
                largest = r;
            }

            // Если самый большой элемент не корень
            if (largest != i)
            {
                log.AppendLine($"Построение кучи: перестановка узлов {arr[i]} и {arr[largest]}.");

                // Подсвечиваем переставляемые элементы
                rectangles[i].Fill = swapColor;
                rectangles[largest].Fill = swapColor;
                Dispatcher.Invoke(() => UpdateRectangles(arr));

                // Переставляем элементы
                (arr[i], arr[largest]) = (arr[largest], arr[i]);

                await Task.Delay(delay);
                Dispatcher.Invoke(() =>
                {
                    UpdateRectangles(arr);
                    UpdateLogTextBox();
                    rectangles[i].Fill = defaultColor;
                    rectangles[largest].Fill = defaultColor;
                });

                // Рекурсивно вызываем Heapify для поддерева
                await Heapify(arr, n, largest);
            }
        }


        private async Task QuickSort(int[] arr, int low, int high)
        {
            if (low < high)
            {
                log.AppendLine($"Быстрая сортировка: обработка подмассива от {low} до {high}.");

                // Подсветка всего массива перед сортировкой
                for (int i = low; i <= high; i++)
                {
                    rectangles[i].Fill = Brushes.LightYellow; // Подсвечиваем весь массив перед обработкой
                }

                int pi = await Partition(arr, low, high);
                log.AppendLine($"Быстрая сортировка: элемент {arr[pi]} установлен в правильную позицию (индекс {pi}).");

                // Рекурсивно сортируем левую и правую части массива
                await QuickSort(arr, low, pi - 1);
                await QuickSort(arr, pi + 1, high);
            }
        }

        private async Task<int> Partition(int[] arr, int low, int high)
        {
            int pivot = arr[high];
            log.AppendLine($"Быстрая сортировка: выбран опорный элемент {pivot}.");

            // Подсвечиваем опорный элемент синим
            rectangles[high].Fill = Brushes.Cyan;

            int i = low - 1;

            // Подсвечиваем левую и правую часть массива
            for (int j = low; j < high; j++)
            {
                // Подсвечиваем текущий элемент для сравнения
                rectangles[j].Fill = Brushes.Yellow;
                Dispatcher.Invoke(() => UpdateRectangles(arr));

                log.AppendLine($"Быстрая сортировка: сравнение элемента {arr[j]} с опорным {pivot}.");

                if (arr[j] < pivot)
                {
                    i++;
                    log.AppendLine($"Быстрая сортировка: перестановка {arr[i]} и {arr[j]}.");

                    // Переставляем элементы и подсвечиваем их красным
                    (arr[i], arr[j]) = (arr[j], arr[i]);
                    rectangles[i].Fill = Brushes.Red;
                    rectangles[j].Fill = Brushes.Red;

                    await Task.Delay(delay);
                    Dispatcher.Invoke(() =>
                    {
                        UpdateRectangles(arr);
                        UpdateLogTextBox();
                        rectangles[i].Fill = defaultColor; // Возвращаем цвет
                        rectangles[j].Fill = defaultColor; // Возвращаем цвет
                    });
                }
                else
                {
                    // Если элемент не меньше опорного, возвращаем его цвет в дефолтный
                    await Task.Delay(delay);
                    Dispatcher.Invoke(() =>
                    {
                        rectangles[j].Fill = defaultColor;
                        UpdateRectangles(arr);
                    });
                }
            }

            // Перестановка опорного элемента с элементом на позиции i+1
            log.AppendLine($"Быстрая сортировка: перестановка опорного элемента {pivot} с {arr[i + 1]}.");
            (arr[i + 1], arr[high]) = (arr[high], arr[i + 1]);
            rectangles[high].Fill = defaultColor;

            await Task.Delay(delay);
            Dispatcher.Invoke(() =>
            {
                UpdateRectangles(arr);
                UpdateLogTextBox();
            });

            // Возвращаем индекс опорного элемента
            return i + 1;
        }



        private void UpdateRectangles(int[] arr)
        {
            double canvasWidth = SortCanvas.ActualWidth;
            double rectWidth = canvasWidth / arr.Length;

            for (int i = 0; i < arr.Length; i++)
            {
                rectangles[i].Height = arr[i] * 20;
                Canvas.SetLeft(rectangles[i], i * rectWidth);
                Canvas.SetBottom(rectangles[i], 0);

                // Обновление текста над столбцами
                TextBlock textBlock = (TextBlock)SortCanvas.Children[i * 2 + 1];
                textBlock.Text = arr[i].ToString();
                Canvas.SetLeft(textBlock, i * rectWidth + rectWidth / 2 - textBlock.ActualWidth / 2);
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
