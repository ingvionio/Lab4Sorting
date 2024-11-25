using System;
using System.Collections.Generic;
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

        private void CreateExcelbtn_Click(object sender, RoutedEventArgs e)
        {
            // Путь к файлу Excel
            string filePath = "C:\\Users\\user\\Desktop\\test.xlsx";

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            // Создаем новый файл Excel
            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                // Добавляем новый лист
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");

                // Записываем данные в разные столбцы
                worksheet.Cells[1, 1].Value = "Column 1";
                worksheet.Cells[1, 2].Value = "Column 2";
                worksheet.Cells[1, 3].Value = "Column 3";

                worksheet.Cells[2, 1].Value = "Data 1";
                worksheet.Cells[2, 2].Value = "Data 2";
                worksheet.Cells[2, 3].Value = "Data 3";

                worksheet.Cells[3, 1].Value = "Data 4";
                worksheet.Cells[3, 2].Value = "Data 5";
                worksheet.Cells[3, 3].Value = "Data 6";

                // Сохраняем файл
                package.Save();
            }
        }

        private void StartSortBtn_Click(object sender, RoutedEventArgs e)
        {
            string filePath = FilePathTB.Text;

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
                        SortedWordsTB.ScrollToEnd();
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
                        SortedWordsTB.AppendText($"{word} \n")
                    }

                    foreach (var item in ABCCountWords)
                    {
                        WordsCountTB.AppendText($"{item.Key} - {item.Value} \n");
                        WordsCountTB.ScrollToEnd();
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
