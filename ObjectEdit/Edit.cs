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
            public Control myObject { get; set; }//Объект на форме
            public ObjElement(string myType, string myName, Control myObject)
            {//Инициализация свойства
                this.myType = myType;
                this.myName = myName;
                this.myObject = myObject;
            }
            public void ChangeControl(Control myObject, string myType)
            {//Изменение объекта
                this.myObject = myObject;
                this.myType = myType;
            }
        }
        public dynamic NewObj { get; set; } //Измененный объект для внесения изменений в другой форме
        static class PointY
        {//Счетчик расположения элементов по оси y 
            private static int y = 10;
            static public int getYplus() 
            {//Вывод увеличенного значения
                y += 30;
                return y;
            }
            static public int getY()
            {//Вывод значения
                return y;
            }
            static public void dropY()
            {//Сброс счетчика
                y = 10;
            }
        }
        private Dictionary<object, ObjElement> Element = new Dictionary<object, ObjElement>();//Список свойств объекта на фрме
        //===В случае если открыли форму для редактирования объекта===
        public Edit(string name, dynamic myObj)
        {//Если передали объект на редактирование
            InitializeComponent();
            NewObj = myObj;
            OutputProperties(name);//Выводим свойства
            //Задваем размер окна:
            Height = PointY.getY() + 75;//Высота
            Width = 300;//Ширина
        }
        private void OutputProperties(string name)
        {//Вывод свойств
            ControlAdd( new Label(), name, new Point(30, PointY.getY()));//Выводим лэйбл имени объекта
            var map = (IDictionary<String, Object>)NewObj;//Словарь свойств
            foreach (var property in map)
            {
                if (property.Value != null)
                {//Вывод свойства
                    ComboBox TypeBox = TypeControl();//Вывод бокса типа объекта
                    Control PropertyControl = new Control();//Элемент связанный с чекбокс 
                    if (property.Value.GetType() == typeof(string))
                    {//Вывод строкового свойства
                        PropertyControl = StringControl();//Определяем контроллер для строкового свойтва 
                        TypeBox.SelectedItem = "string";//Задаем значение комбобокса(строковый)
                    }
                    else if (property.Value.GetType() == typeof(int))
                    {//Вывод целочисленного свойства
                        PropertyControl = IntControl();//Определяем контроллер для целочисленного свойтва 
                        TypeBox.SelectedItem = "int";//Задаем значение комбобокса(числовой)
                    }
                    ControlAdd(PropertyControl, property.Value.ToString(), new Point(80, PointY.getYplus()));//и выводим его       
                    ControlAdd(new Label(), property.Key.ToString() + ":", new Point(30, PointY.getY() + 3));//Вывод лейбла названия свойства
                    ControlAdd(TypeBox, null, new Point(190, PointY.getY()));//Вывод комбобокса
                    Element.Add(TypeBox, new ObjElement(TypeBox.SelectedItem.ToString(), property.Key.ToString(), PropertyControl));//Добавление в словарь свойства 
                }
            }
            Button ButClose = new Button(); //Кнопка принятия изменений
            ButClose.Click +=new EventHandler(BClose);//Событие на нажатие
            ControlAdd(ButClose, "Принять", new Point(110, PointY.getYplus()));//Добавляем кнопку
        }
        private void ChangeType(object sender, EventArgs e)
        {//Изменение эелмента формы в зависимоти от значения
            var curr = Element[sender];//Выбираем свойсвтво из словаря
            Controls.Remove((curr.myObject));//Удаляем стрый элемент формы
            if (((ComboBox)sender).SelectedItem.ToString() == "string")
            {//Если объект был для строковых значений
                Control NewControl = StringControl();
                ControlAdd(NewControl, (curr.myObject).Text.ToString(), (curr.myObject).Location);
                NewControl.BringToFront();
                Element[sender].ChangeControl(NewControl, "string");
            }
            else if(((ComboBox)sender).SelectedItem.ToString() == "int")
            {//Если объект был для целочисленных значений
                Control NewControl = IntControl();
                ControlAdd(NewControl, (curr.myObject).Text.ToString(), (curr.myObject).Location);
                NewControl.BringToFront();
                Element[sender].ChangeControl(NewControl, "int");
            }
            
        }
        private ComboBox TypeControl() 
        {//Элемент формы для отображения типа данных данных
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
            var myObj = new ExpandoObject() as IDictionary<string, Object>; ;
            try
            {
                foreach (var curr in Element)//числовые
                {
                    if (myObj.ContainsKey(curr.Key.ToString()))//Проверка на совпадение свойства
                        continue;
                    var currObject = curr.Value;
                    if (currObject.myType == "int") 
                    {
                        if (string.IsNullOrWhiteSpace((currObject.myObject).Text))//Числовой тип не может быть null
                            throw new ArgumentException("Поле " + curr.Key + " пустое");
                        myObj.Add(currObject.myName, int.Parse((currObject.myObject).Text.ToString()));
                    }
                    else
                        myObj.Add(currObject.myName, (currObject.myObject).Text.ToString());
                }
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Внимание !", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return ;
            }
            NewObj = myObj;//Запоминаем новый объект с обновленными свойствами
            Close();//закрыть
        }
        private void Edit_FormClosing(object sender, FormClosingEventArgs e)
        {//Событие ри закрытии формы
            PointY.dropY();//Сброс счетчика оси 
        }
    }
}
