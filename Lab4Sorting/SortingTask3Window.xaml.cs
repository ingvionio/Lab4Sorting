using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using OfficeOpenXml;

namespace Lab4Sorting
{
    /// <summary>
    /// Логика взаимодействия для SortingTask3Window.xaml
    /// </summary>
    public partial class SortingTask3Window : Window
    {
        public SortingTask3Window()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }

        //Эти методы и эта кнопка только для того, если спросят, как мы делали
        /*private void CreateExcelbtn_Click(object sender, RoutedEventArgs e)
        {

            // Путь к файлу Excel
            string filePath = "C:\\Users\\user\\Desktop\\results.xlsx";

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            // Создаем новый файл Excel
            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                // Добавляем новый лист
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");

                // Записываем данные в разные столбцы
                worksheet.Cells[1, 1].Value = "Количество файлов";
                worksheet.Cells[1, 2].Value = "Время (мс)";
                worksheet.Cells[1, 4].Value = "Количество файлов";
                worksheet.Cells[1, 5].Value = "Время (мс)";

                string[] files = { "100words.txt", "500words.txt", "1000words.txt", "5000words.txt", "10000words.txt", "20000words.txt" };

                int line = 2;

                foreach (var file in files)
                {
                    string testPath = $"C:\\Users\\user\\Desktop\\Expirement\\{file}";

                    worksheet.Cells[line, 1].Value = file;
                    worksheet.Cells[line, 2].Value = CountTimeMerge(testPath);
                    worksheet.Cells[line, 4].Value = file;
                    worksheet.Cells[line, 5].Value = CountTimeABC(testPath);
                    line++;
                }

                // Сохраняем файл
                package.Save();
            }

        }

        private static long CountTimeMerge(string testPath)
        {
            FileReader reader = new FileReader();
            List<string> list = reader.ReadListFromFile(testPath);
            Stopwatch stopwatch = new Stopwatch();
            List<string> listSortedByMerge = new List<string>();

            stopwatch.Start();
            listSortedByMerge = MergeSort.MergeSorting(list, 0, list.Count() - 1);
            stopwatch.Stop();

            return stopwatch.ElapsedMilliseconds;
        }

        private static long CountTimeABC(string testPath)
        {
            FileReader reader = new FileReader();
            List<string> list = reader.ReadListFromFile(testPath);
            Stopwatch stopwatch = new Stopwatch();
            List<string> listSortedByABC = new List<string>();

            stopwatch.Start();
            listSortedByABC = ABCSort.ABCSorting(list, 0);
            stopwatch.Stop();

            return stopwatch.ElapsedMilliseconds;
        }
*/
        private void StartSortBtn_Click(object sender, RoutedEventArgs e)
        {
            string filePath = FilePathTB.Text;

            SortedWordsTB.Text = "";
            WordsCountTB.Text = "";

            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                MessageBox.Show("Неверный путь к файлу.");
                return;
            }

            FileReader reader = new FileReader();
            List<string> listWords = reader.ReadListFromFile(filePath);

            switch (SortComboBox.SelectedIndex)
            {
                case 0:
                    List<string> mergeSortedWords = MergeSort.MergeSorting(listWords, 0, listWords.Count() - 1);
                    Dictionary<string, int> mergeCountWords = CountWords(mergeSortedWords);

                    foreach (string word in mergeSortedWords)
                    {
                        SortedWordsTB.AppendText($"{word} \n");
                    }

                    foreach (var item in mergeCountWords)
                    {
                        WordsCountTB.AppendText($"{item.Key} - {item.Value} \n");
                    }
                    break;

                case 1:
                    List<string> ABCSortedWords = ABCSort.ABCSorting(listWords, 0);
                    Dictionary<string, int> ABCCountWords = CountWords(ABCSortedWords);

                    foreach (string word in ABCSortedWords)
                    {
                        SortedWordsTB.AppendText($"{word} \n");
                    }

                    foreach (var item in ABCCountWords)
                    {
                        WordsCountTB.AppendText($"{item.Key} - {item.Value} \n");
                    }
                    break;

                default:
                    MessageBox.Show("Выберите сортировку");
                    break;
            }
        }

        private Dictionary<string, int> CountWords(List<string> words)
        {
            Dictionary<string, int> wordCount = new Dictionary<string, int>();

            foreach (var word in words)
            {
                if (wordCount.ContainsKey(word))
                {
                    wordCount[word]++;
                }
                else
                {
                    wordCount[word] = 1;
                }
            }
            return wordCount;
        }
    }
}
