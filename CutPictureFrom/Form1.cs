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
            public Bitmap bitmap;
            public int index;
            public int i;
            public int j;
            public int amount;
            public Temp(Bitmap bitmap,int index,int i,int j,int amount)
            {
                this.bitmap = bitmap;
                this.index = index;
                this.i = i;
                this.j = j;
                this.amount = amount;
            }
        }
        public static void ThreadMethod1(object obj)
        {
            FileStream fs = new FileStream(name, FileMode.Open, FileAccess.Read);
            Image image = Image.FromStream(fs);
            fs.Close();
            fs.Dispose();

            Bitmap bitmap = new Bitmap(image);
            Temp t = obj as Temp;

            image.Dispose();
            fs.Close();
            fs.Dispose();

            int index = t.index;
            int i = t.i;
            int j = t.j;
            int amount = t.amount;
            Bitmap temp = new Bitmap(bitmap.Width / amount, bitmap.Height / amount);
            int t1 = 0;
            int t2 = 0;
            for (int x = j * (bitmap.Width / amount); x < j * (bitmap.Width / amount) + bitmap.Width / amount; x++)
            {
                t2 = 0;
                for (int y = i * (bitmap.Height / amount); y < i * (bitmap.Height / amount) + bitmap.Height / amount; y++)
                {
                    temp.SetPixel(t1, t2, bitmap.GetPixel(x, y));
                    t2++;
                }
                t1++;
            }

            temp.Save(path + "\\" + amount + "X" + amount + "\\" + (index + 1) + ".png", ImageFormat.Png);
            temp.Dispose();
            bitmap.Dispose();

            finish++;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            finish = 0;
            name = FileName.Text;
            timer1.Start();
            progressBar1.Maximum = 4 * 4 + 5 * 5 + 6 * 6;//决定最大切割网格
            progressBar1.Value = 0;
            for (int amount = 4; amount <= 6; amount++)//修改也记得改这里
            {
                int index = 0;
                Directory.CreateDirectory(SaveFold.Text + "\\" + amount + "X" + amount);
                for (int i = 0; i < amount; i++)
                {
                    for (int j = 0; j < amount; j++)
                    {
                        try
                        {
                            ThreadPool.QueueUserWorkItem(new WaitCallback(ThreadMethod1), new Temp(null, index, i, j, amount));
                            index++;

                        }
                        catch (Exception ex)
                        {
                            j--;
                        }
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
