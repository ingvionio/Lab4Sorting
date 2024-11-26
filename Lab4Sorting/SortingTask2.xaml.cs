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
                await NaturalMergeSort(table, sortKey);

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

        private async Task NaturalMergeSort(List<Dictionary<string, string>> table, string sortKey)
        {
            int n = table.Count;

            while (true)
            {
                // Шаг 1: Разделение на естественные серии
                var fileB = new List<Dictionary<string, string>>();
                var fileC = new List<Dictionary<string, string>>();
                bool writeToB = true; // Чередуем запись между B и C

                for (int i = 0; i < n;)
                {
                    var series = new List<Dictionary<string, string>>();
                    series.Add(table[i++]); // Начинаем серию с текущего элемента

                    // Находим серию максимальной длины
                    while (i < n && CompareValues(table[i - 1][sortKey], table[i][sortKey]) <= 0)
                    {
                        series.Add(table[i++]);
                    }

                    // Записываем серию либо в B, либо в C
                    if (writeToB)
                        fileB.AddRange(series);
                    else
                        fileC.AddRange(series);

                    writeToB = !writeToB; // Чередуем файл
                }

                Log($"Файлы после разделения:\nB: {string.Join(", ", fileB.Select(row => row[sortKey]))}\nC: {string.Join(", ", fileC.Select(row => row[sortKey]))}");
                await VisualizeBlocksWithLabels(new List<List<Dictionary<string, string>>> { fileB, fileC }, sortKey, "B", "C", 0);

                // Если файл C пуст, значит сортировка завершена
                if (fileC.Count == 0)
                {
                    Log("Сортировка завершена.");
                    return;
                }

                // Шаг 2: Слияние серий
                var merged = new List<Dictionary<string, string>>();
                int bIndex = 0, cIndex = 0;

                while (bIndex < fileB.Count || cIndex < fileC.Count)
                {
                    // Сливаем одну серию из B и одну серию из C
                    var seriesB = GetSeries(fileB, ref bIndex, sortKey);
                    var seriesC = GetSeries(fileC, ref cIndex, sortKey);
                    var mergedSeries = MergeSeries(seriesB, seriesC, sortKey);

                    merged.AddRange(mergedSeries);
                    await VisualizeMerged(merged, sortKey, "A");
                }

                table.Clear();
                table.AddRange(merged);

                Log($"После слияния: {string.Join(", ", table.Select(row => row[sortKey]))}");
                await VisualizeBlocksWithLabels(new List<List<Dictionary<string, string>>> { table }, sortKey, "A", "", table.Count);


            }
        }

        // Метод для извлечения одной серии из файла
        private List<Dictionary<string, string>> GetSeries(List<Dictionary<string, string>> file, ref int index, string sortKey)
        {
            var series = new List<Dictionary<string, string>>();

            // Проверяем, что индекс не выходит за пределы
            if (index >= file.Count)
            {
                return series; // Возвращаем пустую серию, если индекс вне диапазона
            }

            series.Add(file[index++]); // Начинаем серию с текущего элемента

            // Пока не достигнут конец и значения возрастают, продолжаем добавлять элементы в серию
            while (index < file.Count && CompareValues(file[index - 1][sortKey], file[index][sortKey]) <= 0)
            {
                series.Add(file[index++]);
            }

            return series;
        }


        // Метод для слияния двух серий
        private List<Dictionary<string, string>> MergeSeries(
            List<Dictionary<string, string>> seriesB,
            List<Dictionary<string, string>> seriesC,
            string sortKey)
        {
            var merged = new List<Dictionary<string, string>>();
            int bIndex = 0, cIndex = 0;

            while (bIndex < seriesB.Count || cIndex < seriesC.Count)
            {
                if (bIndex < seriesB.Count && (cIndex >= seriesC.Count || CompareValues(seriesB[bIndex][sortKey], seriesC[cIndex][sortKey]) <= 0))
                {
                    merged.Add(seriesB[bIndex++]);
                }
                else if (cIndex < seriesC.Count)
                {
                    merged.Add(seriesC[cIndex++]);
                }
            }

            return merged;
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

                Log("Файл формируются следующим образом: идем по файлу A и по очерёдно записываем в файл B и С цепочки текущей длины");
                Log($"Формирование файлов B и C: \nB: {string.Join(", ", fileB.Select(row => row[sortKey]))} \nC: {string.Join(", ", fileC.Select(row => row[sortKey]))}");
                await VisualizeBlocksWithLabels(new List<List<Dictionary<string, string>>> { fileB, fileC }, sortKey, "B", "C", seriesLength);

                // 2. Слияние вспомогательных файлов обратно в основной массив
                var merged = new List<Dictionary<string, string>>();
                int bIndex = 0, cIndex = 0;

                while (bIndex < fileB.Count || cIndex < fileC.Count)
                {
                    int bCount = 0, cCount = 0;

                    while ((bCount < seriesLength && bIndex < fileB.Count) || (cCount < seriesLength && cIndex < fileC.Count))
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

        private void DirectMergeSortRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            LogTextBox_Algorithm.Text = "Алгоритм сортировки простым слияния является простейшим алгоритмом внешней сортировки, основанный на процедуре слияния серией.\r\n\r\nВ данном алгоритме длина серий фиксируется на каждом шаге. В исходном файле все серии имеют длину 1, после первого шага она равна 2, после второго – 4, после третьего – 8, после k -го шага – 2k.\r\n\r\nАлгоритм сортировки простым слиянием\r\n\r\nШаг 1. Исходный файл A разбивается на два вспомогательных файла B и C.\r\n\r\nШаг 2. Вспомогательные файлы B и C сливаются в файл A, при этом одиночные элементы образуют упорядоченные пары.\r\n\r\nШаг 3. Полученный файл A вновь обрабатывается, как указано в шагах 1 и 2. При этом упорядоченные пары переходят в упорядоченные четверки.\r\n\r\nШаг 4. Повторяя шаги, сливаем четверки в восьмерки и т.д., каждый раз удваивая длину слитых последовательностей до тех пор, пока не будет упорядочен целиком весь файл.\r\n\r\nПосле выполнения i проходов получаем два файла, состоящих из серий длины 2i. Окончание процесса происходит при выполнении условия 2i>=n. Следовательно, процесс сортировки простым слиянием требует порядка O(log n) проходов по данным.\r\n\r\nПризнаками конца сортировки простым слиянием являются следующие условия:\r\n\r\nдлина серии не меньше количества элементов в файле (определяется после фазы слияния);\r\nколичество серий равно 1 (определяется на фазе слияния).\r\nпри однофазной сортировке второй по счету вспомогательный файл после распределения серий остался пустым.";
        }

        private void NaturalMergeSortRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            LogTextBox_Algorithm.Text = "Алгоритм сортировки естественным слиянием\r\n\r\nШаг 1. Исходный файл A разбивается на два вспомогательных файла B и C. Распределение происходит следующим образом: поочередно считываются записи ai исходной последовательности (неупорядоченной) таким образом, что если значения ключей соседних записей удовлетворяют условию A(ai)<=A(ai+1), то они записываются в первый вспомогательный файл B. Как только встречаются A(ai)>A(ai+1), то записи ai+1 копируются во второй вспомогательный файл C. Процедура повторяется до тех пор, пока все записи исходной последовательности не будут распределены по файлам.\r\n\r\nШаг 2. Вспомогательные файлы B и C сливаются в файл A, при этом серии образуют упорядоченные последовательности.\r\n\r\nШаг 3. Полученный файл A вновь обрабатывается, как указано в шагах 1 и 2.\r\n\r\nШаг 4. Повторяя шаги, сливаем упорядоченные серии до тех пор, пока не будет упорядочен целиком весь файл.\r\n\r\nСимвол \"`\" обозначает признак конца серии.\r\n\r\nПризнаками конца сортировки естественным слиянием являются следующие условия:\r\n\r\nколичество серий равно 1 (определяется на фазе слияния).\r\nпри однофазной сортировке второй по счету вспомогательный файл после распределения серий остался пустым.";
        }
    }
}
