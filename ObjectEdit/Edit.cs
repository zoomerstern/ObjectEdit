using System;
using System.Collections.Generic;
using System.Drawing;
using System.Dynamic;
using System.Windows.Forms;
namespace ObjectEdit
{
    public partial class Edit : Form
    {
        public class EditControlProperty
        {//Класс обработки типа свойства
            private readonly IEdit _edit;//Интерфейс в который внедряем  вид обработки
            public EditControlProperty(IEdit edit)
            {//Внедрение зависимотси
                _edit = edit;
            }
            public Control ReturnControl(object property)
            {//Возврат конроллера для определеного типа свойства
                return _edit.CurrControl(property);
            }
            public Control ChangeControl(string type)
            {//Изменение контроллера в случае изменения типа свойства
                return _edit.ChangeControl(type);
            }
            public string ReturnName()
            {//Возврат имени типа свойства
                return _edit.TypeCheck();
            }
        }
        public EditControlProperty myEditControlProperty;
        class ObjElement 
        {//Свойство передаваемого объекта
            public string myType { get; set; }//Название свойства
            public string myName { get; set; }//Имя свойства объекта
            public Control myLabelObject { get; set; }//Объект на форме
            public Control myObject { get; set; }//Объект на форме
            public ObjElement(string myType, string myName, Control myLabelObject, Control myObject)
            {//Инициализация свойства
                this.myType = myType;
                this.myName = myName;
                this.myLabelObject = myLabelObject;
                this.myObject = myObject;
            }
            public void ChangeControl(Control myObject, string myType)
            {//Изменение объекта
                this.myObject = myObject;
                this.myType = myType;
            }
        }
        public dynamic NewObj { get; set; }//Измененный объект, который будет передаваться в другую форму
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
        private Dictionary<object, ObjElement> Element;//Список свойств объекта на фрме
        //===В случае если открыли форму для редактирования объекта===
        public Edit(string name, dynamic myObj)
        {//Если передали объект на редактирование
            InitializeComponent();
            NewObj = myObj;
            OutElement(name);//Выводим свойства
            //Задваем размер окна:
            Height = PointY.getY() + 75;//Высота
            Width = 300;//Ширина
        }
        private void OutElement(string name)
        {//Вывод свойств
            ControlAdd(LabelControl(name), null, new Point(10, PointY.getY()+3));//Выводим лэйбл имени объекта
            //Добавление кнопок упарвления
            Button MyBut = new Button(); //Кнопка принятия изменений
            MyBut.Click += new EventHandler(BAdd);//Событие на нажатие
            ControlAdd(MyBut, "Доб. св-во", new Point(80, PointY.getY()));//Добавляем кнопку
            MyBut = new Button(); //Кнопка принятия изменений
            MyBut.Click += new EventHandler(BClose);//Событие на нажатие
            ControlAdd(MyBut, "Принять", new Point(190, PointY.getY()));//Добавляем кнопку
            //Добавление элементов свойств
            OutProperty();
        }
        private void OutProperty() {
            Element = new Dictionary<object, ObjElement>();
            myEditControlProperty = new EditControlProperty(new IntOrString());//Обьявляем класс обработки свойств. И делаем в него инъекциюо класса для работы со строковыми и целыми значениями
            var map = (IDictionary<String, Object>)NewObj;//Словарь свойств
            foreach (var property in map)
            {
                if (property.Value != null)
                {//Вывод свойства
                    Label myLabel = LabelControl(property.Key.ToString() + ":");
                    ComboBox TypeBox = TypeControl();//Инииализация бокса типа объекта
                    Control PropertyControl;//Элемент связанный с чекбокс и контейнер для свойства
                    PropertyControl = myEditControlProperty.ReturnControl(property.Value);//вывод соответвующего элемента формы
                    TypeBox.SelectedItem = myEditControlProperty.ReturnName();//Вывод названия типа данных
                    ControlAdd(PropertyControl, property.Value.ToString(), new Point(80, PointY.getYplus()));//Вывод контейнера свойства
                    ControlAdd(myLabel, null, new Point(30, PointY.getY() + 3)); ; ;//Вывод лейбла названия свойства
                    ControlAdd(TypeBox, null, new Point(190, PointY.getY()));//Вывод комбобокса типов 
                    Element.Add(TypeBox, new ObjElement(TypeBox.SelectedItem.ToString(), property.Key.ToString(), myLabel, PropertyControl));//Добавление в словарь свойства 
                }
            }
        }
        private void ChangeType(object sender, EventArgs e)
        {//Изменение эелмента формы в зависимоти от значения
            var curr = Element[sender];//Выбираем свойсвтво из словаря
            Controls.Remove((curr.myObject));//Удаляем стрый элемент формы
            Control NewControl = myEditControlProperty.ChangeControl(((ComboBox)sender).SelectedItem.ToString());//Заменяем контроллер
            ControlAdd(NewControl, (curr.myObject).Text.ToString(), (curr.myObject).Location);
            NewControl.BringToFront();//Выводим на передний план
            Element[sender].ChangeControl(NewControl, myEditControlProperty.ReturnName());
        }
        //==Типизированные элементы формы и работа с ними==
        private Label LabelControl(string  text)
        {//Элемент для отображения название обекта и его свойств
            Label TypeBox = new Label();
            TypeBox.Width = 50;
            TypeBox.Text = text;
            return TypeBox;
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
        private void ControlAdd(Control eForm, string text, Point ContrPoint)
        {//Вывод элемента формы
            if(text!=null)
                eForm.Text = text;//Вводим занчение элемента
            eForm.Location = ContrPoint;//Определяем его положение     
            Controls.Add(eForm);//Выводим на форму
        }
        //=================================
        private void BAdd(object sender, EventArgs e)
        {//Жобавление нового свойства объекту
            Add frm = new Add();//Открытие формы на добавление объекта
            if (frm.Enabled != false)
                frm.ShowDialog();
            //==Добавление свойства
            if (frm.NewProperty==null || ((IDictionary<string, Object>)NewObj).ContainsKey(frm.NewProperty.myname))
            {//Есть ли данное свойство
                MessageBox.Show("Такое свойство уже имеется или оно равно null!");
                return;
            }
            try
            {//Добавление свойства
                object value;
                if (frm.NewProperty.mytype.Equals("int"))
                    value = int.Parse(frm.NewProperty.myvalue);//Численый тип
                else
                    value = frm.NewProperty.myvalue;//Строковый
                ((IDictionary<string, Object>)NewObj).Add(frm.NewProperty.myname, value);//Добавление свойства в наш объект
                MessageBox.Show("Добавлнно новое совйство " + frm.NewProperty.myname);
            }
            catch
            {//В случае непредусмотренной ошибки
                Console.WriteLine("Возникло исключение!\n Свойсто не добавлено");
                return;
            }
            foreach (var cur in Element)
            {//Очистка элеменотов свойств
                Controls.Remove((Control)cur.Key);
                Controls.Remove(cur.Value.myLabelObject);
                Controls.Remove(cur.Value.myObject);
            }
            PointY.dropY();//обнуление занчения оси икс
            OutProperty();//И занеого отображаем на форме элементы свойств
            Height = PointY.getY() + 75;//Высота формы
        }
        private void BClose(object sender, EventArgs e)
        {//Когда жмем кнопку "Принять", обновялем данные объекта
            var myObj = new ExpandoObject() as IDictionary<string, Object>;
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
                return;
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
