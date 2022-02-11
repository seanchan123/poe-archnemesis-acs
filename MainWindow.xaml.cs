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
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Threading;
using System.Diagnostics;

namespace poe_archnemesis_acs
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        #region Global Variables

        //Store in List<ArchnemesisModModel> to fetch it more accurately
        List<ArchnemesisModModel> archnemesisMods = new List<ArchnemesisModModel>();

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

        const int WM_HOTKEY = 0x0312;

        private IntPtr _windowHandle;
        private HwndSource _source;

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            _windowHandle = new WindowInteropHelper(this).Handle;
            _source = HwndSource.FromHwnd(_windowHandle);
            _source.AddHook(HwndHook);

            RegisterHotKey(_windowHandle, HOTKEY_ID, MOD_SHIFT, VK_AKEY); //LSHIFT + CAPS
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
                                if (this.ShowActivated == false)
                                {
                                    this.Topmost = true;
                                    cheatsheetGrid.Opacity = 0;
                                    loadingGrid.Opacity = 1;
                                    Screenshot();

                                    this.WindowState = WindowState.Maximized;
                                    this.ShowActivated = true;
                                    this.Show();

                                    MatchImage();

                                    cheatsheetGrid.Opacity = 1;
                                    loadingGrid.Opacity = 0;
                                    modSearchTextBox.Focus();
                                }
                                else
                                {
                                    if (this.Visibility == Visibility.Hidden)
                                    {
                                        this.Topmost = true;
                                        cheatsheetGrid.Opacity = 0;
                                        loadingGrid.Opacity = 1;
                                        Screenshot();
                                        this.Show();

                                        MatchImage();

                                        cheatsheetGrid.Opacity = 1;
                                        loadingGrid.Opacity = 0;
                                        modSearchTextBox.Focus();
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
            modNames.Add("hasted");
            modNames.Add("hasted-2");
            modNames.Add("heralding-minions");
            modNames.Add("sentinel");
            modNames.Add("soul-eater");
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
                    manaSiphonerHidden.Content = totalCount;

                    if (totalCount == 0)
                    {
                        manaSiphonerLbl1.Opacity = 0.6;
                    }

                    break;

                case "mirrorImageHidden":
                    mirrorImageHidden.Content = totalCount;

                    if (totalCount == 0)
                    {
                        mirrorImageLbl1.Opacity = 0.6;
                    }

                    break;

                case "empoweredElementsHidden":
                    empoweredElementsHidden.Content = totalCount;

                    if (totalCount == 0)
                    {
                        empoweredElementsLbl1.Opacity = 0.6;
                    }

                    break;

                case "stormStriderHidden":
                    stormStriderHidden.Content = totalCount;

                    if (totalCount == 0)
                    {
                        stormStriderLbl1.Opacity = 0.6;
                    }

                    break;

                case "arcaneBufferHidden":
                    arcaneBufferHidden.Content = totalCount;

                    if (totalCount == 0)
                    {
                        arcaneBufferLbl1.Opacity = 0.6;
                        arcaneBufferLbl2.Opacity = 0.6;
                    }

                    break;

                case "bloodletterHidden":
                    bloodletterHidden.Content = totalCount;

                    if (totalCount == 0)
                    {
                        bloodletterLbl1.Opacity = 0.6;
                        bloodletterLbl2.Opacity = 0.6;
                        bloodletterLbl3.Opacity = 0.6;
                        bloodletterLbl4.Opacity = 0.6;
                    }

                    break;

                case "bonebreakerHidden":
                    bonebreakerHidden.Content = totalCount;

                    if (totalCount == 0)
                    {
                        bonebreakerLbl1.Opacity = 0.6;
                        bonebreakerLbl2.Opacity = 0.6;
                        bonebreakerLbl3.Opacity = 0.6;
                        bonebreakerLbl4.Opacity = 0.6;
                    }

                    break;

                case "brineKingTouchedHidden":
                    brineKingTouchedHidden.Content = totalCount;

                    if (totalCount == 0)
                    {
                        brineKingTouchedLbl1.Opacity = 0.6;
                    }

                    break;

                case "corrupterHidden":
                    corrupterHidden.Content = totalCount;

                    if (totalCount == 0)
                    {
                        corrupterLbl1.Opacity = 0.6;
                        corrupterLbl2.Opacity = 0.6;
                    }

                    break;

                case "droughtBringerHidden":
                    droughtBringerHidden.Content = totalCount;

                    if (totalCount == 0)
                    {
                        droughtBringerLbl1.Opacity = 0.6;
                    }

                    break;

                case "effigyHidden":
                    effigyHidden.Content = totalCount;

                    if (totalCount == 0)
                    {
                        effigyLbl1.Opacity = 0.6;
                    }

                    break;

                case "frenziedHidden":
                    frenziedHidden.Content = totalCount;

                    if (totalCount == 0)
                    {
                        frenziedLbl1.Opacity = 0.6;
                        frenziedLbl2.Opacity = 0.6;
                        frenziedLbl3.Opacity = 0.6;
                        frenziedLbl4.Opacity = 0.6;
                    }

                    break;

                case "frostStriderHidden":
                    frostStriderHidden.Content = totalCount;

                    if (totalCount == 0)
                    {
                        frostStriderLbl1.Opacity = 0.6;
                    }

                    break;

                case "frostweaverHidden":
                    frostweaverHidden.Content = totalCount;

                    if (totalCount == 0)
                    {
                        frostweaverLbl1.Opacity = 0.6;
                        frostweaverLbl2.Opacity = 0.6;
                    }

                    break;

                case "incendiaryHidden":
                    incendiaryHidden.Content = totalCount;

                    if (totalCount == 0)
                    {
                        incendiaryLbl1.Opacity = 0.6;
                        incendiaryLbl2.Opacity = 0.6;
                        incendiaryLbl3.Opacity = 0.6;
                    }

                    break;

                case "invulnerableHidden":
                    invulnerableHidden.Content = totalCount;

                    if (totalCount == 0)
                    {
                        invulnerableLbl1.Opacity = 0.6;
                        invulnerableLbl2.Opacity = 0.6;
                    }

                    break;

                case "juggernautHidden":
                    juggernautHidden.Content = totalCount;

                    if (totalCount == 0)
                    {
                        juggernautLbl1.Opacity = 0.6;
                        juggernautLbl2.Opacity = 0.6;
                        juggernautLbl3.Opacity = 0.6;
                    }

                    break;

                case "magmaBarrierHidden":

                    if (totalCount != 0)
                    {
                        magmaBarrierHidden.Content = totalCount;
                    }

                    if (int.Parse(magmaBarrierHidden.Content.ToString()) == 0)
                    {
                        magmaBarrierLbl1.Opacity = 0.6;
                        magmaBarrierLbl2.Opacity = 0.6;
                    }

                    break;

                case "maledictionHidden":
                    maledictionHidden.Content = totalCount;

                    if (totalCount == 0)
                    {
                        maledictionLbl1.Opacity = 0.6;
                        maledictionLbl2.Opacity = 0.6;
                    }

                    break;

                case "overchargedHidden":
                    overchargedHidden.Content = totalCount;

                    if (totalCount == 0)
                    {
                        overchargedLbl1.Opacity = 0.6;
                        overchargedLbl2.Opacity = 0.6;
                        overchargedLbl3.Opacity = 0.6;
                        overchargedLbl4.Opacity = 0.6;
                        overchargedLbl5.Opacity = 0.6;
                        overchargedLbl6.Opacity = 0.6;
                    }

                    break;

                case "permafrostHidden":
                    permafrostHidden.Content = totalCount;

                    if (totalCount == 0)
                    {
                        permafrostLbl1.Opacity = 0.6;
                        permafrostLbl2.Opacity = 0.6;
                    }

                    break;

                case "rejuvenatingHidden":
                    rejuvenatingHidden.Content = totalCount;

                    if (totalCount == 0)
                    {
                        rejuvenatingLbl1.Opacity = 0.6;
                        rejuvenatingLbl2.Opacity = 0.6;
                    }

                    break;

                case "shakariTouchedHidden":
                    shakariTouchedHidden.Content = totalCount;

                    if (totalCount == 0)
                    {
                        shakariTouchedLbl1.Opacity = 0.6;
                    }

                    break;

                case "stormweaverHidden":
                    stormweaverHidden.Content = totalCount;

                    if (totalCount == 0)
                    {
                        stormweaverLbl1.Opacity = 0.6;
                        stormweaverLbl2.Opacity = 0.6;
                    }

                    break;

                case "crystalSkinnedHidden":
                    crystalSkinnedHidden.Content = totalCount;

                    if (totalCount == 0)
                    {
                        crystalSkinnedLbl1.Opacity = 0.6;
                    }

                    break;

                case "entanglerHidden":
                    entanglerHidden.Content = totalCount;

                    if (totalCount == 0)
                    {
                        entanglerLbl1.Opacity = 0.6;
                        entanglerLbl2.Opacity = 0.6;
                    }

                    break;

                case "executionerHidden":
                    executionerHidden.Content = totalCount;

                    if (totalCount == 0)
                    {
                        executionerLbl1.Opacity = 0.6;
                        executionerLbl2.Opacity = 0.6;
                        executionerLbl3.Opacity = 0.6;
                    }

                    break;

                case "necromancerHidden":
                    shakariTouchedHidden.Content = totalCount;

                    if (totalCount == 0)
                    {
                        necromancerLbl1.Opacity = 0.6;
                        necromancerLbl2.Opacity = 0.6;
                        necromancerLbl3.Opacity = 0.6;
                        necromancerLbl4.Opacity = 0.6;
                        necromancerLbl5.Opacity = 0.6;
                    }

                    break;

                case "gargantuanHidden":
                    gargantuanHidden.Content = totalCount;

                    if (totalCount == 0)
                    {
                        gargantuanLbl1.Opacity = 0.6;
                        gargantuanLbl2.Opacity = 0.6;
                        gargantuanLbl3.Opacity = 0.6;
                        gargantuanLbl4.Opacity = 0.6;
                        gargantuanLbl5.Opacity = 0.6;
                    }

                    break;

                case "echoistHidden":
                    echoistHidden.Content = totalCount;

                    if (totalCount == 0)
                    {
                        echoistLbl1.Opacity = 0.6;
                        echoistLbl2.Opacity = 0.6;
                        echoistLbl3.Opacity = 0.6;
                        echoistLbl4.Opacity = 0.6;
                    }

                    break;

                case "flameStriderHidden":
                    flameStriderHidden.Content = totalCount;

                    if (totalCount == 0)
                    {
                        flameStriderLbl1.Opacity = 0.6;
                    }

                    break;

                case "hastedHidden":

                    if (totalCount != 0)
                    {
                        hastedHidden.Content = totalCount;
                    }

                    if (int.Parse(hastedHidden.Content.ToString()) == 0)
                    {
                        hastedLbl1.Opacity = 0.6;
                        hastedLbl2.Opacity = 0.6;
                        hastedLbl3.Opacity = 0.6;
                    }

                    break;

                case "heraldingMinionsHidden":
                    heraldingMinionsHidden.Content = totalCount;

                    if (totalCount == 0)
                    {
                        heraldingMinionsLbl1.Opacity = 0.6;
                    }

                    break;

                case "sentinelHidden":
                    sentinelHidden.Content = totalCount;

                    if (totalCount == 0)
                    {
                        sentinelLbl1.Opacity = 0.6;
                        sentinelLbl2.Opacity = 0.6;
                        sentinelLbl3.Opacity = 0.6;
                        sentinelLbl4.Opacity = 0.6;
                    }

                    break;

                case "soulEaterHidden":
                    soulEaterHidden.Content = totalCount;

                    if (totalCount == 0)
                    {
                        soulEaterLbl1.Opacity = 0.6;
                    }

                    break;

                case "steelInfusedHidden":
                    steelInfusedHidden.Content = totalCount;

                    if (totalCount == 0)
                    {
                        steelInfusedLbl1.Opacity = 0.6;
                        steelInfusedLbl2.Opacity = 0.6;
                    }

                    break;

                case "treantHordeHidden":
                    treantHordeHidden.Content = totalCount;

                    if (totalCount == 0)
                    {
                        treantHordeLbl1.Opacity = 0.6;
                    }

                    break;

                case "deadeyeHidden":
                    deadeyeHidden.Content = totalCount;

                    if (totalCount == 0)
                    {
                        deadeyeLbl1.Opacity = 0.6;
                        deadeyeLbl2.Opacity = 0.6;
                        deadeyeLbl3.Opacity = 0.6;
                    }

                    break;

                case "berserkerHidden":
                    berserkerHidden.Content = totalCount;

                    if (totalCount == 0)
                    {
                        berserkerLbl1.Opacity = 0.6;
                        berserkerLbl2.Opacity = 0.6;
                        berserkerLbl3.Opacity = 0.6;
                        berserkerLbl4.Opacity = 0.6;
                    }

                    break;

                case "chaosweaverHidden":
                    chaosweaverHidden.Content = totalCount;

                    if (totalCount == 0)
                    {
                        chaosweaverLbl1.Opacity = 0.6;
                        chaosweaverLbl2.Opacity = 0.6;
                        chaosweaverLbl3.Opacity = 0.6;
                        chaosweaverLbl4.Opacity = 0.6;
                        chaosweaverLbl5.Opacity = 0.6;
                    }

                    break;

                case "consecratorHidden":
                    consecratorHidden.Content = totalCount;

                    if (totalCount == 0)
                    {
                        consecratorLbl1.Opacity = 0.6;
                        consecratorLbl2.Opacity = 0.6;
                        consecratorLbl3.Opacity = 0.6;
                    }

                    break;

                case "dynamoHidden":
                    dynamoHidden.Content = totalCount;

                    if (totalCount == 0)
                    {
                        dynamoLbl1.Opacity = 0.6;
                        dynamoLbl2.Opacity = 0.6;
                    }

                    break;

                case "flameweaverHidden":
                    flameweaverHidden.Content = totalCount;

                    if (totalCount == 0)
                    {
                        flameweaverLbl1.Opacity = 0.6;
                        flameweaverLbl2.Opacity = 0.6;
                    }

                    break;

                case "soulConduitHidden":
                    soulConduitHidden.Content = totalCount;

                    if (totalCount == 0)
                    {
                        soulConduitLbl1.Opacity = 0.6;
                        soulConduitLbl2.Opacity = 0.6;
                    }

                    break;

                case "toxicHidden":
                    toxicHidden.Content = totalCount;

                    if (totalCount == 0)
                    {
                        toxicLbl1.Opacity = 0.6;
                        toxicLbl2.Opacity = 0.6;
                        toxicLbl3.Opacity = 0.6;
                    }

                    break;

                case "vampiricHidden":
                    vampiricHidden.Content = totalCount;

                    if (totalCount == 0)
                    {
                        vampiricLbl1.Opacity = 0.6;
                        vampiricLbl2.Opacity = 0.6;
                        vampiricLbl3.Opacity = 0.6;
                        vampiricLbl4.Opacity = 0.6;
                    }

                    break;

                case "empoweringMinionsHidden":
                    empoweringMinionsHidden.Content = totalCount;

                    if (totalCount == 0)
                    {
                        empoweringMinionsLbl1.Opacity = 0.6;
                        empoweringMinionsLbl2.Opacity = 0.6;
                    }

                    break;
            }

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
