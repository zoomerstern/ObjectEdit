using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ObjectEdit
{
    public partial class Add : Form
    {
        public class MyProperty 
        {//Класс нового свойства
            public string myname;//имя
            public string myvalue;//значение
            public string mytype;//тип
            public MyProperty(string myname, string myvalue, string mytype)
            {
                this.myname= myname;
                this.myvalue= myvalue;
                this.mytype= mytype;
            }
        }
        public MyProperty NewProperty { get; set; }//Передаваемое свойсто
        public Add()
        {
            InitializeComponent();
            //Инициализация бокса типов
            comboBox1.Items.Add("int");
            comboBox1.Items.Add("string");
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
            if (comboBox1.SelectedItem == null || (comboBox1.SelectedItem.ToString() != "int" && comboBox1.SelectedItem.ToString() != "string"))
            {//Проверка типа
                MessageBox.Show("Неправильный тип");
                return;
            } 
            if (val == "")
            {//Проверка значения             
                MessageBox.Show("Значение свойства не может быть пустым");
                return;
            }
            if(comboBox1.SelectedItem.ToString() == "int" && Regex.IsMatch(val, @"\D"))
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
