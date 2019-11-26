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
        Stopwatch stopwatch = new Stopwatch();

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
            stopwatch.Start();
            // Load image and convert it to grayscale
            Bitmap image =  new Bitmap("C:\\Users\\Tomifko\\Desktop\\SIFT-MATLAB-master\\images\\bolt.jpg");
            Bitmap grayscaleImg = MakeGrayscale(image);

            // List of cells.
            List<Bitmap> listOfCells = GetCells(grayscaleImg);
            List<double[]> listOfHistograms = new List<double[]>();
            
            // List of descriptors.
            List<Bitmap> listOfDescriptors = new List<Bitmap>();
            //Bitmap descriptor = new Bitmap("C:\\Users\\Tomifko\\Desktop\\SIFT-MATLAB-master\\images\\desc.jpg");

            // Creates a list of histograms for each cell.
            foreach (Bitmap cell in listOfCells)
            {
                double[] hist = GetHistForEveryCell(cell);
                listOfHistograms.Add(hist);

                //Find index of max value in array
                double maxValue = hist.Max();
                int maxIndex = hist.ToList().IndexOf(maxValue);

                listOfDescriptors.Add(VisualizeDescriptor(maxIndex, cell));
            }



            //descriptor.SetPixel(7, 7, Color.Black);
            //descriptor.SetResolution(96, 96);
            //DrawHorizontalLine(descriptor);
            //listOfCells[27].Save("C:\\Users\\Tomifko\\Desktop\\SIFT-MATLAB-master\\images\\chichi.jpg");
            //pictureBox1.Image = listOfCells[13];
            //pictureBox2.Image = image;
            //pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;

            Bitmap result = RecreateBitmapFromCells(listOfDescriptors, grayscaleImg);
            pictureBox2.Image = result;

            result.Save(("C:\\Users\\Tomifko\\Desktop\\SIFT-MATLAB-master\\images\\chichi.jpg"));
            var time = stopwatch.Elapsed;
            Debug.Print(time.ToString());
        }

        private Bitmap VisualizeDescriptor(int index, Bitmap bmp)
        {
            Bitmap tempBmp = null;

            if (index < 2 || index > 32) tempBmp = DrawVerticalLine(bmp);
            if (index > 13 && index < 20) tempBmp = DrawVerticalLine(bmp);
            
            if (index > 5 && index < 11) tempBmp = DrawHorizontalLine(bmp);
            if (index > 23 && index < 29) tempBmp = DrawHorizontalLine(bmp);

            if (index >= 2 && index < 6) tempBmp = DrawLineFromLeftDownToRightUp(bmp);
            if (index >= 20 && index < 24) tempBmp = DrawLineFromLeftDownToRightUp(bmp);

            if (index >= 11 && index < 14) tempBmp = DrawLineFromLeftUpToRightDown(bmp);
            if (index >= 29 && index < 33) tempBmp = DrawLineFromLeftUpToRightDown(bmp);

            return tempBmp;
        }

        private Bitmap DrawLineFromLeftDownToRightUp(Bitmap bmp)
        {
            for (int i = 0; i < 8; i++)
            {
                bmp.SetPixel(i, bmp.Width - i - 1, Color.White);
            }
            return bmp;
        }

        private Bitmap DrawLineFromLeftUpToRightDown(Bitmap bmp)
        {
            for (int i = 0; i < 8; i++)
            {
                bmp.SetPixel(i, i, Color.White);
            }
            return bmp;
        }

        private Bitmap DrawHorizontalLine(Bitmap bmp)
        {
            for (int i = 0; i < 8; i++)
            {
                bmp.SetPixel(i, 4, Color.White);
            }
            return bmp;
        }

        private Bitmap DrawVerticalLine(Bitmap bmp)
        {
            for (int i = 0; i < 8; i++)
            {
                bmp.SetPixel(4, i, Color.White);
            }
            return bmp;
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

        private Bitmap RecreateBitmapFromCells(List<Bitmap> cells, Bitmap originalImg)
        {
            Bitmap finalBitmap = new Bitmap(originalImg.Width, originalImg.Height);
                                //8
            for (int i = 0; i < originalImg.Width / 8; i++)
            {                       //16
                for (int j = 0; j < originalImg.Height / 8; j++)
                {
                    for (int x = 0; x < 8; x++)
                    {
                        for (int y = 0; y < 8; y++)
                        {
                            finalBitmap.SetPixel(x + (i * 8), y + (j * 8), cells[(i * 16) + j].GetPixel(x, y));
                        }
                    }
                }
            }

            return finalBitmap;
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
