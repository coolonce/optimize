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
using System.IO;
namespace kursOptimiz
{
    /// <summary>
    /// Логика взаимодействия для ShowFormalTask.xaml
    /// </summary>
    public partial class ShowFormalTask : Window
    {
        
        public ShowFormalTask()
        {
            InitializeComponent();
            
        }

        public Uri URI
        {
            get => new Uri(Directory.GetCurrentDirectory() + @"/tasks.html");
            set => browser.Source = new Uri(Directory.GetCurrentDirectory() + @"/tasks.html");
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }
    }
}
