using System;
using System.Windows;
using System.Windows.Input;
using System.ComponentModel;
using System.Windows.Interop;
using System.Runtime.InteropServices;

using OpenCvSharp;
using Point = OpenCvSharp.Point;
using Size = OpenCvSharp.Size;
using Rect = OpenCvSharp.Rect;

using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;

using MessageBox = System.Windows.MessageBox;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace poe_archnemesis_acs
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {

        #region Hotkey Event Listener
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private const int HOTKEY_ID = 9000;

        //Modifiers:
        private const uint MOD_NONE = 0x0000; //(none)
        private const uint MOD_ALT = 0x0001; //ALT
        private const uint MOD_CONTROL = 0x0002; //CTRL
        private const uint MOD_SHIFT = 0x0004; //SHIFT
        private const uint MOD_WIN = 0x0008; //WINDOWS
        //CAPS LOCK:
        private const uint VK_OEM_3 = 0xC0;

        const int WM_HOTKEY = 0x0312;

        private IntPtr _windowHandle;
        private HwndSource _source;

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            _windowHandle = new WindowInteropHelper(this).Handle;
            _source = HwndSource.FromHwnd(_windowHandle);
            _source.AddHook(HwndHook);

            RegisterHotKey(_windowHandle, HOTKEY_ID, MOD_SHIFT, VK_OEM_3); //SHIFT + ~
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case WM_HOTKEY:
                    switch (wParam.ToInt32())
                    {
                        case HOTKEY_ID:
                            int vkey = (((int)lParam >> 16) & 0xFFFF);
                            if (vkey == VK_OEM_3)
                            {
                                if (this.ShowActivated == false)
                                {
                                    //For startup
                                    Screenshot();
                                    MatchImage();
                                    this.WindowState = WindowState.Maximized;
                                    this.ShowActivated = true;
                                    this.Show();
                                }
                                else
                                {
                                    if (this.Visibility == Visibility.Hidden)
                                    {
                                        MatchImage();
                                        this.Show();
                                    }
                                    else
                                    {
                                        this.Hide();
                                    }
                                }
                            }
                            handled = true;
                            break;
                    }
                    break;
            }
            return IntPtr.Zero;
        }
        #endregion

        public MainWindow()
        {
            InitializeComponent();

            Width = 0;
            Height = 0;
            ShowInTaskbar = false;
            ShowActivated = false;

            this.Show();
        }

        #region MainWindow Event Listeners
        //Event Listener for KeyDown when MainWindow is focused
        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            //if (Keyboard.IsKeyDown(Key.LeftShift) && e.Key == Key.OemTilde)
            if (e.Key == Key.Z)
            {
                MessageBox.Show("yes..");
            }
        }

        //EventListener for Closing when closing MainWindow/Exiting Application
        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            MessageBox.Show("Closing called");
        }
        #endregion

        #region Image Matching

        private void MatchImage()
        {
            using (Mat reference = new Mat("..\\..\\Resources\\Capture.jpg"))
            using (Mat template = new Mat("..\\..\\Resources\\mirror-image.png"))
            using (Mat results = new Mat(reference.Rows - template.Rows + 1, reference.Cols - template.Cols + 1, MatType.CV_32FC1))
            {
                //Mat result = DrawMatches.Draw(modelImage, observedImage, out matchTime);

                Mat gref = reference.CvtColor(ColorConversionCodes.BGR2RGB);
                Mat gtpl = template.CvtColor(ColorConversionCodes.BGR2RGB);

                Cv2.MatchTemplate(gref, gtpl, results, TemplateMatchModes.CCorrNormed);
                Cv2.Threshold(results, results, 0.85, 1.0, ThresholdTypes.Tozero);

                while (true)
                {
                    double minval, maxval, threshold = 0.85;
                    Point minloc, maxloc;
                    Cv2.MinMaxLoc(results, out minval, out maxval, out minloc, out maxloc);


                    if (maxval >= threshold)
                    {
                        //Random colors for match
                        Random rValue = new Random();

                        //Setup the rectangle to draw
                        Rect r = new Rect(new Point(maxloc.X, maxloc.Y), new Size(template.Width, template.Height));

                        //Draw a rectangle of the matching area
                        Cv2.Rectangle(reference, r, Scalar.FromRgb(rValue.Next(0, 255), rValue.Next(0, 255), rValue.Next(0,255)), 2);

                        //Fill in the res Mat so you don't find the same area again in the MinMaxLoc
                        Rect outRect;
                        Cv2.FloodFill(results, maxloc, new Scalar(0), out outRect, new Scalar(0.1), new Scalar(1.0));
                    }
                    else
                        break;
                }

                Cv2.ImShow("Matches", reference);
                Cv2.WaitKey(5);

            }

        }

        private void Screenshot()
        {
            int screenLeft = SystemInformation.VirtualScreen.Left;
            int screenTop = SystemInformation.VirtualScreen.Top;
            int screenWidth = SystemInformation.VirtualScreen.Width;
            int screenHeight = SystemInformation.VirtualScreen.Height;

            // Create a bitmap of the appropriate size to receive the full-screen screenshot.
            using (Bitmap bitmap = new Bitmap(screenWidth, screenHeight))
            {
                // Draw the screenshot into our bitmap.
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.CopyFromScreen(screenLeft, screenTop, 0, 0, bitmap.Size);
                }

                //Save the screenshot as a Jpg image
                var uniqueFileName = "..\\..\\Resources\\Capture.jpg";
                try
                {
                    bitmap.Save(uniqueFileName, ImageFormat.Jpeg);
                }
                catch (Exception ex)
                {
                }
            }

        }

        #endregion
    }
}
