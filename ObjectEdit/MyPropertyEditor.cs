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
    public partial class MyPropertyEditor : Form
    {
        private Dictionary<string, object> Objects = new Dictionary<string, object>();//Список объектов для редактирования
        public MyPropertyEditor()
        {
            InitializeComponent();
            ObjectBegin();//Начальные объекты
            ObjectOut();//Вывод объектов
        }
        private void ObjectBegin()
        {//Добавим начальные объекты
            //1
            dynamic myObj = new ExpandoObject();
            myObj.Name = "Smith";
            myObj.Age = 33;
            myObj.qwe3 = "Test2";
            myObj.qwe2 = "Test2";
            Objects.Add("Object" + Objects.Count, myObj);
            //2
            myObj = new ExpandoObject();
            myObj.Name = "Дмитрий";
            myObj.Name2 = "Козлов";
            myObj.Prof = "Стритель";
            myObj.Age = 30;
            myObj.Hobby = "Туризм";
            Objects.Add("Object" + Objects.Count, myObj);
            //3
            myObj = new ExpandoObject();
            myObj.Name = "Mitsubishi";
            myObj.Name2 = "Lancer";
            myObj.Motor = "lb 2000";
            myObj.Mass = "2000";
            Objects.Add("Object" + Objects.Count, myObj);
        }
        
        private void ObjectOut() {
            //Вывод объектов
            int i = 1;
            dataGridView1.Rows.Clear();//очистить список
            foreach (var Obj in Objects)
            {
                dataGridView1.Rows.Add(new string[2] { i.ToString(), Obj.Key.ToString()
                                                      });
                i++;
            }
        }
        private void Edit()
        {//Функция открытии формы создания/редактирования объекта          
            if (dataGridView1[0, dataGridView1.CurrentRow.Index].Value == null) //Проверка на нажатие
                return;
            //Определяем имя объекта
            string objName = dataGridView1[1, dataGridView1.CurrentRow.Index].Value.ToString();
            //Открываем форму для редактирования и переносим объект с его именем
            try
            {
                Edit frm = new Edit(Objects[objName], objName);
                if (frm.Enabled != false)
                    frm.ShowDialog();
                Objects[objName] = frm.myObj;//Обновляем объект
            }
            catch
            {
                MessageBox.Show("Возникла ошибка");
            }
            //Обновляем список
            ObjectOut();
            return;
        }

        private void bEdit_Click(object sender, EventArgs e)
        {//Кнопка "Выбрать"
            Edit(); 
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {//Событие на выбор объекта
            Edit();
        }
    }
}
