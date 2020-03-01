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
        System.Windows.Controls.Label selectedMethods;

        public Window2(ref System.Windows.Controls.Label selectedMethods)
        {
            InitializeComponent();
            string[] methodNames = { "Метод сканирования с фиксированным шагом", "Комплексный метод бокса", "Метод сканирования с переменным шагом", "Симплексный метод", "Метод наискорейшего спуска", "Метод поочерёдного варьирования переменных" };
            foreach (string method in methodNames)
            {
                this.selectMetodcomboBox.Items.Add(method);
            }
            this.selectMetodcomboBox.SelectedItem = methodNames[0];
            this.selectedMethods = selectedMethods;
            selectedMethods.Content = this.selectMetodcomboBox.SelectedValue.ToString();
        }

        private void windowSelectMethod_Closed(object sender, EventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }

        private void selectMethod_Click(object sender, RoutedEventArgs e)
        {
            var selectMethod = selectMetodcomboBox.SelectedValue;
            if (selectMethod.ToString() != "Метод сканирования с фиксированным шагом" && selectMethod.ToString() != "Комплексный метод бокса")
            {
                MessageBox.Show("На данный момент, из представленных методов реализован только метод сканирования и комплексный метод бокса, поэтому оптимизация задач будет проводится с использованием метода сканирования с фиксированным шагом");
                this.selectMetodcomboBox.SelectedItem = this.selectMetodcomboBox.Items[0].ToString();
            }
            else
            {
                selectedMethods.Content = this.selectMetodcomboBox.SelectedValue.ToString();
                this.Close();
            }
            
        }
    }
}
