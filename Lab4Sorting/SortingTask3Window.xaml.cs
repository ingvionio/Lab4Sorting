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
            FileReader reader = new FileReader();
            string filePath = "C:\\Users\\user\\Desktop\\test.txt"; // Укажите путь к вашему файлу

            try
            {
                string[] words = reader.ReadWordsFromFile(filePath);
                foreach (var word in words)
                {
                    testLabel.Content += word;
                    testLabel.Content += " ";
                }

                string[] sortedWords = MergeSort.MergeSorting(words, 0, words.Length - 1);
                foreach (var word in sortedWords)
                {
                    sortedLabel.Content += word;
                    sortedLabel.Content += " ";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
