using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using ClosedXML.Excel;

namespace Lab4Sorting
{
    public partial class SortingTask2 : Window
    {
        private StringBuilder log;
        private int delay;
        private SolidColorBrush defaultColor = Brushes.LightBlue;
        private List<Dictionary<string, string>> table;
        private string[] headers;

        public SortingTask2()
        {
            InitializeComponent();
            log = new StringBuilder();
            delay = 500; // Default delay in ms
        }

        private void LoadExcelButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Excel Files (*.xlsx)|*.xlsx|All Files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                using (var workbook = new XLWorkbook(openFileDialog.FileName))
                {
                    var worksheet = workbook.Worksheet(1);
                    headers = worksheet.FirstRowUsed()
                                       .CellsUsed()
                                       .Select(cell => cell.Value.ToString())
                                       .ToArray();

                    table = worksheet.RowsUsed()
                                     .Skip(1)
                                     .Select(row => headers.Zip(row.CellsUsed().Select(cell => cell.Value.ToString()),
                                                                (header, value) => new { header, value })
                                                            .ToDictionary(x => x.header, x => x.value))
                                     .ToList();
                }

                ExcelColumnComboBox.ItemsSource = headers;
                Log("Excel-файл загружен. Выберите колонку для сортировки.");
            }
        }

        private async void StartSorting_Click(object sender, RoutedEventArgs e)
        {
            if (table == null || headers == null || ExcelColumnComboBox.SelectedItem == null)
            {
                Log("Не выбран файл или колонка для сортировки.");
                return;
            }

            string sortKey = ExcelColumnComboBox.SelectedItem.ToString();
            Log($"Сортировка по колонке: {sortKey}");

            if (DirectMergeSortRadioButton.IsChecked == true)
            {
                await DirectMergeSort(table, sortKey);
            }
            else if (NaturalMergeSortRadioButton.IsChecked == true)
            {
                Log("Естественное слияние пока не реализовано.");
            }
            else if (MultiwayMergeSortRadioButton.IsChecked == true)
            {
                Log("Многопутевое слияние пока не реализовано.");
            }

            Log("Сортировка завершена.");
        }

        private void DelaySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            delay = (int)e.NewValue;
            DelayLabel.Content = $"Задержка: {delay} мс"; // Обновляем Label с задержкой
        }

        private async Task DirectMergeSort(List<Dictionary<string, string>> table, string sortKey)
        {
            int n = table.Count;
            int seriesLength = 1;
            int step = 1; // Номер шага сортировки

            while (seriesLength < n)
            {
                Log($"Шаг {step}: Длина цепочки = {seriesLength}");

                // 1. Разделение исходного массива на два вспомогательных
                var fileB = new List<Dictionary<string, string>>();
                var fileC = new List<Dictionary<string, string>>();

                int i = 0;
                while (i < n)
                {
                    for (int j = 0; j < seriesLength && i < n; j++, i++)
                        fileB.Add(table[i]);

                    for (int j = 0; j < seriesLength && i < n; j++, i++)
                        fileC.Add(table[i]);
                }

                Log($"Формирование файлов B и C: \nB: {string.Join(", ", fileB.Select(row => row[sortKey]))} \nC: {string.Join(", ", fileC.Select(row => row[sortKey]))}");
                await VisualizeBlocksWithLabels(new List<List<Dictionary<string, string>>> { fileB, fileC }, sortKey, "B", "C", seriesLength);

                // 2. Слияние вспомогательных файлов обратно в основной массив
                var merged = new List<Dictionary<string, string>>();
                int bIndex = 0, cIndex = 0;

                while (bIndex < fileB.Count || cIndex < fileC.Count)
                {
                    int bCount = 0, cCount = 0;

                    while ((bCount < seriesLength && bIndex < fileB.Count) ||
       (cCount < seriesLength && cIndex < fileC.Count))
                    {
                        await HighlightComparison(fileB, fileC, bIndex, cIndex, sortKey, seriesLength);

                        if (bCount < seriesLength && bIndex < fileB.Count &&
                            (cCount >= seriesLength || cIndex >= fileC.Count || CompareValues(fileB[bIndex][sortKey], fileC[cIndex][sortKey]) <= 0))
                        {
                            if (cIndex < fileC.Count)
                                Log($"Сравнение: {fileB[bIndex][sortKey]} (из B) < {fileC[cIndex][sortKey]} (из C): Берём {fileB[bIndex][sortKey]}");
                            else
                                Log($"C пусто, берём из B {fileB[bIndex][sortKey]}");
                            merged.Add(fileB[bIndex]);
                            bIndex++;
                            bCount++;
                        }
                        else if (cCount < seriesLength && cIndex < fileC.Count)
                        {
                            if (fileC.Count != 0)
                                Log($"Сравнение: {fileC[cIndex][sortKey]} (из C) <= {fileB[bIndex][sortKey]} (из B): Берём {fileC[cIndex][sortKey]}");
                            else 
                                Log("C пусто, берём из B {fileB[bIndex][sortKey]}");
                            merged.Add(fileC[cIndex]);
                            cIndex++;
                            cCount++;
                        }

                        await VisualizeMerged(merged, sortKey, "A");
                    }

                }

                table.Clear();
                table.AddRange(merged);

                Log($"После слияния: {string.Join(", ", table.Select(row => row[sortKey]))}");
                await VisualizeBlocksWithLabels(new List<List<Dictionary<string, string>>> { table }, sortKey, "A", "", seriesLength);

                // 3. Увеличение длины серии и номера шага
                seriesLength *= 2;
                step++;
            }
        }

        private async Task HighlightComparison(
     List<Dictionary<string, string>> fileB,
     List<Dictionary<string, string>> fileC,
     int bIndex,
     int cIndex,
     string sortKey,
     int seriesLength)
        {
            // Создаем копии списков для визуализации
            var blockB = new List<Dictionary<string, string>>(fileB);
            var blockC = new List<Dictionary<string, string>>(fileC);

            // Добавляем null, если индексы выходят за пределы списка
            var highlightedB = bIndex < blockB.Count ? blockB[bIndex] : null;
            var highlightedC = cIndex < blockC.Count ? blockC[cIndex] : null;

            // Визуализируем блоки B и C, выделяя элементы
            await VisualizeBlocksWithHighlights(new List<List<Dictionary<string, string>>>
    {
        blockB,
        blockC
    }, sortKey, "B", "C", seriesLength, highlightedB, highlightedC);
        }

        private async Task VisualizeBlocksWithHighlights(
    List<List<Dictionary<string, string>>> blocks,
    string sortKey,
    string labelA,
    string labelB,
    int seriesLength,
    Dictionary<string, string> highlightedB,
    Dictionary<string, string> highlightedC)
        {
            SortCanvas.Children.Clear();
            double canvasWidth = SortCanvas.ActualWidth;
            double blockWidth = canvasWidth / blocks.Count;

            for (int i = 0; i < blocks.Count; i++)
            {
                var block = blocks[i];
                double rectHeight = block.Count * 20;

                var rect = new Rectangle
                {
                    Width = blockWidth - 10,
                    Height = rectHeight,
                    Fill = defaultColor,
                    Stroke = Brushes.Black,
                    StrokeThickness = 1
                };

                Canvas.SetLeft(rect, i * blockWidth);
                Canvas.SetTop(rect, SortCanvas.ActualHeight - rectHeight - 20);
                SortCanvas.Children.Add(rect);

                // Подпись для блоков
                var blockLabel = new TextBlock
                {
                    Text = (i == 0 ? labelA : labelB),
                    Foreground = Brushes.Black,
                    FontSize = 14,
                    FontWeight = FontWeights.Bold,
                    TextAlignment = TextAlignment.Center
                };
                Canvas.SetLeft(blockLabel, i * blockWidth + blockWidth / 2 - 30);
                Canvas.SetTop(blockLabel, SortCanvas.ActualHeight - rectHeight - 40);
                SortCanvas.Children.Add(blockLabel);

                for (int j = 0; j < block.Count; j++)
                {
                    if (block[j] == null) continue;

                    string key = block[j][headers[0]]; // Название строки (значение первого столбца)
                    string value = block[j][sortKey]; // Значение для сортировки

                    // Проверяем, является ли элемент выделенным
                    bool isHighlighted = block[j] == highlightedB || block[j] == highlightedC;

                    var label = new TextBlock
                    {
                        Text = $"{key} ({value})",
                        Foreground = isHighlighted ? Brushes.Red : Brushes.Black,
                        Background = Brushes.White,
                        TextAlignment = TextAlignment.Center,
                        Width = blockWidth - 10,
                        Height = 20
                    };

                    Canvas.SetLeft(label, i * blockWidth);
                    Canvas.SetTop(label, SortCanvas.ActualHeight - rectHeight + j * 20 - 20);
                    SortCanvas.Children.Add(label);
                }
            }

            await Task.Delay(delay);
        }




        private async Task VisualizeMerged(List<Dictionary<string, string>> merged, string sortKey, string label)
        {
            await VisualizeBlocksWithLabels(new List<List<Dictionary<string, string>>> { merged }, sortKey, label, "", seriesLength: merged.Count);
        }

        private async Task VisualizeBlocksWithLabels(
            List<List<Dictionary<string, string>>> blocks,
            string sortKey,
            string labelA,
            string labelB,
            int seriesLength,
            bool highlight = false)
        {
            SortCanvas.Children.Clear();
            double canvasWidth = SortCanvas.ActualWidth;
            double blockWidth = canvasWidth / blocks.Count;

            for (int i = 0; i < blocks.Count; i++)
            {
                var block = blocks[i];
                double rectHeight = block.Count * 20;

                var rect = new Rectangle
                {
                    Width = blockWidth - 10,
                    Height = rectHeight,
                    Fill = highlight ? Brushes.LightGreen : defaultColor,
                    Stroke = Brushes.Black,
                    StrokeThickness = 1
                };

                Canvas.SetLeft(rect, i * blockWidth);
                Canvas.SetTop(rect, SortCanvas.ActualHeight - rectHeight - 20);
                SortCanvas.Children.Add(rect);

                // Подпись для блоков
                var blockLabel = new TextBlock
                {
                    Text = (i == 0 ? labelA : labelB),
                    Foreground = Brushes.Black,
                    FontSize = 14,
                    FontWeight = FontWeights.Bold,
                    TextAlignment = TextAlignment.Center
                };
                Canvas.SetLeft(blockLabel, i * blockWidth + blockWidth / 2 - 30);
                Canvas.SetTop(blockLabel, SortCanvas.ActualHeight - rectHeight - 40);
                SortCanvas.Children.Add(blockLabel);

                for (int j = 0; j < block.Count; j++)
                {
                    if (block[j] == null) continue;

                    string key = block[j][headers[0]]; // Название строки (значение первого столбца)
                    string value = block[j][sortKey]; // Значение для сортировки

                    var label = new TextBlock
                    {
                        Text = $"{key} ({value})",
                        Foreground = highlight ? Brushes.Red : Brushes.Black,
                        Background = Brushes.White,
                        TextAlignment = TextAlignment.Center,
                        Width = blockWidth - 10,
                        Height = 20
                    };

                    Canvas.SetLeft(label, i * blockWidth);
                    Canvas.SetTop(label, SortCanvas.ActualHeight - rectHeight + j * 20 - 20);
                    SortCanvas.Children.Add(label);
                }
            }

            await Task.Delay(delay);
        }

        private int CompareValues(string value1, string value2)
        {
            if (double.TryParse(value1, out var num1) && double.TryParse(value2, out var num2))
            {
                return num1.CompareTo(num2);
            }
            return string.Compare(value1, value2, StringComparison.Ordinal);
        }

        private void Log(string message)
        {
            log.AppendLine(message);
            LogTextBox.Text = log.ToString();
            LogTextBox.ScrollToEnd();
        }
    }
}
