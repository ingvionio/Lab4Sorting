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

namespace Lab4Sorting
{
    /// <summary>
    /// Логика взаимодействия для SortingTask2.xaml
    /// </summary>
    public partial class SortingTask2 : Window
    {
        private int[] array;
        private StringBuilder log;
        private int delay;
        private Rectangle[] rectangles;
        private int arraySize = 20;
        private SolidColorBrush defaultColor = Brushes.LightBlue;
        private SolidColorBrush compareColor = Brushes.Yellow;
        private SolidColorBrush swapColor = Brushes.Red;


        public SortingTask2()
        {
            InitializeComponent();
            // AlgorithmDescription.Text = "Выберите алгоритм сортировки";
            log = new StringBuilder();
            delay = 100;
        }

        private async void StartSorting_Click(object sender, RoutedEventArgs e)
        {
            Random rand = new Random();
            array = Enumerable.Range(1, arraySize).OrderBy(x => rand.Next()).ToArray();
            log.Clear();
            CreateRectangles(array);

            if (DirectMergeSortRadioButton.IsChecked == true)
            {
                await DirectMergeSort(array.ToList(), blockSize: 5);
            }
            else if (NaturalMergeSortRadioButton.IsChecked == true)
            {
                await NaturalMergeSort(array.ToList());
            }
            else if (MultiwayMergeSortRadioButton.IsChecked == true)
            {
                await MultiwayMergeSort(array.ToList(), numberOfWays: 4);
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

        public class DataBlock
        {
            public int[] Values { get; set; }
            public SolidColorBrush BlockColor { get; set; } = Brushes.LightBlue;
        }


        private void CreateBlocks(List<DataBlock> blocks)
        {
            SortCanvas.Children.Clear();

            double canvasWidth = SortCanvas.ActualWidth;
            double blockWidth = canvasWidth / blocks.Count;

            for (int i = 0; i < blocks.Count; i++)
            {
                var block = blocks[i];
                var rect = new Rectangle
                {
                    Width = blockWidth - 10,
                    Height = block.Values.Length * 20,
                    Fill = block.BlockColor,
                    Stroke = Brushes.Black,
                    StrokeThickness = 1
                };

                Canvas.SetLeft(rect, i * blockWidth);
                Canvas.SetBottom(rect, 0);
                SortCanvas.Children.Add(rect);
            }
        }


        private async Task DirectMergeSort(List<int> array, int blockSize)
        {
            List<List<int>> blocks = SplitIntoBlocks(array, blockSize);
            foreach (var block in blocks)
            {
                block.Sort(); // Внутренняя сортировка каждого блока
            }

            log.AppendLine("Блоки после сортировки:");
            foreach (var block in blocks)
            {
                log.AppendLine(string.Join(", ", block));
            }
            await VisualizeBlocks(blocks);

            while (blocks.Count > 1)
            {
                List<int> merged = MergeTwoRuns(blocks[0], blocks[1]);
                blocks.RemoveAt(0);
                blocks[0] = merged;

                await VisualizeBlocks(blocks);
                log.AppendLine("Слияние блоков:");
                log.AppendLine(string.Join(", ", blocks[0]));
            }
            array.Clear();
            array.AddRange(blocks[0]);
        }

        private List<List<int>> SplitIntoBlocks(List<int> array, int blockSize)
        {
            List<List<int>> blocks = new();
            for (int i = 0; i < array.Count; i += blockSize)
            {
                blocks.Add(array.Skip(i).Take(blockSize).ToList());
            }
            return blocks;
        }



        private List<int> MergeTwoRuns(List<int> left, List<int> right)
        {
            List<int> merged = new();
            int i = 0, j = 0;
            while (i < left.Count && j < right.Count)
            {
                if (left[i] <= right[j])
                    merged.Add(left[i++]);
                else
                    merged.Add(right[j++]);
            }
            merged.AddRange(left.Skip(i));
            merged.AddRange(right.Skip(j));
            return merged;
        }


        private async Task NaturalMergeSort(List<int> array)
        {
            List<List<int>> runs = IdentifyNaturalRuns(array);

            log.AppendLine("Найденные подпоследовательности:");
            foreach (var run in runs)
            {
                log.AppendLine(string.Join(", ", run));
            }
            await VisualizeBlocks(runs);

            while (runs.Count > 1)
            {
                List<int> merged = MergeTwoRuns(runs[0], runs[1]);
                runs.RemoveAt(0);
                runs[0] = merged;

                await VisualizeBlocks(runs);
                log.AppendLine("Слияние подпоследовательностей:");
                log.AppendLine(string.Join(", ", runs[0]));
            }
            array.Clear();
            array.AddRange(runs[0]);
        }

        private List<List<int>> IdentifyNaturalRuns(List<int> array)
        {
            List<List<int>> runs = new();
            List<int> currentRun = new() { array[0] };

            for (int i = 1; i < array.Count; i++)
            {
                if (array[i] >= array[i - 1])
                {
                    currentRun.Add(array[i]);
                }
                else
                {
                    runs.Add(currentRun);
                    currentRun = new List<int> { array[i] };
                }
            }
            runs.Add(currentRun);
            return runs;
        }

        private async Task MultiwayMergeSort(List<int> array, int numberOfWays)
        {
            List<List<int>> blocks = SplitIntoBlocks(array, array.Count / numberOfWays);
            foreach (var block in blocks)
            {
                block.Sort();
            }

            log.AppendLine("Сортированные блоки:");
            foreach (var block in blocks)
            {
                log.AppendLine(string.Join(", ", block));
            }
            await VisualizeBlocks(blocks);

            SortedDictionary<int, int> priorityQueue = new();
            List<int> pointers = new(blocks.Count);

            for (int i = 0; i < blocks.Count; i++)
            {
                if (blocks[i].Count > 0)
                {
                    priorityQueue[blocks[i][0]] = i;
                    pointers.Add(1);
                }
            }

            List<int> sorted = new();
            while (priorityQueue.Count > 0)
            {
                var min = priorityQueue.First();
                priorityQueue.Remove(min.Key);

                sorted.Add(min.Key);

                int blockIndex = min.Value;
                if (pointers[blockIndex] < blocks[blockIndex].Count)
                {
                    int nextValue = blocks[blockIndex][pointers[blockIndex]];
                    pointers[blockIndex]++;
                    priorityQueue[nextValue] = blockIndex;
                }

                await VisualizeArray(sorted);
            }
            array.Clear();
            array.AddRange(sorted);
        }


        private async Task VisualizeBlocks(List<List<int>> blocks)
        {
            List<DataBlock> dataBlocks = blocks.Select(block => new DataBlock
            {
                Values = block.ToArray(),
                BlockColor = defaultColor
            }).ToList();
            Dispatcher.Invoke(() => CreateBlocks(dataBlocks));
            await Task.Delay(delay);
        }

        private async Task VisualizeArray(List<int> array)
        {
            Dispatcher.Invoke(() => UpdateRectangles(array.ToArray()));
            await Task.Delay(delay);
        }


    }
}

