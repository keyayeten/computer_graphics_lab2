using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _123
{
    public partial class Form1 : Form
    {
        Bitmap bmp;
        Graphics g;
        Pen pen0 = new Pen(Color.Black,1);
        Pen penx = new Pen(Color.Red, 1);
        Pen peny = new Pen(Color.Blue, 1);
        Pen penz = new Pen(Color.Lime, 1);
        List<Pen> pens;
        MyMatrix Rx_plus, Ry_plus, Rz_plus, Rx_minus, Ry_minus, Rz_minus, Ax, Az, A_iso;
        MyObject axe_x, axe_y, axe_z, floor, col1, col2, roof, wall1, wall2;
        List<MyObject> my_object0, my_object;
        bool is_down = false;
        int cursor_x, cursor_y;

        public Form1()
        {
            InitializeComponent();
            bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            g = Graphics.FromImage(bmp);
            g.Clear(Color.White);
            g.TranslateTransform(pictureBox1.Width/2, pictureBox1.Height/2);
            g.ScaleTransform(1, -1);
            
            Rx_plus = new MyMatrix(Rx((float)0.1));
            Rx_minus = new MyMatrix(Rx(-(float)0.1));
            Ry_plus = new MyMatrix(Ry((float)0.1));
            Ry_minus = new MyMatrix(Ry(-(float)0.1));
            Rz_plus = new MyMatrix(Rz((float)0.1));
            Rz_minus = new MyMatrix(Rz(-(float)0.1));

            Ax = new MyMatrix();
            Ax.Add(0, 1, 0, 0);
            Ax.Add(1, 0, 0, 0);
            Ax.Add(0, 0, 1, 0);
            Ax.Add(0, 0, 0, 1);

            Az = new MyMatrix();
            Az.Add(1, 0, 0, 0);
            Az.Add(0, 0, 1, 0);
            Az.Add(0, 1, 0, 0);
            Az.Add(0, 0, 0, 1);

            A_iso = new MyMatrix();
            A_iso.Add(1, 0, 0, 0);
            A_iso.Add(0, 0, 1, 0);
            A_iso.Add(0, 1, 0, 0);
            A_iso.Add(0, 0, 0, 1);

            init_obj();
            draw_pic();
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            is_down = true;
            cursor_x = e.X;
            cursor_y = e.Y;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            int tmp = 20;
            if (is_down)
            {
                int x = e.X - cursor_x;
                int y = e.Y - cursor_y;
                if (x > tmp)
                {
                    mult_all(Rz_plus, ref my_object);
                    cursor_x = e.X;
                    cursor_y += y / 2;
                }
                if (x < -tmp)
                {
                    mult_all(Rz_minus, ref my_object);
                    cursor_x = e.X;
                    cursor_y += y/2;
                }
                if (y > tmp)
                {
                    if (ModifierKeys.HasFlag(Keys.Shift))
                    {
                        mult_all(Ry_plus, ref my_object);
                    }
                    else mult_all(Rx_plus, ref my_object);
                    cursor_x += x/2;
                    cursor_y = e.Y;
                }
                if (y < -tmp)
                {
                    if (ModifierKeys.HasFlag(Keys.Shift))
                    {
                        mult_all(Ry_minus, ref my_object);
                    }
                    else mult_all(Rx_minus, ref my_object);
                    cursor_x += x / 2;
                    cursor_y = e.Y;
                }
                draw_pic();
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            is_down = false;
        }

        public class MyMatrix
        {
            public int height;
            public List<List<float>> numbers;
            public MyMatrix()
            {
                height = 0;
                numbers = new List<List<float>>();
            }
            public MyMatrix(int height)
            {
                this.height = height;
                numbers = new List<List<float>>();
                for (int i = 0; i < this.height; i++)
                {
                    numbers.Add(new List<float>());
                    for (int j = 0; j <= 3; j++)
                    {
                        numbers[i].Add(new float());
                    }
                }
            }
            public MyMatrix(MyMatrix copy) : this(copy.height)
            {
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j <= 3; j++)
                    {
                        numbers[i][j] = copy.numbers[i][j];
                    }
                }
            }
            public void Add(float x, float y, float z, float p)
            {
                numbers.Add(new List<float>());
                height++;
                numbers[height - 1].Add(x);
                numbers[height - 1].Add(y);
                numbers[height - 1].Add(z);
                numbers[height - 1].Add(p);
            }
            public static MyMatrix operator *(MyMatrix A, MyMatrix B)
            {
                MyMatrix res = new MyMatrix(A.height);
                if (4 == B.height)
                {
                    for (int i = 0; i < res.height; i++)
                    {
                        for (int j = 0; j <= 3; j++)
                        {
                            res.numbers[i][j] = (float)0;
                            for (int k = 0; k <= 3; k++)
                            {
                                res.numbers[i][j] += A.numbers[i][k] * B.numbers[k][j];
                            }
                        }
                    }
                }
                return res;
            }
            public static MyMatrix operator +(MyMatrix A, MyMatrix B)
            {
                MyMatrix res = new MyMatrix(A.height);
                if (A.height == B.height)
                {
                    for (int i = 0; i < res.height; i++)
                    {
                        for (int j = 0; j <= 3; j++)
                        {
                            res.numbers[i][j] = A.numbers[i][j] + B.numbers[i][j];
                        }
                    }
                }
                return res;
            }
            public static MyMatrix operator *(MyMatrix A, float num)
            {
                MyMatrix res = new MyMatrix(A.height);
                for (int i = 0; i < res.height; i++)
                {
                    for (int j = 0; j <= 3; j++)
                    {
                        res.numbers[i][j] = A.numbers[i][j] * num;
                    }
                }
                return res;
            }
        }

        public MyMatrix moving_on(float dx, float dy, float dz)
        {
            MyMatrix A = new MyMatrix();
            A.Add(1, 0, 0, 0);
            A.Add(0, 1, 0, 0);
            A.Add(0, 0, 1, 0);
            A.Add(dx, dy, dz, 1);
            return A;
        }

        public MyMatrix zooming(float kx, float ky, float kz)
        {
            MyMatrix A = new MyMatrix();
            A.Add(kx, 0, 0, 0);
            A.Add(0, ky, 0, 0);
            A.Add(0, 0, kz, 0);
            A.Add(0, 0, 0, 1);
            return A;
        }

        public MyMatrix Rx(float fi)
        {
            MyMatrix res = new MyMatrix();
            res.Add(1, 0, 0, 0);
            res.Add(0, (float)Math.Cos(fi), -(float)Math.Sin(fi), 0);
            res.Add(0, (float)Math.Sin(fi), (float)Math.Cos(fi), 0);
            res.Add(0, 0, 0, 1);
            return res;
        }
        public MyMatrix Ry(float fi)
        {
            MyMatrix res = new MyMatrix();
            res.Add((float)Math.Cos(fi), 0, (float)Math.Sin(fi), 0);
            res.Add(0, 1, 0, 0);
            res.Add(-(float)Math.Sin(fi), 0, (float)Math.Cos(fi), 0);
            res.Add(0, 0, 0, 0);
            return res;
        }
        public MyMatrix Rz(float fi)
        {
            MyMatrix res = new MyMatrix();
            res.Add((float)Math.Cos(fi), (float)Math.Sin(fi), 0, 0);
            res.Add(-(float)Math.Sin(fi), (float)Math.Cos(fi), 0, 0);
            res.Add(0, 0, 1, 0);
            res.Add(0, 0, 0, 1);
            return res;
        }
        public class MyObject
        {
            public MyMatrix vertices;
            public List<int[]> edges;
            public List<int[]> pics;
            public MyObject()
            {
                vertices = new MyMatrix();
                edges = new List<int[]>();
                pics = new List<int[]>();
            }
            public MyObject(MyObject copy)
            {
                vertices = new MyMatrix(copy.vertices);
                this.edges = copy.edges;
            }
            public void draw(ref Graphics g, Pen pen)
            {
                for (int i = 0; i < edges.Count; i++)
                {
                    if (edges[i] != null)
                    {
                        for (int j = 0; j < edges[i].Length; j++)
                        {
                            float x1 = vertices.numbers[i][0];
                            float z1 = vertices.numbers[i][2];
                            float x2 = vertices.numbers[edges[i][j]][0];
                            float z2 = vertices.numbers[edges[i][j]][2];
                            g.DrawLine(pen, x1, z1, x2, z2);
                        }
                    }
                }
            }
        }

        public void init_obj()
        {
            my_object0 = new List<MyObject>();
            my_object = new List<MyObject>();
            pens = new List<Pen>();

            roof = new MyObject();
            roof.vertices.Add(0, 0, 0, 1); //0
            roof.edges.Add(new int[] { 1,3,4});
            roof.vertices.Add(0, 12, 0, 1); //1
            roof.edges.Add(new int[] { 2,5 });
            roof.vertices.Add(20, 12, 0, 1); //2
            roof.edges.Add(new int[] { 3, 6 });
            roof.vertices.Add(20, 0, 0, 1); //3
            roof.edges.Add(new int[] { 7 });
            roof.vertices.Add(0, 0, 2, 1); //4
            roof.edges.Add(new int[] { 5, 7 });
            roof.vertices.Add(0, 12, 2, 1); //5
            roof.edges.Add(new int[] { 6 });
            roof.vertices.Add(20, 12, 2, 1); //6
            roof.edges.Add(new int[] { 7 });
            roof.vertices.Add(20, 0, 2, 1); //7
            roof.edges.Add(null);
            roof.vertices *= moving_on(-10, -6, 10);
            my_object0.Add(roof);
            pens.Add(pen0);

            wall1 = new MyObject();
            wall1.vertices.Add(0, 0, 0, 1); //0
            wall1.edges.Add(new int[] { 1, 11, 12 });
            wall1.vertices.Add(0, 2, 0, 1); //1
            wall1.edges.Add(new int[] { 2, 13 });
            wall1.vertices.Add((float)0.5, 2, 0, 1); //2
            wall1.edges.Add(new int[] { 3, 14 });
            wall1.vertices.Add((float)0.5, 8, 0, 1); //3
            wall1.edges.Add(new int[] { 4, 15 });
            wall1.vertices.Add(0, 8, 0, 1); //4
            wall1.edges.Add(new int[] { 5, 16 });
            wall1.vertices.Add(0, 10, 0, 1); //5
            wall1.edges.Add(new int[] { 6, 17 });
            wall1.vertices.Add(2, 10, 0, 1); //6
            wall1.edges.Add(new int[] { 7, 18 });
            wall1.vertices.Add(2, 8, 0, 1); //7
            wall1.edges.Add(new int[] { 8, 19 });
            wall1.vertices.Add((float)1.5, 8, 0, 1); //8
            wall1.edges.Add(new int[] { 9, 20 });
            wall1.vertices.Add((float)1.5, 2, 0, 1); //9
            wall1.edges.Add(new int[] { 10, 21 });
            wall1.vertices.Add(2, 2, 0, 1); //10
            wall1.edges.Add(new int[] { 11, 22 });
            wall1.vertices.Add(2, 0, 0, 1); //11
            wall1.edges.Add(new int[] { 23 });
            wall1.vertices.Add(0, 0, 16, 1); //12
            wall1.edges.Add(new int[] { 13, 23 });
            wall1.vertices.Add(0, 2, 16, 1); //13
            wall1.edges.Add(new int[] { 14 });
            wall1.vertices.Add((float)0.5, 2, 16, 1); //14
            wall1.edges.Add(new int[] { 15 });
            wall1.vertices.Add((float)0.5, 8, 16, 1); //15
            wall1.edges.Add(new int[] { 16 });
            wall1.vertices.Add(0, 8, 16, 1); //16
            wall1.edges.Add(new int[] { 17 });
            wall1.vertices.Add(0, 10, 16, 1); //17
            wall1.edges.Add(new int[] { 18 });
            wall1.vertices.Add(2, 10, 16, 1); //18
            wall1.edges.Add(new int[] { 19 });
            wall1.vertices.Add(2, 8, 16, 1); //19
            wall1.edges.Add(new int[] { 20 });
            wall1.vertices.Add((float)1.5, 8, 16, 1); //20
            wall1.edges.Add(new int[] { 21 });
            wall1.vertices.Add((float)1.5, 2, 16, 1); //21
            wall1.edges.Add(new int[] { 22 });
            wall1.vertices.Add(2, 2, 16, 1); //22
            wall1.edges.Add(new int[] { 23 });
            wall1.vertices.Add(2, 0, 16, 1); //23
            wall1.edges.Add(null);
            wall2 = new MyObject(wall1);
            wall1.vertices *= moving_on(-9, -5, -6);
            wall2.vertices *= moving_on(7, -5, -6);
            my_object0.Add(wall1);
            my_object0.Add(wall2);
            pens.Add(pen0);
            pens.Add(pen0);

            floor = new MyObject();
            floor.vertices.Add(0, 0, 0, 1); //0
            floor.edges.Add(new int[] { 1, 7, 12 });
            floor.vertices.Add(4, 0, 0, 1); //1
            floor.edges.Add(new int[] { 2, 8 });
            floor.vertices.Add(4, -2, 0, 1); //2
            floor.edges.Add(new int[] { 3, 11 });
            floor.vertices.Add(16, -2, 0, 1); //3
            floor.edges.Add(new int[] { 4, 10 });
            floor.vertices.Add(16, 0, 0, 1); //4
            floor.edges.Add(new int[] { 5, 9 });
            floor.vertices.Add(20, 0, 0, 1); //5
            floor.edges.Add(new int[] { 6, 13 });
            floor.vertices.Add(20, -12, 0, 1); //6
            floor.edges.Add(new int[] { 7, 14 });
            floor.vertices.Add(0, -12, 0, 1); //7
            floor.edges.Add(new int[] { 15 });
            floor.vertices.Add(4, 0, -2, 1); //8
            floor.edges.Add(new int[] { 9, 11 });
            floor.vertices.Add(16, 0, -2, 1); //9
            floor.edges.Add(new int[] { 10 });
            floor.vertices.Add(16, -2, -2, 1); //10
            floor.edges.Add(new int[] { 11 });
            floor.vertices.Add(4, -2, -2, 1); //11
            floor.edges.Add(null);
            floor.vertices.Add(0, 0, -4, 1); //12
            floor.edges.Add(new int[] { 13, 15 });
            floor.vertices.Add(20, 0, -4, 1); //13
            floor.edges.Add(new int[] { 14 });
            floor.vertices.Add(20, -12, -4, 1); //14
            floor.edges.Add(new int[] { 15 });
            floor.vertices.Add(0, -12, -4, 1); //15
            floor.edges.Add(null);
            floor.vertices *= moving_on(-10, 6, -6);
            my_object0.Add(floor);
            pens.Add(pen0);

            col1 = new MyObject();
            col1.vertices.Add(1, 0, 0, 1); //0
            col1.edges.Add(new int[] { 1, 11, 12 });
            for (int i = 1; i <= 10; i++)
            {
                col1.vertices.Add((float)Math.Cos(i*0.5236), (float)Math.Sin(i * 0.5236), 0, 1); //1..10
                col1.edges.Add(new int[] { i+1, i+12 });
            }
            col1.vertices.Add((float)Math.Cos(11 * 0.5236), (float)Math.Sin(11 * 0.5236), 0, 1); //11
            col1.edges.Add(new int[] { 23 });
            col1.vertices.Add((float)Math.Cos(11 * 0.5236), (float)Math.Sin(11 * 0.5236), 14, 1); //12
            col1.edges.Add(new int[] { 13, 23 });
            for (int i = 1; i <= 10; i++)
            {
                col1.vertices.Add((float)Math.Cos(i * 0.5236), (float)Math.Sin(i * 0.5236), 14, 1); //13..22
                col1.edges.Add(new int[] { i + 13 });
            }
            col1.vertices.Add((float)Math.Cos(11 * 0.5236), (float)Math.Sin(11 * 0.5236), 14, 1); //23
            col1.edges.Add(null);
            col2 = new MyObject(col1);
            col1.vertices *= moving_on(-5, 8, -10);
            col2.vertices *= moving_on(5, 8, -10);
            my_object0.Add(col1);
            my_object0.Add(col2);
            pens.Add(pen0);
            pens.Add(pen0);

            axe_x = new MyObject();
            axe_x.vertices.Add(0, 0, 0, 1); //0
            axe_x.edges.Add(new int[] { 1 });
            axe_x.vertices.Add(20, 0, 0, 1); //1
            my_object0.Add(axe_x);
            pens.Add(penx);

            axe_y = new MyObject();
            axe_y.vertices.Add(0, 0, 0, 1); //0
            axe_y.edges.Add(new int[] { 1 });
            axe_y.vertices.Add(0, 20, 0, 1); //1
            my_object0.Add(axe_y);
            pens.Add(peny);

            axe_z = new MyObject();
            axe_z.vertices.Add(0, 0, 0, 1); //0
            axe_z.edges.Add(new int[] { 1 });
            axe_z.vertices.Add(0, 0, 20, 1); //1
            my_object0.Add(axe_z);
            pens.Add(penz);

            mult_all(zooming(15, 15, 15), ref my_object0);
            default_object();
        }

        private void default_object()
        {
            if (my_object != null) my_object.Clear();
            my_object = new List<MyObject>();
            for (int i = 0; i < my_object0.Count; i++) my_object.Add(new MyObject (my_object0[i]));
        }
        public void mult_all(MyMatrix A, ref List<MyObject> list)
        {
            for (int i = 0; i < list.Count; i++) list[i].vertices *= A;
        }
        public void draw_pic()
        {
            g.Clear(Color.White);
            for (int i = 0; i < my_object.Count; i++) my_object[i].draw(ref g, pens[i]);
            pictureBox1.Image = bmp;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            default_object();
            draw_pic();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            default_object();
            mult_all(Ax, ref my_object);
            draw_pic();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            default_object();
            mult_all(Az, ref my_object);
            draw_pic();
        }
    }
}
