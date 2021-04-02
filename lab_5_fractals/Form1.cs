using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Windows.Forms;


namespace lab_5_fractals
{
    public partial class Form1 : Form
    {

        const int cellSize = 10;
        int celllInLine, cellInCol;
        int cellCount;
        float[,,] u0, u, b;
        int w, h;
        const int delta = 4;
        //double[] xi = new double[delta];
        //double[] yi = new double[delta];
        float[,] vol;
        float[] area;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string path = "1.jpg";
            Bitmap img = new Bitmap(path);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.Image = img;
            h = img.Height;
            w = img.Width;
            celllInLine = w / cellSize;
            cellInCol = h / cellSize;
            cellCount = celllInLine * cellInCol;
            u0 = new float[cellCount, cellSize, cellSize];
            u = new float[cellCount, cellSize, cellSize];
            b = new float[cellCount, cellSize, cellSize];

            vol = new float[cellCount, delta];
            area = new float[cellCount];
            setU0(img);
            float s;
            int d;
            //calc u and b 2 times
            for (d = 1; d <= delta; d++)
            {
                calcU(d);
                calcB(d);
                for (int k = 0; k < cellCount; k++)
                {
                    s = 0;
                    for (int i = cellSize - 1; i >= 0; i--)
                    {
                        for (int j = cellSize - 1; j >= 0; j--)
                        {
                            s += u[k, i, j] - b[k, i, j];
                        }
                    }
                    vol[k, d - 1] = s;
                }

            }
            d = delta - 1;
            s = 0f;
            for (int k = 0; k < cellCount; k++)
            {
                area[k] = (vol[k, d] - vol[k, d - 1]) / 2;
                s += area[k];
            }
            s /= cellCount;
            for (int k = 0; k < cellCount; k++)
            {
                try { 
                    if (area[k] < s)
                    {
                        for (int i = cellSize - 1; i >= 0; i--)
                        {
                            for (int j = cellSize - 1; j >= 0; j--)
                            {
                                img.SetPixel(i + (cellSize * (k % celllInLine)), j + (cellSize * (k / celllInLine)), Color.White);
                            }
                        }
                    }
                    else
                    {
                        for (int i = cellSize - 1; i >= 0; i--)
                        {
                            for (int j = cellSize - 1; j >= 0; j--)
                            {
                                img.SetPixel(i + (cellSize * (k % celllInLine)), j + (cellSize * (k / celllInLine)), Color.Black);
                            }
                        }
                    }
                } catch (Exception exception) {

                }
            }
            img.Save("result.png");
            string path2 = "result.png";
            Bitmap img2 = new Bitmap(path2);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.Image = img2;
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void setU0(Bitmap img)
        {
            for (int k = 0; k < cellCount; k++)
            {
                for (int i = cellSize - 1; i >= 0; i--)
                {
                    for (int j = cellSize - 1; j >= 0; j--)
                    {
                        u0[k, i, j] = getIntense(img.GetPixel(i + (cellSize * (k % celllInLine)), j + (cellSize * (k / celllInLine))));
                    }
                }
            }

            return;
        }
        //верхняя поверхность uδ(i,j)
        private void calcU(int delta)
        {
            if (delta > 1)
            {
                for (int k = 0; k < cellCount; k++)
                {
                    for (int i = cellSize - 1; i >= 0; i--)
                    {
                        for (int j = cellSize - 1; j >= 0; j--)
                        {
                            u0[k, i, j] = u[k, i, j];
                        }
                    }
                }
            }
            for (int k = 0; k < cellCount; k++)
            {
                for (int i = cellSize - 1; i >= 0; i--)
                {
                    for (int j = cellSize - 1; j >= 0; j--)
                    {
                        u[k, i, j] = Math.Max(
                            u0[k, i, j] + 1,
                            Math.Max(
                                Math.Max(
                                    i > 0 ? u0[k, i - 1, j] : 0,
                                    j > 0 ? u0[k, i, j - 1] : 0
                                    ),
                                Math.Max(
                                    i < cellSize - 1 ? u0[k, i + 1, j] : 0,
                                    j < cellSize - 1 ? u0[k, i, j + 1] : 0
                                    )
                                )
                            );
                    }
                }
            }
            return;
        }

        //нижняя поверхность uδ(i,j)
        private void calcB(int delta)
        {
            if (delta > 1)
            {
                for (int k = 0; k < cellCount; k++)
                {
                    for (int i = cellSize - 1; i >= 0; i--)
                    {
                        for (int j = cellSize - 1; j >= 0; j--)
                        {
                            u0[k, i, j] = b[k, i, j];
                        }
                    }
                }
            }
            for (int k = 0; k < cellCount; k++)
            {
                for (int i = cellSize - 1; i >= 0; i--)
                {
                    for (int j = cellSize - 1; j >= 0; j--)
                    {
                        b[k, i, j] = Math.Min(
                            u0[k, i, j] - 1,
                            Math.Min(
                                Math.Min(
                                    i > 0 ? u0[k, i - 1, j] : 256,
                                    j > 0 ? u0[k, i, j - 1] : 256
                                    ),
                                Math.Min(
                                    i < cellSize - 1 ? u0[k, i + 1, j] : 256,
                                    j < cellSize - 1 ? u0[k, i, j + 1] : 256
                                    )
                                )
                            );
                    }
                }
            }
            return;
        }

        private float getIntense(Color color)
        {
            return (color.R + color.G + color.B) / 3;
        }
    }
}
