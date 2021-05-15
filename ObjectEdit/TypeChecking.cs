using System.Windows.Forms;

namespace ObjectEdit
{
    public interface ICheckType
    {//Интерфейс через который будем обращаться к классу, обрабатывающем свойства
         Control Check(object property);//Вывод контроллера, в зависимости от определенного типа
         Control ChangeType(string type);//Замена теущего контроллера на другой
         string TypeCheck();//Возврат названия типа
    }
    public class IntOrString : ICheckType
    {//Классо обработки целого и строкового типа
        private string MyType{ get; set; }//Название типа свойства
        public Control Check(object property)
        {//Вывод контроллера свойства
            Control NewControl = new Control();
            if (property.GetType() == typeof(string))
            {//Выбор строкового свойства
                MyType = "string";
                NewControl= StringControl();//Определяем контроллер для строкового свойтва 
            }
            else if (property.GetType() == typeof(int))
            {//Выбор целочисленного свойства
                MyType = "int";
                NewControl= IntControl();//Определяем контроллер для целочисленного свойтва 
            }
            return NewControl;
        }
        public Control ChangeType(string type)
        {//Вывод контроллера свойства для заммены
            Control NewControl = new Control();
            if (type == "string")
            {//Если объект был для строковых значений
                MyType = "string";
                NewControl = StringControl();
            }
            else if (type == "int")
            {//Если объект был для целочисленных значений
                MyType = "int";
                NewControl=IntControl();
            }
            return NewControl;
        }
        public string TypeCheck()
        {//Возват названия типа
            return MyType;
        }
        private Control StringControl()
        {//Элемент формы для строковых данных
            TextBox myBox = new TextBox();
            myBox.Width = 100;
            return new TextBox();
        }
        private Control IntControl()
        {//Элемент формы для целочисленных данных
            NumericUpDown myBox = new NumericUpDown();
            myBox.Maximum = int.MaxValue;
            myBox.Width = 100;
            return myBox;
        }
    }

}
