using System;
using System.Collections.Generic;
using System.Drawing;
using System.Dynamic;
using System.Windows.Forms;

namespace ObjectEdit
{
    public partial class Edit : Form
    {
        class ObjElement 
        {//Свойство передаваемого объекта
            public string myType { get; set; }//Название свойства
            public string myName { get; set; }//Имя свойства объекта
            public object myObject { get; set; }//Объект на форме
            public ObjElement(string myType, string myName, object myObject)
            {//Инициализация свойства
                this.myType = myType;
                this.myName = myName;
                this.myObject = myObject;
            }
            public void ChangeControl(object myObject, string myType)
            {//Изменение объекта
                this.myObject = myObject;
                this.myType = myType;
            }
        }
        static class PointY
        {//Счетчик расположения элементов по оси y 
            private static  int y = 10;
            static public int getYplus() 
            {//Вывод увеличенного значения
                y += 30;
                return y;
            }
            static public int getY()
            {//Вывод значения
                return y;
            }
        }
        private Dictionary<object, ObjElement> Element = new Dictionary<object, ObjElement>();//Список свойств объекта на фрме
        public dynamic myObj = new ExpandoObject();//Передаваемый объект
        //===В случае если открыли форму для редактирования объекта===
        public Edit(dynamic myObj, string name)
        {//Если передали объект на редактирование
            InitializeComponent();
            this.myObj = myObj;//Переданный объект
            OutputProperties(name);//Выводим свойства
            //Задваем размер окна:
            Height = PointY.getY() + 75;//Высота
            Width = 300;//Ширина
        }
        private void OutputProperties(string name) 
        {//Вывод свойств
            ControlAdd( new Label(), name, new Point(30, PointY.getY()));//Выводим лэйбл имени объекта
            var map = (IDictionary<String, Object>)myObj;//Словарь свойств
            foreach (var property in map)
            {
                if (property.Value != null)
                {//Вывод свойства
                    ComboBox TypeBox = TypeControl();//Вывод бокса типа объекта
                    object SelectControl = null;//Элемент связанный с чекбокс
                    if (property.Value.GetType() == typeof(string))
                    {//Вывод строкового свойства
                        //--значение свойства
                        SelectControl = StringControl();
                        ControlAdd((Control)SelectControl, property.Value.ToString(), new Point(80, PointY.getYplus()));
                        //--название свойства
                        ControlAdd(new Label(), property.Key.ToString() + ":", new Point(30, PointY.getY() + 3));
                        TypeBox.SelectedItem = "string";
                    }
                    else if (property.Value.GetType() == typeof(int))
                    {//Вывод целочисленного свойства
                        //--значение свойства
                        SelectControl = IntControl();
                        ControlAdd((Control)SelectControl, property.Value.ToString(), new Point(80, PointY.getYplus()));
                        //--название свойства
                        ControlAdd(new Label(), property.Key.ToString() + ":", new Point(30, PointY.getY() + 3));
                        TypeBox.SelectedItem = "int";
                    }
                    Element.Add(TypeBox, new ObjElement(TypeBox.SelectedItem.ToString(), property.Key.ToString(), SelectControl));
                    ControlAdd(TypeBox, null, new Point(190, PointY.getY()));
                }
            }
            //Кнопка принятия изменений
            Button ButClose = new Button();
            ButClose.Click +=new EventHandler(BClose);//Событие на нажатие
            ControlAdd(ButClose, "Принять", new Point(110, PointY.getYplus()));
        }
        private void ChangeType(object sender, EventArgs e)
        {//Изменение эелмента формы в зависимоти от значения
            var curr = Element[sender];//Выбираем свойсвтво из словаря
            Controls.Remove(((Control)curr.myObject));//Удаляем стрый элемент формы
            Control NewControl;//Создаем новый элемент формы
            if (((ComboBox)sender).SelectedItem.ToString() == "string")
            {//Если объект был для строковых значений
                NewControl = StringControl();
                ControlAdd(NewControl, ((Control)curr.myObject).Text.ToString(), ((Control)curr.myObject).Location);
                NewControl.BringToFront();
                Element[sender].ChangeControl(NewControl, "string");
            }
            else if(((ComboBox)sender).SelectedItem.ToString() == "int")
            {//Если объект был для целочисленных значений
                NewControl = IntControl();
                ControlAdd(NewControl, ((Control)curr.myObject).Text.ToString(), ((Control)curr.myObject).Location);
                NewControl.BringToFront();
                Element[sender].ChangeControl(NewControl, "int");
            }
        }
        private ComboBox TypeControl() 
        {//Элемент формы для строковых данных
            ComboBox TypeBox=new ComboBox();
            TypeBox.Items.Add("int");
            TypeBox.Items.Add("string");
            TypeBox.Width = 70;
            TypeBox.SelectionChangeCommitted += new EventHandler(ChangeType);
            return TypeBox;
        }
        private Control StringControl()
        {//Элемент формы для строковых данных
            return new TextBox(); 
        }
        private Control IntControl()
        {//Элемент формы для целочисленных данных
            NumericUpDown myBox = new NumericUpDown();
            myBox.Maximum = int.MaxValue;
            myBox.Width = 100;
            return myBox;
        }
        private void ControlAdd(Control eForm, string text, Point ContrPoint)
        {//Вывод элемента формы
            if(text!=null)
                eForm.Text = text;//Вводим занчение элемента
            eForm.Location = ContrPoint;//Определяем его положение     
            Controls.Add(eForm);//Выводим на форму
        }
        private void BClose(object sender, EventArgs e)
        {//Когда жмем кнопку "Принять", обновялем данные объекта
            try
            {
                foreach (var curr in Element)//числовые
                {
                    var currObject = curr.Value;
                    if (currObject.myType == "int") 
                    {
                        if (string.IsNullOrWhiteSpace(((Control)currObject.myObject).Text))//Числовой тип не может быть null
                            throw new ArgumentException("Поле " + curr.Key + " пустое");
                        ((IDictionary<String, Object>)myObj)[currObject.myName] = int.Parse(((Control)currObject.myObject).Text.ToString());
                    }
                    else
                        ((IDictionary<String, Object>)myObj)[currObject.myName] = ((Control)currObject.myObject).Text.ToString();
                }
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Внимание !", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            Close();//закрыть
        }
    }
}
