using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace MyPaint
{

    enum UserTool
    {
        None,
        Line,
        Rectangle,
        Circle,
        Pen
    }
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        bool press = false;             // Флаговая переменная нажатия ЛКМ

        int x1;                         // Координаты для рисования фигур
        int y1;                         // Координаты для рисования фигур

        int _width;                     // Ширина фигуры
        int _height;                    // Высота фигуры

        Point MousePressed;             // Точка на плоскости, в которой была нажата ЛКМ
        Point MouseUnpressed;           // Точка на плоскости, в которой была нажата ПКМ

        Color Current = Color.Black;    // Текущий цвет пользователя
        int thickness = 1;              // Толщина линии

        public Bitmap bmp = new Bitmap(674, 385);  // Создаем картинку в которой будем выполнять рисование
       
        /// <summary>
        /// Решил сделать выбор пользователя перечислениями, на мой взгляд, код становиться более читебелен,
        /// Нежели указывать номера кнопок (решил кнопки не переименовывать).
        /// </summary>
        private UserTool UserChiose()
        {
            UserTool UI = UserTool.None;
            if (radioButton1.Checked)
            {
                UI = UserTool.Circle;
            }
            if (radioButton2.Checked)
            {
                UI = UserTool.Rectangle;
            }
            if (radioButton3.Checked)
            {
                UI = UserTool.Line;
            }
            if (radioButton4.Checked)
            {
                UI = UserTool.Pen;
            }
            return UI;
        }

        /// <summary>
        /// Меню диалога выбора цвета для пользовтеля
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult D = colorDialog1.ShowDialog();         // Вывод диалога о выборе цвета
            if (D == DialogResult.OK)                           // Если диалг выведен
            {
                Current = colorDialog1.Color;                   // Применяем выбранный цвет
            }
        }

        /// <summary>
        /// Выбор толщины линии
        /// </summary>
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            thickness = int.Parse(comboBox1.Text);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            pictureBox1.Image = bmp;
            DoubleBuffered = true;
            progressBar1.Visible = false;

        }

        /// <summary>
        /// Функция "кисть", отслеживается текущее положение мыши, текущее положение является началом, и концом для 
        /// отрисовки линии. т.е. получаем точку
        /// </summary>
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            Graphics g = Graphics.FromImage(pictureBox1.Image);                     
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;  // Сглаживание "фигур"

            if (press)                                  // Если зажата мышь, начальная координата будет перемещаться
            {
                MouseUnpressed = e.Location;            // моменты "отжатия" являются текущим положением мыши

                if (UserChiose() == UserTool.Pen)       // Рисование кистью
                {
                    g.DrawLine(new Pen(Current, thickness), MousePressed, MouseUnpressed);  // Рисуем  "точку"

                    MousePressed = MouseUnpressed;      // "Предыдущая" координата для отрисовки "линии", которая превратится в точку
                }
            }
            pictureBox1.Refresh();                      // Обновляем картинку
        }

        /// <summary>
        /// Рисуем фигуры на "поле"
        /// </summary>
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Pen MyPen = new Pen(Current, thickness);    // Выбранный тип кисти

            if (press)                                  // Условие для рисования
            {
                x1 = Math.Min(MousePressed.X, MouseUnpressed.X);
                y1 = Math.Min(MousePressed.Y, MouseUnpressed.Y);

                _width = Math.Abs(MousePressed.X - MouseUnpressed.X);
                _height = Math.Abs(MousePressed.Y - MouseUnpressed.Y);

                if (UserChiose() == UserTool.Rectangle)
                {
                    e.Graphics.DrawRectangle(MyPen, x1, y1, _width, _height);
                }
                if (UserChiose() == UserTool.Circle)
                {
                    e.Graphics.DrawEllipse(MyPen, x1, y1, _width, _height); 
                }
                if (UserChiose() == UserTool.Line)
                {
                    e.Graphics.DrawLine(MyPen, MousePressed, MouseUnpressed);
                }
            }
        }

        /// <summary>
        /// При отпускании ЛКМ, сохраняем фигуры на поле.
        /// </summary>
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            Pen MyPen = new Pen(Current, thickness);

            press = false;
            Graphics g = Graphics.FromImage(pictureBox1.Image);

            if (UserChiose() == UserTool.Circle)
            {
                g.DrawEllipse(MyPen, x1, y1, _width, _height);
            }
            if (UserChiose() == UserTool.Rectangle)
            {
                g.DrawRectangle(MyPen, x1, y1, _width, _height);
            }
            if (UserChiose() == UserTool.Line)
            {
                g.DrawLine(MyPen, MousePressed, MouseUnpressed);
            }
            g.Save();       // Сохраняем изменения
        }

        /// <summary>
        /// Флаг нажатия клавиши, сохраняем координаты нажатия мыши
        /// </summary>
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            press = true;
            MousePressed = e.Location;
        }

        /// <summary>
        /// Очищаем рисунок, накладываем пустой рисунок поверх имеющегося
        /// </summary>
        private void button2_Click(object sender, EventArgs e)
        {
            Bitmap bit2 = new Bitmap(674, 385);
            pictureBox1.Image = bit2;

            pictureBox1.Invalidate();       // Обновляем Picturebox
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Толщина шрифта, при неверном вводе, сохраняется старый параметр
        /// </summary>
        private void comboBox1_TextUpdate(object sender, EventArgs e)
        {
            int.TryParse(comboBox1.Text, out thickness);
        }

        /// <summary>
        /// Меню сохранение картинки
        /// </summary>
        private void Save_Click(object sender, EventArgs e)
        {
            Bitmap bmpSave = bmp;
            SaveFileDialog sfd = new SaveFileDialog();

            sfd.DefaultExt = "bmp";
            sfd.Filter = "Image Files (*.bmp) | *.bmp |All files (*.*)|*.*";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                bmpSave.Save(sfd.FileName);
            }

        }

        /// <summary>
        /// Выпадающее меню
        /// </summary>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Открытие BMP файла 
        /// </summary>
        private void button5_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "All Files|*.*";

            openFile.DefaultExt = "bmp";

            if (openFile.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image = Image.FromFile(openFile.FileName);
            }
        }

        /// <summary>
        /// Инверсия рисунка, выполняем в отдельном потоке с помощью Background Worker'a
        /// </summary>
        private void btn_Inversion_Click(object sender, EventArgs e)
        {
            progressBar1.Visible = true;
            btn_Inversion.Enabled = false;          // Деактивируем кнопку
            pictureBox1.Enabled = false;

            backgroundWorker1.RunWorkerAsync();     // Запускаем задачу в фоне
        }

        
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            IsInverse();        // Задача для фоновой обработки
        }

        /// <summary>
        /// Метод, инвертирующий цвета на рисунке, 
        /// </summary>
        private void IsInverse()
        {
            int status = bmp.Width;     // Считаем внешний цикл как "индикатор"
            int currentStatus = 0;      // Текущее состояние обработки

            for (int x = 0; x <= bmp.Width - 1; x++, currentStatus++)
            {
                backgroundWorker1.ReportProgress((x * 100) / status);   // Отчет % выполнения для backgroundWorker'a

                for (int y = 0; y <= bmp.Height - 1; y++)
                {
                    Color oldColor = bmp.GetPixel(x, y);
                    Color newColor = Color.FromArgb(oldColor.A, 255 - oldColor.R, 255 - oldColor.G, 255 - oldColor.B);  // Инверсия RGB цветов
                    bmp.SetPixel(x, y, newColor);
                }
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage + 1;  // такой-себе костыль, для корректной отработки прогресс-бара

            progressBar1.Value = e.ProgressPercentage;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            btn_Inversion.Enabled = true;
            pictureBox1.Enabled = true;
            progressBar1.Visible = false;

            pictureBox1.Refresh();                          // Обновляем картинку по завершении задачи
        }

    }
}
