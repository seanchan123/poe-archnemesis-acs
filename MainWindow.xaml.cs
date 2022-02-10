using System;
using System.Windows;
using System.Windows.Input;
using System.ComponentModel;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.UI;
using Emgu.CV.Structure;
using System.Collections.Generic;
using System.Drawing;
using Emgu.CV.Bitmap;

namespace poe_archnemesis_acs
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
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
                                    this.WindowState = WindowState.Maximized;
                                    this.ShowActivated = true;
                                    this.Show();
                                    MatchImage();
                                }
                                else
                                {
                                    if (this.Visibility == Visibility.Hidden)
                                    {
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

        long matchTime;

        private void MatchImage()
        {
            //using (Mat modelImage = CvInvoke.Imread("..\\..\\Resources\\mirror-image.png", ImreadModes.Color))
            //using (Mat observedImage = CvInvoke.Imread("..\\..\\Resources\\Capture.JPG", ImreadModes.Color))
            //{
            //    Mat result = DrawMatches.Draw(modelImage, observedImage, out matchTime);
            //    ImageViewer.Show(result, String.Format("Matched in {0} milliseconds", matchTime));
            //}

            Dictionary<string, Image<Bgr, byte>> imgList = new Dictionary<string, Image<Bgr, byte>>();
            var observedImage = CvInvoke.Imread("..\\..\\Resources\\Capture.JPG", ImreadModes.Color).Bitmap;

            try
            {
                var imgScene = imgList["..\\..\\Resources\\mirror-image.png"].Clone();
                IInputArray template = new Bitmap(observedImage).ToImage<Bgr, byte>();

                Mat imgOut = new Mat();
                CvInvoke.MatchTemplate(imgScene, template, imgOut, Emgu.CV.CvEnum.TemplateMatchingType.Sqdiff);

                Mat imgOutNorm = new Mat();

                CvInvoke.Normalize(imgOut, imgOutNorm, 0, 1, Emgu.CV.CvEnum.NormType.MinMax, Emgu.CV.CvEnum.DepthType.Cv64F);

                Matrix<double> matches = new Matrix<double>(imgOutNorm.Size);
                imgOutNorm.CopyTo(matches);

                double minValue = 0, maxVal = 0;
                System.Drawing.Point minLoc = new System.Drawing.Point();
                System.Drawing.Point maxLoc = new System.Drawing.Point();

                do
                {
                    CvInvoke.MinMaxLoc(matches, ref minValue, ref maxVal, ref minLoc, ref maxLoc);
                    Rectangle r = new Rectangle(minLoc, template.Size);
                    CvInvoke.Rectangle(imgScene, r, new MCvScalar(255, 0, 0), 1);

                    matches[minLoc.Y, minLoc.X] = 0.5;
                    matches[maxLoc.Y, maxLoc.X] = 0.5;
                } while (minValue <= 0.80);

                ImageViewer.Show(imgScene);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #endregion
    }
}
