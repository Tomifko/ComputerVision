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
        #region Variables
        // Array used to store values of "histogram"
        double[] bins = new double[36];
        // Stopwatch used to measure times.
        Stopwatch stopwatch = new Stopwatch();
        Stopwatch totalStopwatch = new Stopwatch();
        // Value in which will be temporarily stored bmp of descriptor visualization.
        Bitmap _arrow = null;
        // List which contains every descriptor visualisation.
        List<Bitmap> arrows = new List<Bitmap>();

        #endregion

        #region Form initialization
        public Form1()
        {
            InitializeComponent();
        }
        #endregion

        #region Main
        /// <summary>
        /// Main of function.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            List<TimeSpan> timeSpans = new List<TimeSpan>();
            totalStopwatch.Start();
            stopwatch.Start();
            for (int i = 0; i < 1000; i++)
            {
                pictureBox1.Image = null;
                pictureBox2.Image = null;
                pictureBox3.Image = null;
                arrows = new List<Bitmap>();

                stopwatch.Restart();
                // Load image and convert it to grayscale
                Bitmap image = new Bitmap("C:\\Users\\Tomifko\\Desktop\\PV\\source\\bolt.jpg");
                pictureBox1.Image = image;
                Bitmap grayscaleImg = MakeGrayscale(image);

                // Create list of cells == divide image to separate parts.
                List<Bitmap> listOfCells = GetCells(grayscaleImg);
                List<double[]> listOfHistograms = new List<double[]>();

                // Create list of descriptors == used to store cells with visualised descriptors.
                List<Bitmap> listOfDescriptors = new List<Bitmap>();
                // Create list of descriptors represented with arrays.
                List<Bitmap> listOfArrayDescriptors = new List<Bitmap>();

                // Creates a list of histograms for each cell.
                foreach (Bitmap cell in listOfCells)
                    {
                        double[] hist = GetHistForEveryCell(cell);
                        listOfHistograms.Add(hist);

                        //Find index of max value in array and index of it.
                        double maxValue = hist.Max();
                        int maxIndex = hist.ToList().IndexOf(maxValue);
                    
                        listOfDescriptors.Add(VisualizeDescriptor(maxIndex, cell));
                        Bitmap tmpBmp = VisualizeDescriptor(hist);
                        arrows.Add(tmpBmp);
                        Bitmap resizedArrayDescriptor = new Bitmap(tmpBmp, new Size(8, 8));
                        listOfArrayDescriptors.Add(resizedArrayDescriptor);

                    }

                // Connect cells to one final image.
                Bitmap result = RecreateBitmapFromCells(listOfDescriptors, grayscaleImg);
                pictureBox2.Image = result;

                // Save final image to computer.
                result.Save((@"C:\\Users\\Tomifko\\Desktop\\PV\\source\\result.jpg"));

                // Measure time passed from beginning of program and add this value to list.
                // List is used to store 1000 of this values.
                timeSpans.Add(stopwatch.Elapsed);
            }
            // Total time of 1000 measurements.
            TimeSpan totalTime = totalStopwatch.Elapsed;

            // After 1000 measurements calculate average value of time passed and write it to .txt file.
            using (System.IO.StreamWriter file =
                new System.IO.StreamWriter(@"C:\\Users\\Tomifko\\Desktop\\Measurements.txt", true))
            {
                file.WriteLine("Average time: " + AverageTime.Average(timeSpans));
                file.WriteLine("Total time: " + totalTime);
            }

            // Write every value to .txt file.
            foreach (TimeSpan times in timeSpans)
            {
                using (System.IO.StreamWriter file =
                    new System.IO.StreamWriter(@"C:\\Users\\Tomifko\\Desktop\\Measurements.txt", true))
                {
                    file.WriteLine(times);
                }
            }

        }
        #endregion

        #region Line drawing
        /// <summary>
        /// Draw line from left down corner to right up corner.
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns> Input image with requested line added. </returns>
        private Bitmap DrawLineFromLeftDownToRightUp(Bitmap bmp)
        {
            for (int i = 0; i < 8; i++)
            {
                bmp.SetPixel(i, bmp.Width - i - 1, Color.White);
            }
            return bmp;
        }

        /// <summary>
        /// Draw line from left up corner to rigth down corner.
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns> Input image with requested line added. </returns>
        private Bitmap DrawLineFromLeftUpToRightDown(Bitmap bmp)
        {
            for (int i = 0; i < 8; i++)
            {
                bmp.SetPixel(i, i, Color.White);
            }
            return bmp;
        }

        /// <summary>
        /// Draws horizontal line across image.
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns> Input image with requested line added. </returns>
        private Bitmap DrawVerticalLine(Bitmap bmp)
        {
            for (int i = 0; i < 8; i++)
            {
                bmp.SetPixel(i, 4, Color.White);
            }
            return bmp;
        }

        /// <summary>
        /// Draws vertical line across image.
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns> Input image with requested line added. </returns>
        private Bitmap DrawHorizontalLine(Bitmap bmp)
        {
            for (int i = 0; i < 8; i++)
            {
                bmp.SetPixel(4, i, Color.White);
            }
            return bmp;
        }
        #endregion

        #region Cell operations
        /// <summary>
        /// Create "histogram" for every cell.
        /// </summary>
        /// <param name="cell"></param>
        /// <returns> Returns array which represents histogram. </returns>
        private double[] GetHistForEveryCell(Bitmap cell)
        {
            // Reset this array every time this funcion is called.
            bins = new double[36];
            List<double> mag = new List<double>();
            List<double> ori = new List<double>();


            for (int i = 1; i < cell.Width - 1; i++)
            {
                for (int j = 1; j < cell.Height - 1; j++)
                {
                    // Calculate brightness of surrounding pixels (used to calculate gradients).
                    int x1 = GetBrightness(cell.GetPixel(i + 1, j));
                    int x2 = GetBrightness(cell.GetPixel(i - 1, j));
                    int y1 = GetBrightness(cell.GetPixel(i, j + 1));
                    int y2 = GetBrightness(cell.GetPixel(i, j - 1));

                    // Calculate gradients for both x and y directions.
                    int Gx = GetGradient(x1, x2);
                    int Gy = GetGradient(y1, y2);

                    // Calculate magnitude (brightness) and orientation of image.
                    double magnitude = GetMagnitude(Gx, Gy);
                    double orientation = GetOrientation(Gx, Gy) + 180;

                    mag.Add(magnitude);
                    ori.Add(orientation);
                    // Replace NaN values with zero.
                    if (double.IsNaN(orientation)) orientation = 1;

                    //Create histogram from given magnitude and orientation.
                    CreateHist(magnitude, orientation);
                }
            }
            return bins;
        }

        /// <summary>
        /// Divide input image to separate cells (diferent parts of image).
        /// </summary>
        /// <param name="image"></param>
        /// <returns> Image divided to separate parts. </returns>
        private List<Bitmap> GetCells(Bitmap image)
        {
            // Create list which will be used to store each cell.
            List<Bitmap> listOfCells = new List<Bitmap>();
            // Create temporary bitmap used to store current cell.
            Bitmap tempMap = new Bitmap(8, 8);

            for (int i = 0; i < image.Width / 8; i++)
            {
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
                    // Reset bitmap every time next cell is created
                    tempMap = new Bitmap(8, 8);
                }
            }
            // Dispose this object because it is no longer needed.
            tempMap.Dispose();
            return listOfCells;
        }

        /// <summary>
        /// Recreate image from cells with visualized descriptors.
        /// </summary>
        /// <param name="cells"></param>
        /// <param name="originalImg"></param>
        /// <returns></returns>
        private Bitmap RecreateBitmapFromCells(List<Bitmap> cells, Bitmap originalImg)
        {
            // Create value used to store final image.
            Bitmap finalBitmap = new Bitmap(originalImg.Width, originalImg.Height);

            for (int i = 0; i < originalImg.Width / 8; i++)
            {
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
        /// Visualize descriptor for every cell based on index of maximum value from histogram.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="bmp"></param>
        /// <returns> Image with visualized descriptor. </returns>
        private Bitmap VisualizeDescriptor(int index, Bitmap bmp)
        {
            Bitmap tempBmp = null;
            //if (index < 2 || index > 32) tempBmp = DrawHorizontalLine(bmp);//
            //if (index > 14 && index < 20) tempBmp = DrawHorizontalLine(bmp);//

            //if (index > 5 && index < 11) tempBmp = DrawVerticalLine(bmp);//
            //if (index > 23 && index < 29) tempBmp = DrawVerticalLine(bmp);

            //if (index >= 2 && index < 6) tempBmp = DrawLineFromLeftDownToRightUp(bmp);//
            //if (index >= 20 && index < 24) tempBmp = DrawLineFromLeftDownToRightUp(bmp);

            //if (index >= 11 && index < 15) tempBmp = DrawLineFromLeftUpToRightDown(bmp);
            //if (index >= 29 && index < 33) tempBmp = DrawLineFromLeftUpToRightDown(bmp);

            if (index < 2 || index > 32) tempBmp = DrawHorizontalLine(bmp);//
            if (index > 14 && index < 20) tempBmp = DrawHorizontalLine(bmp);//

            if (index >= 2 && index < 6) tempBmp = DrawLineFromLeftDownToRightUp(bmp);//
            if (index >= 20 && index < 24) tempBmp = DrawLineFromLeftDownToRightUp(bmp);

            if (index > 5 && index < 11) tempBmp = DrawVerticalLine(bmp);//
            if (index > 23 && index < 29) tempBmp = DrawVerticalLine(bmp);

            if (index >= 11 && index < 15) tempBmp = DrawLineFromLeftUpToRightDown(bmp);
            if (index >= 29 && index < 33) tempBmp = DrawLineFromLeftUpToRightDown(bmp);


            return tempBmp;
        }

        private Bitmap VisualizeDescriptor(double[] hist)
        {
            List<double> setsOfDegrees = new List<double>()
            {
                hist[6] + hist[7] + hist[8] + hist[9] + hist[10],//
                hist[2] + hist[3] + hist[4] + hist[5],
                hist[33] + hist[34] + hist[35] + hist[0] + hist[1],//
                hist[11] + hist[12] + hist[13] + hist[14],
                hist[24] + hist[25] + hist[26] + hist[27] + hist[28],//
                hist[20] + hist[21] + hist[22] + hist[23],
                hist[15] + hist[16] + hist[17] + hist[18] + hist[19],//
                hist[29] + hist[30] + hist[31] + hist[32]
            };

            double maxValue = setsOfDegrees.Max();
            int indexOfMax = setsOfDegrees.ToList().IndexOf(maxValue);

            // After this, max value should be 50 - max length of arrow
            List<double> lengthsOfArrows = new List<double>();
            foreach (double set in setsOfDegrees)
            {
                lengthsOfArrows.Add((set / setsOfDegrees[indexOfMax]) * 50);
            }


            Bitmap b = new Bitmap(this.pictureBox3.ClientSize.Width, this.pictureBox3.ClientSize.Height);

            using (Graphics g = Graphics.FromImage(b))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            }

            this.pictureBox3.Image = b;

            _arrow = new Bitmap(128, 128);

            using (Graphics g = Graphics.FromImage(_arrow))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                using (Pen p = new Pen(Color.Black, 1))
                {
                    System.Drawing.Drawing2D.AdjustableArrowCap l = new System.Drawing.Drawing2D.AdjustableArrowCap(3, 4);
                    p.CustomEndCap = l;

                    // Right        
                    g.DrawLine(p, new PointF(_arrow.Width / 2F, _arrow.Height / 2F), new PointF((_arrow.Width / 2F) + (float)lengthsOfArrows[0], _arrow.Height / 2F));
                    // RightUp
                    g.DrawLine(p, new PointF(_arrow.Width / 2F, _arrow.Height / 2F), new PointF((_arrow.Width / 2F) + (float)lengthsOfArrows[1], (_arrow.Height / 2F) - (float)lengthsOfArrows[1]));
                    // Up
                    g.DrawLine(p, new PointF(_arrow.Width / 2F, _arrow.Height / 2F), new PointF(_arrow.Width / 2F, (_arrow.Height / 2F) - (float)lengthsOfArrows[2]));
                    // LeftUp
                    g.DrawLine(p, new PointF(_arrow.Width / 2F, _arrow.Height / 2F), new PointF((_arrow.Width / 2F) - (float)lengthsOfArrows[3], (_arrow.Height / 2F) - (float)lengthsOfArrows[3]));
                    // Left
                    g.DrawLine(p, new PointF(_arrow.Width / 2F, _arrow.Height / 2F), new PointF((_arrow.Width / 2F) - (float)lengthsOfArrows[4], _arrow.Height / 2F));
                    // LeftDown
                    g.DrawLine(p, new PointF(_arrow.Width / 2F, _arrow.Height / 2F), new PointF((_arrow.Width / 2F) - (float)lengthsOfArrows[5], (_arrow.Height / 2F) + (float)lengthsOfArrows[5]));
                    // Down
                    g.DrawLine(p, new PointF(_arrow.Width / 2F, _arrow.Height / 2F), new PointF(_arrow.Width / 2F, (_arrow.Height / 2F) + (float)lengthsOfArrows[6]));
                    // RightDown
                    g.DrawLine(p, new PointF(_arrow.Width / 2F, _arrow.Height / 2F), new PointF((_arrow.Width / 2F) + (float)lengthsOfArrows[7], (_arrow.Height / 2F) + (float)lengthsOfArrows[7]));
                }
            }
            _arrow = new Bitmap(_arrow, new Size(40, 40));
            return _arrow;
        }


        #endregion

        #region Histogram
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
        }
        #endregion

        #region Calculate operations
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
        #endregion

        #region Grayscale operation
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
        #endregion

        private void pictureBox3_Paint(object sender, PaintEventArgs e)
        {
            if (_arrow != null)
            {
                e.Graphics.TranslateTransform(-this.pictureBox3.Image.Width / 2F, -this.pictureBox3.Image.Height / 2F);
                //e.Graphics.RotateTransform(_rot, System.Drawing.Drawing2D.MatrixOrder.Append);
                e.Graphics.TranslateTransform(this.pictureBox3.Image.Width / 2F, this.pictureBox3.Image.Height / 2F, System.Drawing.Drawing2D.MatrixOrder.Append);

                for (int i = 0; i < arrows.Count / 16; i++)
                {
                    for (int j = 0; j < arrows.Count / 8; j++)
                    {
                        Debug.Print(((i * 16) + j).ToString());
                        e.Graphics.DrawImage(arrows[(i * 16) + j], new PointF(i * 40, j * 40));

                    }
                }

                int numOfCells = 8;
                int cellSize = 40;
                Pen p = new Pen(Color.Black);

                for (int y = 0; y < numOfCells * 2 + 1; ++y)
                {
                    e.Graphics.DrawLine(p, 0, y * cellSize, numOfCells * cellSize, y * cellSize);
                }

                for (int x = 0; x < numOfCells + 1; ++x)
                {
                    e.Graphics.DrawLine(p, x * cellSize, 0, x * cellSize, numOfCells * cellSize * 2);
                }
            }
        }
    }

    #region Compute average time from measurements
    public static class AverageTime
    {
        /// <summary>
        /// Static function used to compute average time from measurements from list of TimeSpans.
        /// </summary>
        /// <param name="spans"></param>
        /// <returns> Average time. </returns>
        public static TimeSpan Average(this IEnumerable<TimeSpan> spans) => TimeSpan.FromSeconds(spans.Select(s => s.TotalSeconds).Average());
    }
    #endregion
}