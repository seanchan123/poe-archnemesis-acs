using System;
using System.IO;
using System.Text;
using System.Drawing;
using System.Windows;
using System.Diagnostics;
using System.Windows.Forms;
using System.ComponentModel;
using System.Windows.Interop;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;

using OpenCvSharp;
using Point = OpenCvSharp.Point;
using Size = OpenCvSharp.Size;
using Rect = OpenCvSharp.Rect;

using MessageBox = System.Windows.MessageBox;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace poe_archnemesis_acs
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        #region Global Variables

        //Coallate all archnemesis mod names
        List<string> modNames = new List<string>();

        #endregion


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
        //TILDE (~):
        private const uint VK_OEM_3 = 0xC0;
        //A KEY
        private const uint VK_AKEY = 0x41;
        //S KEY
        private const uint VK_SKEY = 0x53;

        const int WM_HOTKEY = 0x0312;

        private IntPtr _windowHandle;
        private HwndSource _source;

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            _windowHandle = new WindowInteropHelper(this).Handle;
            _source = HwndSource.FromHwnd(_windowHandle);
            _source.AddHook(HwndHook);

            RegisterHotKey(_windowHandle, HOTKEY_ID, MOD_CONTROL + MOD_SHIFT, VK_AKEY); //CTRL + LSHIFT + A
            RegisterHotKey(_windowHandle, HOTKEY_ID, MOD_CONTROL + MOD_SHIFT, VK_SKEY); //CTRL + LSHIFT + S
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
                            if (vkey == VK_AKEY)
                            {
                                //SHOW
                                if (this.Visibility == Visibility.Hidden)
                                {
                                    loadingGrid.Opacity = 0;
                                    cheatsheetGrid.Opacity = 1;

                                    ShowOverlay();

                                    modSearchTextBox.Focus();
                                }
                                else
                                {
                                    loadingGrid.Opacity = 1;
                                    cheatsheetGrid.Opacity = 0;
                                    this.Hide();
                                }
                            }
                            else if (vkey == VK_SKEY)
                            {
                                //SCAN
                                if (this.Visibility == Visibility.Hidden)
                                {
                                    Screenshot();

                                    loadingGrid.Opacity = 1;
                                    cheatsheetGrid.Opacity = 0;

                                    ShowOverlay();
                                    ReturnLabelToDefault();
                                    MatchImage();

                                    cheatsheetGrid.Opacity = 1;
                                    loadingGrid.Opacity = 0;
                                    modSearchTextBox.Focus();
                                }
                                else
                                {
                                    loadingGrid.Opacity = 1;
                                    cheatsheetGrid.Opacity = 0;
                                    this.Hide();
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

            //MainWindow properties
            Width = 0;
            Height = 0;
            ShowInTaskbar = false;
            ShowActivated = false;

            modNames.Add("mana-siphoner");
            modNames.Add("mirror-image");
            modNames.Add("empowered-elements");
            modNames.Add("storm-strider");
            modNames.Add("arcane-buffer");
            modNames.Add("bloodletter");
            modNames.Add("bonebreaker");
            modNames.Add("brine-king-touched");
            modNames.Add("corrupter");
            modNames.Add("drought-bringer");
            modNames.Add("effigy");
            modNames.Add("frenzied");
            modNames.Add("frost-strider");
            modNames.Add("frostweaver");
            modNames.Add("incendiary");
            modNames.Add("invulnerable");
            modNames.Add("juggernaut");
            modNames.Add("magma-barrier");
            modNames.Add("magma-barrier-2");
            modNames.Add("malediction");
            modNames.Add("overcharged");
            modNames.Add("permafrost");
            modNames.Add("rejuvenating");
            modNames.Add("shakari-touched");
            modNames.Add("stormweaver");
            modNames.Add("crystal-skinned");
            modNames.Add("entangler");
            modNames.Add("executioner");
            modNames.Add("necromancer");
            modNames.Add("gargantuan");
            modNames.Add("echoist");
            modNames.Add("flame-strider");
            modNames.Add("flame-strider-2");
            modNames.Add("hasted");
            modNames.Add("hasted-2");
            modNames.Add("heralding-minions");
            modNames.Add("sentinel");
            modNames.Add("soul-eater");
            modNames.Add("soul-eater-2");
            modNames.Add("steel-infused");
            modNames.Add("treant-horde");
            modNames.Add("deadeye");
            modNames.Add("berserker");
            modNames.Add("chaosweaver");
            modNames.Add("consecrator");
            modNames.Add("dynamo");
            modNames.Add("flameweaver");
            modNames.Add("soul-conduit");
            modNames.Add("toxic");
            modNames.Add("vampiric");
            modNames.Add("empowering-minions");
            modNames.Add("hexer");
            modNames.Add("temporal-bubble");
            modNames.Add("bombardier");
            modNames.Add("corpse-detonator");
            modNames.Add("evocationist");
            modNames.Add("tukohama-touched");
            modNames.Add("assassin");
            modNames.Add("arakaali-touched");
            modNames.Add("opulent");
            modNames.Add("lunaris-touched");
            modNames.Add("solaris-touched");
            modNames.Add("innocence-touched");
            modNames.Add("ice-prison");

            //Show() to allow global event listener to work, Hide() to hide it from view
            this.Show();
            this.Hide();
        }


        #region MainWindow Event Listeners
        //Event Listener for KeyDown when MainWindow is focused
        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            //if (Keyboard.IsKeyDown(Key.LeftShift) && e.Key == Key.OemTilde)
            //if (e.Key == Key.Z)
            //{
            //    MessageBox.Show("yes..");
            //}
        }

        //EventListener for Closing when closing MainWindow/Exiting Application
        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            MessageBox.Show("Closing called");
        }

        private void modSearchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            string searchText = modSearchTextBox.Text;

        }
        #endregion


        #region Image Matching
        private void MatchImage()
        {
            int totalCount = 0;

            //Store in List<ArchnemesisModModel> to fetch it more accurately
            List<ArchnemesisModModel> archnemesisMods = new List<ArchnemesisModModel>();

            foreach (string modName in modNames)
            {
                ArchnemesisModModel archnemesisMod = new ArchnemesisModModel();
                archnemesisMod.Name = modName;
                archnemesisMod.ImageSource = "..\\..\\Resources\\Mods\\" + modName + ".png";
                archnemesisMod.ControlName = FindControlName(modName);
                archnemesisMod.Count = 0;

                archnemesisMods.Add(archnemesisMod);
            }

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            using (Mat screenshot = new Mat("..\\..\\Resources\\Capture.jpg"))
            //using (Mat screenshot = new Mat("..\\..\\Resources\\Test1.png"))
            {
                foreach (ArchnemesisModModel archnemesisMod in archnemesisMods)
                {
                    using (Mat archnemesisLogo = new Mat(archnemesisMod.ImageSource))
                    using (Mat results = new Mat(screenshot.Rows - archnemesisLogo.Rows + 1, screenshot.Cols - archnemesisLogo.Cols + 1, MatType.CV_32FC1))
                    {
                        Cv2.MatchTemplate(screenshot, archnemesisLogo, results, TemplateMatchModes.CCorrNormed);
                        Cv2.Threshold(results, results, 0.98, 1.0, ThresholdTypes.Tozero);

                        //Random colors for match
                        Random randomValue = new Random();
                        int rValue = randomValue.Next(0, 255);
                        int gValue = randomValue.Next(0, 255);
                        int bValue = randomValue.Next(0, 255);

                        while (true)
                        {
                            double minval, maxval, threshold = 0.98;
                            Point minloc, maxloc;
                            Cv2.MinMaxLoc(results, out minval, out maxval, out minloc, out maxloc);

                            if (maxval >= threshold)
                            {

                                //Setup the rectangle to draw
                                Rect r = new Rect(new Point(maxloc.X, maxloc.Y), new Size(archnemesisLogo.Width, archnemesisLogo.Height));

                                //Draw a rectangle of the matching area
                                Cv2.Rectangle(screenshot, r, Scalar.FromRgb(rValue, gValue, bValue), 2);

                                //Fill in the res Mat so you don't find the same area again in the MinMaxLoc
                                Rect outRect;
                                Cv2.FloodFill(results, maxloc, new Scalar(0), out outRect, new Scalar(0.1), new Scalar(1.0));

                                //Add count if archnemesis mod exists
                                archnemesisMod.Count++;
                                totalCount++;
                            }
                            else
                                break;
                        }
                    }

                    BindModToHidden(archnemesisMod.ControlName, archnemesisMod.Count);
                }

                //Cv2.ImShow("Matches", screenshot);
                //Cv2.WaitKey();
            }

            stopwatch.Stop();
            File.Delete("..\\..\\Resources\\Capture.jpg");
            //MessageBox.Show(stopwatch.ElapsedMilliseconds.ToString());
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


        #region Cheatsheet View Methods

        public void BindModToHidden(string controlName, int totalCount)
        {
            switch (controlName)
            {
                case "manaSiphonerHidden":

                    if (totalCount != 0)
                    {
                        manaSiphonerHidden.Content = totalCount;
                        manaSiphonerLbl1.Opacity = 1.0;
                    }

                    break;

                case "mirrorImageHidden":

                    if (totalCount != 0)
                    {
                        mirrorImageHidden.Content = totalCount;
                        mirrorImageLbl1.Opacity = 1.0;
                    }

                    break;

                case "empoweredElementsHidden":

                    if (totalCount != 0)
                    {
                        empoweredElementsHidden.Content = totalCount;
                        empoweredElementsLbl1.Opacity = 1.0;
                    }

                    break;

                case "stormStriderHidden":

                    if (totalCount != 0)
                    {
                        stormStriderHidden.Content = totalCount;
                        stormStriderLbl1.Opacity = 1.0;
                    }

                    break;

                case "arcaneBufferHidden":

                    if (totalCount != 0)
                    {
                        arcaneBufferHidden.Content = totalCount;
                        arcaneBufferLbl1.Opacity = 1.0;
                        arcaneBufferLbl2.Opacity = 1.0;
                    }

                    break;

                case "bloodletterHidden":

                    if (totalCount != 0)
                    {
                        bloodletterHidden.Content = totalCount;
                        bloodletterLbl1.Opacity = 1.0;
                        bloodletterLbl2.Opacity = 1.0;
                        bloodletterLbl3.Opacity = 1.0;
                        bloodletterLbl4.Opacity = 1.0;
                    }

                    break;

                case "bonebreakerHidden":

                    if (totalCount != 0)
                    {
                        bonebreakerHidden.Content = totalCount;
                        bonebreakerLbl1.Opacity = 1.0;
                        bonebreakerLbl2.Opacity = 1.0;
                        bonebreakerLbl3.Opacity = 1.0;
                        bonebreakerLbl4.Opacity = 1.0;
                    }

                    break;

                case "brineKingTouchedHidden":

                    if (totalCount != 0)
                    {
                        brineKingTouchedHidden.Content = totalCount;
                        brineKingTouchedLbl1.Opacity = 1.0;
                    }

                    break;

                case "corrupterHidden":

                    if (totalCount != 0)
                    {
                        corrupterHidden.Content = totalCount;
                        corrupterLbl1.Opacity = 1.0;
                        corrupterLbl2.Opacity = 1.0;
                    }

                    break;

                case "droughtBringerHidden":

                    if (totalCount != 0)
                    {
                        droughtBringerHidden.Content = totalCount;
                        droughtBringerLbl1.Opacity = 1.0;
                    }

                    break;

                case "effigyHidden":

                    if (totalCount != 0)
                    {
                        effigyHidden.Content = totalCount;
                        effigyLbl1.Opacity = 1.0;
                    }

                    break;

                case "frenziedHidden":

                    if (totalCount != 0)
                    {
                        frenziedHidden.Content = totalCount;
                        frenziedLbl1.Opacity = 1.0;
                        frenziedLbl2.Opacity = 1.0;
                        frenziedLbl3.Opacity = 1.0;
                        frenziedLbl4.Opacity = 1.0;
                    }

                    break;

                case "frostStriderHidden":

                    if (totalCount != 0)
                    {
                        frostStriderHidden.Content = totalCount;
                        frostStriderLbl1.Opacity = 1.0;
                    }

                    break;

                case "frostweaverHidden":

                    if (totalCount != 0)
                    {
                        frostweaverHidden.Content = totalCount;
                        frostweaverLbl1.Opacity = 1.0;
                        frostweaverLbl2.Opacity = 1.0;
                    }

                    break;

                case "incendiaryHidden":

                    if (totalCount != 0)
                    {
                        incendiaryHidden.Content = totalCount;
                        incendiaryLbl1.Opacity = 1.0;
                        incendiaryLbl2.Opacity = 1.0;
                        incendiaryLbl3.Opacity = 1.0;
                    }

                    break;

                case "invulnerableHidden":

                    if (totalCount != 0)
                    {
                        invulnerableHidden.Content = totalCount;
                        invulnerableLbl1.Opacity = 1.0;
                        invulnerableLbl2.Opacity = 1.0;
                    }

                    break;

                case "juggernautHidden":

                    if (totalCount != 0)
                    {
                        juggernautHidden.Content = totalCount;
                        juggernautLbl1.Opacity = 1.0;
                        juggernautLbl2.Opacity = 1.0;
                        juggernautLbl3.Opacity = 1.0;
                    }

                    break;

                case "magmaBarrierHidden":

                    if (totalCount != 0)
                    {
                        magmaBarrierHidden.Content = totalCount;
                        magmaBarrierLbl1.Opacity = 1.0;
                        magmaBarrierLbl2.Opacity = 1.0;
                    }

                    break;

                case "maledictionHidden":

                    if (totalCount != 0)
                    {
                        maledictionHidden.Content = totalCount;
                        maledictionLbl1.Opacity = 1.0;
                        maledictionLbl2.Opacity = 1.0;
                    }

                    break;

                case "overchargedHidden":

                    if (totalCount != 0)
                    {
                        overchargedHidden.Content = totalCount;
                        overchargedLbl1.Opacity = 1.0;
                        overchargedLbl2.Opacity = 1.0;
                        overchargedLbl3.Opacity = 1.0;
                        overchargedLbl4.Opacity = 1.0;
                        overchargedLbl5.Opacity = 1.0;
                        overchargedLbl6.Opacity = 1.0;
                    }

                    break;

                case "permafrostHidden":

                    if (totalCount != 0)
                    {
                        permafrostHidden.Content = totalCount;
                        permafrostLbl1.Opacity = 1.0;
                        permafrostLbl2.Opacity = 1.0;
                    }

                    break;

                case "rejuvenatingHidden":

                    if (totalCount != 0)
                    {
                        rejuvenatingHidden.Content = totalCount;
                        rejuvenatingLbl1.Opacity = 1.0;
                        rejuvenatingLbl2.Opacity = 1.0;
                    }

                    break;

                case "shakariTouchedHidden":

                    if (totalCount != 0)
                    {
                        shakariTouchedHidden.Content = totalCount;
                        shakariTouchedLbl1.Opacity = 1.0;
                    }

                    break;

                case "stormweaverHidden":

                    if (totalCount != 0)
                    {
                        stormweaverHidden.Content = totalCount;
                        stormweaverLbl1.Opacity = 1.0;
                        stormweaverLbl2.Opacity = 1.0;
                    }

                    break;

                case "crystalSkinnedHidden":

                    if (totalCount != 0)
                    {
                        crystalSkinnedHidden.Content = totalCount;
                        crystalSkinnedLbl1.Opacity = 1.0;
                    }

                    break;

                case "entanglerHidden":

                    if (totalCount != 0)
                    {
                        entanglerHidden.Content = totalCount;
                        entanglerLbl1.Opacity = 1.0;
                        entanglerLbl2.Opacity = 1.0;
                    }

                    break;

                case "executionerHidden":

                    if (totalCount != 0)
                    {
                        executionerHidden.Content = totalCount;
                        executionerLbl1.Opacity = 1.0;
                        executionerLbl2.Opacity = 1.0;
                        executionerLbl3.Opacity = 1.0;
                    }

                    break;

                case "necromancerHidden":

                    if (totalCount != 0)
                    {
                        necromancerHidden.Content = totalCount;
                        necromancerLbl1.Opacity = 1.0;
                        necromancerLbl2.Opacity = 1.0;
                        necromancerLbl3.Opacity = 1.0;
                        necromancerLbl4.Opacity = 1.0;
                        necromancerLbl5.Opacity = 1.0;
                    }

                    break;

                case "gargantuanHidden":

                    if (totalCount != 0)
                    {
                        gargantuanHidden.Content = totalCount;
                        gargantuanLbl1.Opacity = 1.0;
                        gargantuanLbl2.Opacity = 1.0;
                        gargantuanLbl3.Opacity = 1.0;
                        gargantuanLbl4.Opacity = 1.0;
                        gargantuanLbl5.Opacity = 1.0;
                    }

                    break;

                case "echoistHidden":

                    if (totalCount != 0)
                    {
                        echoistHidden.Content = totalCount;
                        echoistLbl1.Opacity = 1.0;
                        echoistLbl2.Opacity = 1.0;
                        echoistLbl3.Opacity = 1.0;
                        echoistLbl4.Opacity = 1.0;
                    }

                    break;

                case "flameStriderHidden":

                    if (totalCount != 0)
                    {
                        flameStriderHidden.Content = totalCount;
                        flameStriderLbl1.Opacity = 1.0;
                    }

                    break;

                case "hastedHidden":

                    if (totalCount != 0)
                    {
                        hastedHidden.Content = totalCount;
                        hastedLbl1.Opacity = 1.0;
                        hastedLbl2.Opacity = 1.0;
                        hastedLbl3.Opacity = 1.0;
                    }

                    break;

                case "heraldingMinionsHidden":

                    if (totalCount != 0)
                    {
                        heraldingMinionsHidden.Content = totalCount;
                        heraldingMinionsLbl1.Opacity = 1.0;
                    }

                    break;

                case "sentinelHidden":

                    if (totalCount != 0)
                    {
                        sentinelHidden.Content = totalCount;
                        sentinelLbl1.Opacity = 1.0;
                        sentinelLbl2.Opacity = 1.0;
                        sentinelLbl3.Opacity = 1.0;
                        sentinelLbl4.Opacity = 1.0;
                    }

                    break;

                case "soulEaterHidden":

                    if (totalCount != 0)
                    {
                        soulEaterHidden.Content = totalCount;
                        soulEaterLbl1.Opacity = 1.0;
                    }

                    break;

                case "steelInfusedHidden":

                    if (totalCount != 0)
                    {
                        steelInfusedHidden.Content = totalCount;
                        steelInfusedLbl1.Opacity = 1.0;
                        steelInfusedLbl2.Opacity = 1.0;
                    }

                    break;

                case "treantHordeHidden":

                    if (totalCount != 0)
                    {
                        treantHordeHidden.Content = totalCount;
                        treantHordeLbl1.Opacity = 1.0;
                    }

                    break;

                case "deadeyeHidden":

                    if (totalCount != 0)
                    {
                        deadeyeHidden.Content = totalCount;
                        deadeyeLbl1.Opacity = 1.0;
                        deadeyeLbl2.Opacity = 1.0;
                        deadeyeLbl3.Opacity = 1.0;
                    }

                    break;

                case "berserkerHidden":

                    if (totalCount != 0)
                    {
                        berserkerHidden.Content = totalCount;
                        berserkerLbl1.Opacity = 1.0;
                        berserkerLbl2.Opacity = 1.0;
                        berserkerLbl3.Opacity = 1.0;
                        berserkerLbl4.Opacity = 1.0;
                    }

                    break;

                case "chaosweaverHidden":

                    if (totalCount != 0)
                    {
                        chaosweaverHidden.Content = totalCount;
                        chaosweaverLbl1.Opacity = 1.0;
                        chaosweaverLbl2.Opacity = 1.0;
                        chaosweaverLbl3.Opacity = 1.0;
                        chaosweaverLbl4.Opacity = 1.0;
                        chaosweaverLbl5.Opacity = 1.0;
                    }

                    break;

                case "consecratorHidden":

                    if (totalCount != 0)
                    {
                        consecratorHidden.Content = totalCount;
                        consecratorLbl1.Opacity = 1.0;
                        consecratorLbl2.Opacity = 1.0;
                        consecratorLbl3.Opacity = 1.0;
                    }

                    break;

                case "dynamoHidden":

                    if (totalCount != 0)
                    {
                        dynamoHidden.Content = totalCount;
                        dynamoLbl1.Opacity = 1.0;
                        dynamoLbl2.Opacity = 1.0;
                    }

                    break;

                case "flameweaverHidden":

                    if (totalCount != 0)
                    {
                        flameweaverHidden.Content = totalCount;
                        flameweaverLbl1.Opacity = 1.0;
                        flameweaverLbl2.Opacity = 1.0;
                    }

                    break;

                case "soulConduitHidden":

                    if (totalCount != 0)
                    {
                        soulConduitHidden.Content = totalCount;
                        soulConduitLbl1.Opacity = 1.0;
                        soulConduitLbl2.Opacity = 1.0;
                    }

                    break;

                case "toxicHidden":

                    if (totalCount != 0)
                    {
                        toxicHidden.Content = totalCount;
                        toxicLbl1.Opacity = 1.0;
                        toxicLbl2.Opacity = 1.0;
                        toxicLbl3.Opacity = 1.0;
                    }

                    break;

                case "vampiricHidden":

                    if (totalCount != 0)
                    {
                        vampiricHidden.Content = totalCount;
                        vampiricLbl1.Opacity = 1.0;
                        vampiricLbl2.Opacity = 1.0;
                        vampiricLbl3.Opacity = 1.0;
                        vampiricLbl4.Opacity = 1.0;
                    }

                    break;

                case "empoweringMinionsHidden":

                    if (totalCount != 0)
                    {
                        empoweringMinionsHidden.Content = totalCount;
                        empoweringMinionsLbl1.Opacity = 1.0;
                        empoweringMinionsLbl2.Opacity = 1.0;
                    }

                    break;

                case "hexerHidden":

                    if (totalCount != 0)
                    {
                        hexerHidden.Content = totalCount;
                        hexerLbl1.Opacity = 1.0;
                        hexerLbl2.Opacity = 1.0;
                    }

                    break;

                case "temporalBubbleHidden":

                    if (totalCount != 0)
                    {
                        temporalBubbleHidden.Content = totalCount;
                        temporalBubbleLbl1.Opacity = 1.0;
                    }

                    break;

                case "bombardierHidden":

                    if (totalCount != 0)
                    {
                        bombardierHidden.Content = totalCount;
                        bombardierLbl1.Opacity = 1.0;
                        bombardierLbl2.Opacity = 1.0;
                        bombardierLbl3.Opacity = 1.0;
                        bombardierLbl4.Opacity = 1.0;
                        bombardierLbl5.Opacity = 1.0;
                    }

                    break;

                case "corpseDetonatorHidden":

                    if (totalCount != 0)
                    {
                        corpseDetonatorHidden.Content = totalCount;
                        corpseDetonatorLbl1.Opacity = 1.0;
                        corpseDetonatorLbl2.Opacity = 1.0;
                    }

                    break;

                case "evocationistHidden":

                    if (totalCount != 0)
                    {
                        evocationistHidden.Content = totalCount;
                        evocationistLbl1.Opacity = 1.0;
                    }

                    break;

                case "tukohamaTouchedHidden":

                    if (totalCount != 0)
                    {
                        tukohamaTouchedHidden.Content = totalCount;
                        tukohamaTouchedLbl1.Opacity = 1.0;
                    }

                    break;

                case "assassinHidden":

                    if (totalCount != 0)
                    {
                        assassinHidden.Content = totalCount;
                        assassinLbl1.Opacity = 1.0;
                        assassinLbl2.Opacity = 1.0;
                    }

                    break;

                case "arakaaliTouchedHidden":

                    if (totalCount != 0)
                    {
                        arakaaliTouchedHidden.Content = totalCount;
                        arakaaliTouchedLbl1.Opacity = 1.0;
                    }

                    break;

                case "opulentHidden":

                    if (totalCount != 0)
                    {
                        opulentHidden.Content = totalCount;
                        opulentLbl1.Opacity = 1.0;
                    }

                    break;

                case "lunarisTouchedHidden":

                    if (totalCount != 0)
                    {
                        lunarisTouchedHidden.Content = totalCount;
                        lunarisTouchedLbl1.Opacity = 1.0;
                    }

                    break;

                case "solarisTouchedHidden":

                    if (totalCount != 0)
                    {
                        solarisTouchedHidden.Content = totalCount;
                        solarisTouchedLbl1.Opacity = 1.0;
                    }

                    break;

                case "innocenceTouchedHidden":

                    if (totalCount != 0)
                    {
                        innocenceTouchedHidden.Content = totalCount;
                        innocenceTouchedLbl1.Opacity = 1.0;
                    }

                    break;

                case "icePrisonHidden":

                    if (totalCount != 0)
                    {
                        icePrisonHidden.Content = totalCount;
                        icePrisonLbl1.Opacity = 1.0;
                    }

                    break;
            }

        }

        public void ReturnLabelToDefault()
        {
            foreach (var childObject in cheatsheetGrid.Children)
            {
                if (childObject.GetType() == typeof(System.Windows.Controls.Label))
                {
                    //Cast childObject as Label
                    ((System.Windows.Controls.Label)childObject).Opacity = 0.5;
                }
            }

            foreach (var childObject in hiddenGrid.Children)
            {
                if (childObject.GetType() == typeof(System.Windows.Controls.Label))
                {
                    //Cast childObject as Label
                    ((System.Windows.Controls.Label)childObject).Content = 0;
                }
            }

        }

        public void ShowOverlay()
        {
            this.Show();
            this.Topmost = true;
            this.WindowState = WindowState.Maximized;
            this.ShowActivated = true;
        }

        #endregion


        #region Other Methods

        public string FindControlName(string modName)
        {
            string controlName = modName;
            StringBuilder controlNameSB = new StringBuilder(modName);

            if (modName.Contains("-"))
            {
                foreach (int index in AllIndexesOf(controlName, "-"))
                {
                    if (Char.IsLetter(modName[index + 1]))
                    {
                        controlNameSB[index] = ' ';
                        controlNameSB[index + 1] = char.Parse(modName[index + 1].ToString().ToUpper());
                    }
                    else if (Char.IsDigit(modName[index + 1]))
                    {
                        controlNameSB[index] = ' ';
                        controlNameSB[index + 1] = ' ';
                    }
                }
            }

            controlName = Regex.Replace(controlNameSB.ToString(), @"\s", "");
            controlName += "Hidden";

            return controlName;
        }

        public IEnumerable<int> AllIndexesOf(string str, string searchstring)
        {
            int minIndex = str.IndexOf(searchstring);
            while (minIndex != -1)
            {
                yield return minIndex;
                minIndex = str.IndexOf(searchstring, minIndex + searchstring.Length);
            }
        }

        #endregion

    }
}
