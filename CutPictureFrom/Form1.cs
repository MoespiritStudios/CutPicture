using OpenCvSharp;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace CutPictureFrom
{
    public partial class Form1 : Form
    {
        public static string path;
        public static string name;
        public static int finish;
        public Form1()
        {
            InitializeComponent();
            finish = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                FileName.Text = openFileDialog1.FileName;
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                SaveFold.Text = folderBrowserDialog1.SelectedPath;
                path = SaveFold.Text;
            }
        }
        public class Temp
        {
            public int index;
            public int x;
            public int y;
            public int width;
            public int height;
            public int amount;
            public Temp(int index, int x, int y, int width, int height, int amount)
            {
                this.index = index;
                this.x = x;
                this.y = y;
                this.width = width;
                this.height = height;
                this.amount = amount;
            }
        }
        public static void ThreadMethod1(object obj)
        {

            Temp t = obj as Temp;
            Mat temp = new Mat(name);
            Mat text = new Mat(temp, new Rect(t.x, t.y, t.width, t.height));
            temp.Release();
            string pathT = path + "\\" + t.amount + "X" + t.amount + "\\" + t.index + ".png";
            text.SaveImage(pathT);
            text.Release();

            finish++;
        }
        private void button3_Click(object sender, EventArgs e)
        {
            finish = 0;
            name = FileName.Text;
            Bitmap image = new Bitmap(Image.FromFile(name));
            timer1.Start();
            progressBar1.Maximum = 4 * 4 + 5 * 5 + 6 * 6;
            progressBar1.Value = 0;
            int width = image.Width;
            int height = image.Height;
            image.Dispose();
            // MessageBox.Show(ThreadPool.SetMaxThreads(8, 8).ToString());
            for (int amount = 4; amount <= 6; amount++)
            {
                int index = 1;
                Directory.CreateDirectory(SaveFold.Text + "\\" + amount + "X" + amount);
                for (int i = 0; i < amount; i++)
                {

                    //ThreadPool.UnsafeQueueUserWorkItem(new WaitCallback(ThreadMethod3), new Temp(index, 0, i * (height / amount), (width / amount), (height / amount), amount));
                    //index += 4;
                    for (int j = 0; j < amount; j++)
                    {
                        //Mat temp = new Mat(name);
                        //Mat text = new Mat(temp, new Rect(j * (width / amount), i * (height / amount), (width / amount), (height / amount)));
                        //temp.Dispose();
                        //string pathT = path + "\\" + amount + "X" + amount + "\\" + index + ".png";
                        // text.SaveImage(pathT);
                        //text.Dispose();
                        ThreadPool.UnsafeQueueUserWorkItem(new WaitCallback(ThreadMethod1), new Temp(index, j * (width / amount), i * (height / amount), (width / amount), (height / amount), amount));
                        //progressBar1.Value++;
                        index++;
                    }
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            progressBar1.Value = finish;
        }
    }
}
