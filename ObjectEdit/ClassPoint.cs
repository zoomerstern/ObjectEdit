using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectEdit
{
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
}
