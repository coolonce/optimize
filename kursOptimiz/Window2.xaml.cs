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
    /// Логика взаимодействия для Window2.xaml
    /// </summary>
    public partial class Window2 : Window
    {
        public Window2()
        {
            InitializeComponent();
            string[] methodNames = { "Метод сканирования с фиксированным шагом","Метод сканирования с переменным шагом", "Комплексный метод бокса", "Симплексный метод", "Метод наискорейшего спуска", "Метод поочерёдного варьирования переменных" };
            foreach (string method in methodNames)
            {
                this.selectMetodcomboBox.Items.Add(method);
            }
            this.selectMetodcomboBox.SelectedItem = methodNames[0];
        }

        private void windowSelectMethod_Closed(object sender, EventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }

        private void selectMethod_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("На данный момент, из представленных методов реализован только метод сканирования, поэтому оптимизация задач будет проводится с использованием метода сканирования с фиксированным шагом");
            this.selectMetodcomboBox.SelectedItem = this.selectMetodcomboBox.Items[0].ToString();
        }
    }
}
