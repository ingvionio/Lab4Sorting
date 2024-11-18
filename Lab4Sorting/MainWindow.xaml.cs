using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Lab4Sorting
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ButtonTask1_Click(object sender, RoutedEventArgs e)
        {
            SortingTask1Window task1Window = new SortingTask1Window();
            task1Window.Show();
            this.Close();
        }

        private void ButtonTask2_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Задание 2 в разработке");
        }

        private void ButtonTask3_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Задание 3 в разработке");
        }
    }
}