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

namespace kursOptimiz
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }
        

        public Window1(Func<bool> intiDataForTask)
        {
            InitializeComponent();
            this.intiDataForTask = intiDataForTask;
        }

        public string KeyTask = "Задание Винокурова Никиты";
        private Func<bool> intiDataForTask;

        public void GetTasks()
        {
            Tasks t = new Tasks();
            var list = t.GetTaskList();
            for (int i = 0; i < list.Count; i++)
            {
                selectTaskcomboBox.Items.Add(list.Keys.ToArray()[i]);
            }
        }
        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string t = selectTaskcomboBox.SelectedValue.ToString();
            this.KeyTask = t;
            intiDataForTask();
            this.Visibility = Visibility.Hidden;
        }

        private void windowSelectTask_Initialized(object sender, EventArgs e)
        {
            GetTasks();
        }

        private void selectTask_Click(object sender, RoutedEventArgs e)
        {
            string t = selectTaskcomboBox.SelectedValue.ToString();            
            this.KeyTask = t;
            intiDataForTask();
            this.Visibility = Visibility.Hidden;
        }

        private void windowSelectTask_Closed(object sender, EventArgs e)
        {
            //intiDataForTask();
            this.Visibility = Visibility.Hidden;
        }
    }
}
