using System.Windows.Forms;

namespace ObjectEdit
{
    public interface IFactory
    {//Интерфейс через который будем обращаться к классу-редактору
         Control ReturnControl();//Вывод контроллера, в зависимости от определенного типа
         object ReturnValue(object value);//Вывод значения объекта с определенным типом
    }
    public class IntFactory : IFactory
    {//Редактор обработки целого типа
        public Control ReturnControl()
        {//Вывод контроллера свойства
            return IntControl();
        }
        public object ReturnValue(object value)
        {//Возврат целочисленного объекта
            if(value.GetType()==typeof(string))
                return int.Parse(value.ToString());
            return value;
        }
        private Control IntControl()
        {//Элемент формы для целочисленных данных
            NumericUpDown myBox = new NumericUpDown();
            myBox.Maximum = int.MaxValue;
            myBox.Width = 100;
            return myBox;
        }
    }

    public class StringFactory : IFactory
    {//Редактор обработки строкового типа
        public Control ReturnControl()
        {//Вывод контроллера свойства
            return StringControl();
        }
        public object ReturnValue(object value) 
        {//Возврат строкового объекта
            return (string)value;
        }
        private Control StringControl()
        {//Элемент формы для строковых данных
            TextBox myBox = new TextBox();
            myBox.Width = 100;
            return new TextBox();
        }
    }


}
