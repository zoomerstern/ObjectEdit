using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ObjectEdit
{
    public partial class Add : Form
    {
        public class MyProperty 
        {//Класс нового свойства
            public string myName;//имя
            public string myValue;//значение
            public string myType;//тип
            public MyProperty(string myName, string myValue, string myType)
            {
                this.myName= myName;
                this.myValue= myValue;
                this.myType= myType;
            }
        }
        public MyProperty NewProperty { get; set; }//Передаваемое свойсто
        public Add()
        {
            InitializeComponent();
            //Инициализация бокса типов
            comboBox1.Items.Add(typeof(int).Name);
            comboBox1.Items.Add(typeof(string).Name);
        }

        private void bAdd(object sender, EventArgs e)
        {//Добавление свойства
            //Проверки формы
            string name = (textBox1.Text).Trim(),//имя свойства
                   val = (textBox2.Text).Trim();//значение
                
            if (name == "")
            {//Проверка названия
                MessageBox.Show("Название свойства не может быть пустым");
                return;
            }
            if (comboBox1.SelectedItem == null || (comboBox1.SelectedItem.ToString() != typeof(int).Name && comboBox1.SelectedItem.ToString() != typeof(string).Name))
            {//Проверка типа
                MessageBox.Show("Неправильный тип");
                return;
            } 
            if (val == "")
            {//Проверка значения             
                MessageBox.Show("Значение свойства не может быть пустым");
                return;
            }
            if(comboBox1.SelectedItem.ToString() == typeof(int).Name && Regex.IsMatch(val, @"\D"))
            {//Проверка целочисленного значения
                MessageBox.Show("Ошибка в числовом значнии");
                return;
            }
            //Инициализация свойства
            NewProperty=new MyProperty(name, val, comboBox1.SelectedItem.ToString());
            Close();//Закрытие формы
        }
    }
}
