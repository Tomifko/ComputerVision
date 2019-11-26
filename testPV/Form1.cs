using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace testPV
{
    public partial class Form1 : Form
    {
        // Array used to store values of "histogram"
        double[] bins = new double[36];

        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Main of function.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            // Load image and convert it to grayscale
            Bitmap image =  new Bitmap("C:\\Users\\Tomifko\\Desktop\\SIFT-MATLAB-master\\images\\a.png");
            Bitmap grayscaleImg = MakeGrayscale(image);

            // List of cells.
            List<Bitmap> listOfCells = GetCells(grayscaleImg);
            List<double[]> listOfHistograms = new List<double[]>();

            // Creates a list of histograms for each cell.
            foreach (Bitmap cell in listOfCells)
            {
                listOfHistograms.Add(GetHistForEveryCell(cell));
            }

            Bitmap descriptor = new Bitmap(8, 8);
            pictureBox1.Image = listOfCells[55];

            for (int i = 0; i < 8; i++)
            {
                listOfCells[55].GetPixel(i, 4).R = 5;

                listOfCells[55].SetPixel(i, 4, listOfCells[55].GetPixel(i, 4).GetBrightness() + 10);
            }

            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
        }

        /// <summary>
        /// Create "histogram" for every cell.
        /// </summary>
        /// <param name="cell"></param>
        /// <returns >Returns array which represents histogram. </returns>
        private double[] GetHistForEveryCell(Bitmap cell)
        {
            bins = new double[36];

            for (int i = 1; i < cell.Width - 1; i++)
            {
                for (int j = 1; j < cell.Height - 1; j++)
                {
                    int x1 = GetBrightness(cell.GetPixel(i + 1, j));
                    int x2 = GetBrightness(cell.GetPixel(i - 1, j));
                    int y1 = GetBrightness(cell.GetPixel(i, j + 1));
                    int y2 = GetBrightness(cell.GetPixel(i, j - 1));

                    int Gx = GetGradient(x1, x2);
                    int Gy = GetGradient(y1, y2);

                    double magnitude = GetMagnitude(Gx, Gy);
                    double orientation = GetOrientation(Gx, Gy) + 180;              // Len aby, len aby !!!!!!!!!!!!!!!!!!!

                    if (double.IsNaN(orientation)) orientation = 0;
                    CreateHist(magnitude, orientation);
                }
            }
            return bins;
        }

        /// <summary>
        /// Divide input image to separate cells.
        /// </summary>
        /// <param name="image"></param>
        /// <returns> List of cells </returns>
        private List<Bitmap> GetCells(Bitmap image)
        {
            List<Bitmap> listOfCells = new List<Bitmap>();

            Bitmap tempMap = new Bitmap(8, 8);
                                //8 -> 0-7 v cykle
            for (int i = 0; i < image.Width / 8; i++)
            {                       //16
                for (int j = 0; j < image.Height / 8; j++)
                {
                    for (int x = 0; x < 8; x++)
                    {
                        for (int y = 0; y < 8; y++)
                        {
                            tempMap.SetPixel(x, y, image.GetPixel((i * 8) + x, (j * 8) + y));
                        }
                    }
                    listOfCells.Add(tempMap);
                    tempMap = new Bitmap(8, 8);
                }
            }
            tempMap.Dispose();
            return listOfCells;
        }

        /// <summary>
        /// Create "histogram" from magnitude and orientation values.
        /// </summary>
        /// <param name="mag"></param>
        /// <param name="ori"></param>
        private void CreateHist(double mag, double ori)
        {
            for (int i = 0; i < 36; i++)
            {
                if ((i * 10) <= ori && ori < (i * 10) + 10) bins[i] += mag;
            }
            //return bins;
        }

        /// <summary>
        /// Get orientation of pixel based on given gradients.
        /// </summary>
        /// <param name="Gx"></param>
        /// <param name="Gy"></param>
        /// <returns> Orientation </returns>
        public double GetOrientation(int Gx, int Gy)
        {
            return (Math.Atan2(Gy, Gx) * (180 / Math.PI));
        }

        /// <summary>
        /// Get magnitude of pixel based on given gradients.
        /// </summary>
        /// <param name="Gx"></param>
        /// <param name="Gy"></param>
        /// <returns> Magnitude </returns>
        public double GetMagnitude(int Gx, int Gy)
        {
            return Math.Sqrt(Math.Pow(Gx, 2) + Math.Pow(Gy, 2));
        }

        /// <summary>
        /// Compute gradient for pixel based on values next to given pixel.
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <returns> Gradient </returns>
        public int GetGradient(int value1, int value2)
        {
            return value1 - value2;
        }

        /// <summary>
        /// Use formula to compute brightness from image.
        /// </summary>
        /// <param name="color"></param>
        /// <returns> Brightness (0-255) </returns>
        public int GetBrightness(Color color)
        {
            return (int)Math.Round((0.2126 * color.R + 0.7152 * color.G + 0.0722 * color.B));
        }

        /// <summary>
        /// Converts image to grayscale.
        /// </summary>
        /// <param name="original"></param>
        /// <returns> Gray image </returns>
        public static Bitmap MakeGrayscale(Bitmap original)
        {
            //create a blank bitmap the same size as original
            Bitmap newBitmap = new Bitmap(original.Width, original.Height);

            //get a graphics object from the new image
            using (Graphics g = Graphics.FromImage(newBitmap))
            {

                //create the grayscale ColorMatrix
                ColorMatrix colorMatrix = new ColorMatrix(
                   new float[][]
                   {
                       new float[] {.3f, .3f, .3f, 0, 0},
                       new float[] {.59f, .59f, .59f, 0, 0},
                       new float[] {.11f, .11f, .11f, 0, 0},
                       new float[] {0, 0, 0, 1, 0},
                       new float[] {0, 0, 0, 0, 1}
                   });

                //create some image attributes
                using (ImageAttributes attributes = new ImageAttributes())
                {

                    //set the color matrix attribute
                    attributes.SetColorMatrix(colorMatrix);

                    //draw the original image on the new image
                    //using the grayscale color matrix
                    g.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height),
                                0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);
                }
            }
            return newBitmap;
        }
    }
}
