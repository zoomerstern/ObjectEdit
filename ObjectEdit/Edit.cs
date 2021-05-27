using System;
using System.Collections.Generic;
using System.Drawing;
using System.Dynamic;
using System.Linq;
using System.Windows.Forms;
namespace ObjectEdit
{
    public partial class Edit : Form
    {
        class ObjElement 
        {//Свойство передаваемого объекта
            public Control myLabelObject { get; set; }//Имя свойства объекта
            public Control myControl { get; set; }//Объект на форме
            public ObjElement(Control myLabelObject, Control myControl)
            {//Инициализация свойства
                this.myLabelObject = myLabelObject;
                this.myControl = myControl;
            }
            public void ChangeControl(Control myControl)
            {//Изменение объекта
                this.myControl = myControl;
            }
        }
        public dynamic NewObj { get; set; }//Измененный объект, который будет передаваться в другую форму
        private Dictionary<object, ObjElement> Element;//Список свойств объекта на фрме
        private Dictionary<Type, IFactory> Factory=new Dictionary<Type, IFactory>();//Массив обработчиков
        //===В случае если открыли форму для редактирования объекта===
        public Edit(string name, dynamic myObj)
        {//Если передали объект на редактирование
            InitializeComponent();
            NewObj = myObj;
            InputFactory();//регистрация обработчиков
            OutElement(name);//Выводим свойства+
            //Задваем размер окна:
            Height = PointY.getY() + 75;//Высота
            Width = 300;//Ширина
        }
        private void InputFactory() 
        {//Регистрация обработчиков
            Factory.Add(typeof(int), new IntFactory());//Целочисленный
            Factory.Add(typeof(string), new StringFactory());//Строковый
        }
        private void OutElement(string name)
        {//Вывод элементов
            label.Text = name; //Имя объекта
            OutProperty(); //Вывод свойств объекта
        }
        private void OutProperty() 
        {//Вывод свойств
            Element = new Dictionary<object, ObjElement>();
            var map = (IDictionary<String, Object>)NewObj;//Словарь свойств
            foreach (var property in map)
            {
                if (property.Value != null && Factory.ContainsKey(property.Value.GetType()))
                {//Вывод свойства
                    Label myLabel = LabelControl(property.Key.ToString());
                    Control PropertyControl = Factory[property.Value.GetType()].ReturnControl();//Элемент связанный с чекбокс и контейнер для свойства
                    ComboBox TypeBox = TypeControl();//Инииализация бокса типа объекта
                    TypeBox.SelectedItem = property.Value.GetType().Name;//Вывод названия типа данных
                    ControlAdd(PropertyControl, property.Value.ToString(), new Point(80, PointY.getYplus()));//Вывод контейнера свойства
                    ControlAdd(myLabel, null, new Point(30, PointY.getY() + 3));//Вывод лейбла названия свойства
                    ControlAdd(TypeBox, null, new Point(190, PointY.getY()));//Вывод комбобокса типов 
                    Element.Add(TypeBox, new ObjElement(myLabel, PropertyControl));//Добавление в словарь свойства 
                }
            }
        }  
        private void ChangeType(object sender, EventArgs e)
        {//Изменение эелмента формы в зависимоти от значения
            var currElement = Element[sender];//Выбираем свойсвтво из словаря
            Controls.Remove((currElement.myControl));//Удаляем стрый элемент формы
            string myType = ((ComboBox)sender).SelectedItem.ToString();
            Control NewControl = Factory.Where(x=>x.Key.Name==myType).FirstOrDefault().Value.ReturnControl();//Заменяем контроллер
            ControlAdd(NewControl, (currElement.myControl).Text.ToString(), (currElement.myControl).Location);
            currElement.ChangeControl(NewControl);//Заменяем контрол
            NewControl.BringToFront();//Выводим на передний план
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
            TypeBox.Items.Add(typeof(int).Name);
            TypeBox.Items.Add(typeof(string).Name);
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
            if (frm.NewProperty==null || ((IDictionary<string, Object>)NewObj).ContainsKey(frm.NewProperty.myName))
            {//Есть ли данное свойство
                MessageBox.Show("Такое свойство уже имеется или оно равно null!");
                return;
            }
            try
            {//Добавление свойства
                var currFactory=Factory.Where(x => x.Key.Name == frm.NewProperty.myType).FirstOrDefault();//выбираем обработчик
                object value= currFactory.Value.ReturnValue(frm.NewProperty.myValue);//выводим объект определенного типа
                ((IDictionary<string, Object>)NewObj).Add(frm.NewProperty.myName, value);//Добавление свойства в наш объект
                MessageBox.Show("Добавлнно новое совйство " + frm.NewProperty.myName);
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
                Controls.Remove(cur.Value.myControl);
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
                    string key = ((ComboBox)curr.Key).SelectedItem.ToString();
                    var currFactory = Factory.Where(x => x.Key.Name == key).FirstOrDefault();//Выбираем обработчик
                    if (currFactory.Key != null)
                        myObj.Add(currObject.myLabelObject.Text, currFactory.Value.ReturnValue(currObject.myControl.Text));//Добовляем свойство
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
