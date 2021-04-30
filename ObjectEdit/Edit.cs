using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ObjectEdit
{
    public partial class Edit : Form
    {
        private Dictionary<string, Dictionary <string, object> > Element = new Dictionary<string, Dictionary<string, object>>();//Список свойств объекта на фрме
        private int y = 10;//Счетчик расстояния
        public dynamic myObj = new ExpandoObject();//Передаваемый объект

        //===В случае если открыли форму для редактирования объекта===
        public Edit(dynamic myObj, string name)
        {//Если передали объект на редактирование
            InitializeComponent();
            this.myObj = myObj;//Переданный объект
            //и его свойства
            Element.Add("string",new Dictionary<string, object>());//Свойства строковые
            Element.Add("int", new Dictionary<string, object>());//Свойства числовые
            inputSelet(name);//Выводим свойства
            //Задваем размер окна:
            Height = y + 75;//Высота
            Width = 250;//Ширина
        }

        private void inputSelet(string name) {
            //Вывод свойств
            ControlAdd( new Label(), name, new Point(30, y));//Выводим лэйбл имени объекта
            var map = (IDictionary<String, Object>)myObj;//Словарь свойств
            foreach (var property in map)
            {
                if(property.Value!=null)
                    if (property.Value.GetType() == typeof(string))
                    {//Вывод строкового свойства
                        //--значение свойства
                        y += 30;
                        TextBox myBox = new TextBox();
                        ControlAdd(myBox, property.Value.ToString(), new Point(80, y));
                        //--название свойства
                        ControlAdd(new Label(), property.Key.ToString()+":", new Point(30, y+3));
                        //--добавляем в словарь
                        Element["string"].Add(property.Key.ToString(), myBox);
                    
                    }
                    else if (property.Value.GetType() == typeof(int))
                    {//Вывод целочисленного свойства
                        //--значение свойства
                        y += 30;
                        NumericUpDown myBox = new NumericUpDown();
                        myBox.Maximum = int.MaxValue;
                        ControlAdd(myBox, property.Value.ToString(), new Point(80, y));
                        //--название свойства
                        ControlAdd(new Label(), property.Key.ToString() + ":", new Point(30, y + 3));
                        //--добавляем в словарь
                        Element["int"].Add(property.Key.ToString(), myBox);
                    }
            }
            //Кнопка принятия изменений
            y += 30;
            Button ButClose = new Button();
            ButClose.Click +=new EventHandler(bClose);//Событие на нажатие
            ControlAdd(ButClose, "Принять", new Point(80, y));
        }
        private void ControlAdd(Control eForm, string text, Point ContrPoint) {
            eForm.Text = text;//Вводим занчение элемента
            eForm.Location = ContrPoint;//Определяем его положение     
            Controls.Add(eForm);//Выводим на форму
        }
        private void bClose(object sender, EventArgs e)
        {//Когда закрываем, обновялем данные объекта
            foreach (var curr in Element["string"])//строковые
                ((IDictionary<String, Object>)myObj)[curr.Key] = ((TextBox)curr.Value).Text.ToString();
            foreach (var curr in Element["int"])//числовые
                ((IDictionary<String, Object>)myObj)[curr.Key] = int.Parse(((NumericUpDown)curr.Value).Text.ToString());
            Close();//закрыть
        }

    }
}
