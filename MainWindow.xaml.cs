using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Numerics;
using System.Windows.Documents;
using System.IO;

namespace GK_Proj_3
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void loadImageButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Title = "Select a picture";
            dialog.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
              "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
              "Portable Network Graphic (*.png)|*.png";
            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                MainCanvas.Children.Clear();
                string fileName = dialog.FileName;
                var imgbitmap = new BitmapImage();
                imgbitmap.BeginInit();
                imgbitmap.UriSource = new Uri(fileName);
                imgbitmap.CacheOption = BitmapCacheOption.OnLoad;
                imgbitmap.EndInit();
                Var.imagebitmap = imgbitmap;
                var img = new Image()
                {
                    Source = Var.imagebitmap,
                    Stretch = Stretch.Uniform,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch
                };
                img.Height = MainCanvas.ActualHeight;
                img.Width = MainCanvas.ActualWidth;
                MainCanvas.Children.Add(img);
                Child1Cavas.Children.Clear();
                Child2Cavas.Children.Clear();
                Child3Cavas.Children.Clear();
                Var.child1WB = new WriteableBitmap(imgbitmap.PixelWidth, imgbitmap.PixelHeight, 96, 96, PixelFormats.Bgra32, null);
                Var.child2WB = new WriteableBitmap(imgbitmap.PixelWidth, imgbitmap.PixelHeight, 96, 96, PixelFormats.Bgra32, null);
                Var.child3WB = new WriteableBitmap(imgbitmap.PixelWidth, imgbitmap.PixelHeight, 96, 96, PixelFormats.Bgra32, null);
                var imgc1 = new Image() { Source = Var.child1WB, Stretch = Stretch.Uniform, HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
                imgc1.Height = Child1Cavas.ActualHeight;
                imgc1.Width = Child1Cavas.ActualWidth;
                var imgc2 = new Image() { Source = Var.child2WB, Stretch = Stretch.Uniform, HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
                imgc2.Height = Child2Cavas.ActualHeight;
                imgc2.Width = Child2Cavas.ActualWidth;
                var imgc3 = new Image() { Source = Var.child3WB, Stretch = Stretch.Uniform, HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
                imgc3.Height = Child3Cavas.ActualHeight;
                imgc3.Width = Child3Cavas.ActualWidth;
                Child1Cavas.Children.Add(imgc1);
                Child2Cavas.Children.Add(imgc2);
                Child3Cavas.Children.Add(imgc3);
                saveImagesButton.IsEnabled = false;
            }
        }

        private void seperateChannelsButton_Click(object sender, RoutedEventArgs e)
        {
            if (ModelComboBox.SelectedItem == null || Var.imagebitmap == null)
                return;

            int width = Var.imagebitmap.PixelWidth;
            int height = Var.imagebitmap.PixelHeight;
            byte[] pixels = new byte[width * height * 4];
            Var.imagebitmap.CopyPixels(pixels, width * 4, 0);

            byte[] channel1 = new byte[pixels.Length];
            byte[] channel2 = new byte[pixels.Length];
            byte[] channel3 = new byte[pixels.Length];

            switch (ModelComboBox.SelectedItem.ToString())
            {
                case "YCbCr":
                    {
                        byte r, g, b;

                        for (int i = 0; i < pixels.Length; i += 4)
                        {
                            b = pixels[i];
                            g = pixels[i + 1];
                            r = pixels[i + 2];

                            double y = 0.299 * r + 0.587 * g + 0.114 * b;
                            double cb = 128 - 0.168736 * r - 0.331264 * g + 0.5 * b;
                            double cr = 128 + 0.5 * r - 0.418688 * g - 0.081312 * b;

                            channel1[i] = channel1[i + 1] = channel1[i + 2] = (byte)y;

                            channel2[i + 2] = 127;
                            channel2[i + 1] = (byte)(255 - cb);
                            channel2[i] = (byte)cb;

                            channel3[i + 1] = (byte)(255 - cr);
                            channel3[i + 2] = (byte)cr;
                            channel3[i] = 127;

                            channel1[i + 3] = channel2[i + 3] = channel3[i + 3] = 255;
                        }
                    }
                    break;
                case "HSV":
                    {
                        byte r, g, b;

                        for (int i = 0; i < pixels.Length; i += 4)
                        {
                            b = pixels[i];
                            g = pixels[i + 1];
                            r = pixels[i + 2];

                            double Xmax = Math.Max(r, Math.Max(g, b));
                            double Xmin = Math.Min(r, Math.Min(g, b));
                            double C = Xmax - Xmin;
                            double H, S, V = Xmax;

                            if (V != 0)
                                S = C / Xmax;
                            else
                                S = 0;

                            if (C == 0)
                                H = 0;
                            else if (V == r)
                                H = 60 * (((g - b) / C) % 6);
                            else if (V == g)
                                H = 60 * ((b - r) / C + 2);
                            else
                                H = 60 * ((r - g) / C + 4);

                            //Option 1
                            channel1[i] = channel1[i + 1] = channel1[i + 2] = (byte)(H / 360 * 255);
                            channel2[i] = channel2[i + 1] = channel2[i + 2] = (byte)(S * 255);
                            channel3[i] = channel3[i + 1] = channel3[i + 2] = (byte)V;
                            channel1[i + 3] = channel2[i + 3] = channel3[i + 3] = 255;
                        }
                    }
                    break;
                case "Lab":
                    {
                        byte r, g, b;
                        float xR, yR, xB, yB, xG, yG, xW, yW, gamma, LabL, Laba, Labb, Yn, Xn, Zn;
                        float[,] M;
                        string colorSpace = "default";
                        if (!float.TryParse(xWhitePointTextBox.Text, out xW) || !float.TryParse(yWhitePointTextBox.Text, out yW))
                            return;
                        if (!float.TryParse(xRedTextBox.Text, out xR) || !float.TryParse(yRedTextBox.Text, out yR))
                            return;
                        if (!float.TryParse(xGreenTextBox.Text, out xG) || !float.TryParse(yGreenTextBox.Text, out yG))
                            return;
                        if (!float.TryParse(xBlueTextBox.Text, out xB) || !float.TryParse(yBlueTextBox.Text, out yB))
                            return;
                        if (!float.TryParse(GammaTextBox.Text, out gamma))
                            return;

                        if (ColorProfileComboBox.SelectedItem is ComboBoxItem selectedItem)
                        {
                            if (selectedItem.Content.ToString() != null)
                                colorSpace = selectedItem.Content.ToString();
                        }

                        try
                        {
                            if (yW == 0)
                                throw new Exception("White point y nie może być 0");
                            M = FindRGBToXYZConversionMatrix(xR, yR, xG, yG, xB, yB, xW, yW);
                        }
                        catch (Exception ex) 
                        {
                            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        Yn = 1;
                        Xn = xW / yW;
                        Zn = (1 - xW - yW) / yW;

                        for (int i = 0; i < pixels.Length; i += 4)
                        {
                            b = pixels[i];
                            g = pixels[i + 1];
                            r = pixels[i + 2];

                            (float X, float Y, float Z) = ConvertRGBToXYZ(r, g, b, M, gamma, colorSpace);

                            if (Y / Yn > 0.008856f)
                                LabL = 116.0f * MathF.Cbrt(Y / Yn) - 16.0f;
                            else
                                LabL = 903.3f * Y / Yn;
                            Laba = 500.0f * (MathF.Cbrt(X / Xn) - MathF.Cbrt(Y / Yn));
                            Labb = 200.0f * (MathF.Cbrt(Y / Yn) - MathF.Cbrt(Z / Zn));
                            Laba = Math.Clamp(Laba, -127, 127);
                            Labb = Math.Clamp(Labb, -127, 127);
                            Laba /= 127.0f;
                            Labb /= 127.0f;
                            Laba = ((Laba + 1.0f) / 2.0f) * 255.0f;
                            Labb = ((Labb + 1.0f) / 2.0f) * 255.0f;

                            // Zapisanie pikseli
                            channel1[i] = channel1[i + 1] = channel1[i + 2] = (byte)LabL;
                            channel2[i + 2] = (byte)Laba;
                            channel2[i + 1] = (byte)(255 - Laba);
                            channel2[i] = 127;
                            channel3[i + 2] = (byte)Labb;
                            channel3[i + 1] = 127;
                            channel3[i] = (byte)(255 - Labb);
                            channel1[i + 3] = channel2[i + 3] = channel3[i + 3] = 255;
                        }
                    }
                    break;
            }

            UpdateWB(Var.child1WB, channel1);
            UpdateWB(Var.child2WB, channel2);
            UpdateWB(Var.child3WB, channel3);
            saveImagesButton.IsEnabled = true;
        }

        private void UpdateWB(WriteableBitmap wb, byte[] pixels)
        {
            wb.Lock();
            try
            {
                wb.WritePixels(new Int32Rect(0, 0, wb.PixelWidth, wb.PixelHeight), pixels, wb.PixelWidth * 4, 0);
            }
            finally
            {
                wb.Unlock();
            }
        }

        private void ColorProfileComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ColorProfileComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                string name = selectedItem.Content.ToString();

                if (Var.colorProfiles.TryGetValue(name, out var values))
                {
                    xRedTextBox.Text = values[0];
                    yRedTextBox.Text = values[1];
                    xGreenTextBox.Text = values[2];
                    yGreenTextBox.Text = values[3];
                    xBlueTextBox.Text = values[4];
                    yBlueTextBox.Text = values[5];
                    xWhitePointTextBox.Text = values[6];
                    yWhitePointTextBox.Text = values[7];
                    GammaTextBox.Text = values[8];
                    foreach (ComboBoxItem item in IlluminantComboBox.Items)
                    {
                        if (item.Content.ToString() == values[9])
                        {
                            IlluminantComboBox.SelectedItem = item;
                            break;
                        }
                    }
                }
            }
        }

        private void IlluminantComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IlluminantComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                string name = selectedItem.Content.ToString();

                if (Var.illuminantDictionary.TryGetValue(name, out var values))
                {
                    xWhitePointTextBox.Text = values[0];
                    yWhitePointTextBox.Text = values[1];
                }
            }
        }

        private static (float X, float Y, float Z) ConvertRGBToXYZ(float r, float g, float b, float[,] M, float gamma, string colorSpace)
        {
            r /= 255.0f;
            g /= 255.0f;
            b /= 255.0f;

            (r, g, b) = InverseGammaCorrection(r, g, b, gamma, colorSpace);

            float X = r * M[0, 0] + g * M[0, 1] + b * M[0, 2];
            float Y = r * M[1, 0] + g * M[1, 1] + b * M[1, 2];
            float Z = r * M[2, 0] + g * M[2, 1] + b * M[2, 2];

            return (X, Y, Z);
        }

        private static float[,] FindRGBToXYZConversionMatrix(float xR, float yR, float xG, float yG, float xB, float yB, float xW, float yW)
        {
            float zR = 1.0f - xR - yR;
            float zG = 1.0f - xG - yG;
            float zB = 1.0f - xB - yB;

            Matrix4x4 matrix = new Matrix4x4()
            {
                M11 = xR / yR,
                M12 = xG / yG,
                M13 = xB / yB,
                M14 = 0,
                M21 = 1.0f,
                M22 = 1.0f,
                M23 = 1.0f,
                M24 = 0,
                M31 = zR / yR,
                M32 = zG / yG,
                M33 = zB / yB,
                M34 = 0,
                M41 = 0,
                M42 = 0,
                M43 = 0,
                M44 = 1
            };

            float XW = xW / yW;
            float YW = 1.0f;
            float ZW = (1.0f - xW - yW) / yW;

            Matrix4x4 matrixinverse;
            if (!Matrix4x4.Invert(matrix, out matrixinverse))
                throw new Exception("Wystąpił błąd przy odwracaniu macierzy przekształcenia! Podaj inne wartości.");

            var SR = XW * matrixinverse.M11 + YW * matrixinverse.M12 + ZW * matrixinverse.M13;
            var SG = XW * matrixinverse.M21 + YW * matrixinverse.M22 + ZW * matrixinverse.M23;
            var SB = XW * matrixinverse.M31 + YW * matrixinverse.M32 + ZW * matrixinverse.M33;

            float[,] M_RGB_to_XYZ = new float[,] {
                { matrix.M11 * SR, matrix.M12 * SG, matrix.M13 * SB },
                { matrix.M21 * SR, matrix.M22 * SG, matrix.M23 * SB },
                { matrix.M31 * SR, matrix.M32 * SG, matrix.M33 * SB }};

            return M_RGB_to_XYZ;
        }

        private static (float, float, float) InverseGammaCorrection(float r, float g, float b, float gamma, string colorSpace)
        {
            float nr, ng, nb;

            switch (colorSpace)
            {
                case "sRGB":
                    {
                        nr = r <= 0.04045f ? r : (float)Math.Pow((r + 0.055f) / 1.055f, gamma);
                        ng = g <= 0.04045f ? g : (float)Math.Pow((g + 0.055f) / 1.055f, gamma);
                        nb = b <= 0.04045f ? b : (float)Math.Pow((b + 0.055f) / 1.055f, gamma);
                        return (nr, ng, nb);
                    }
                default:
                    {
                        return ((float)Math.Pow(r, gamma), (float)Math.Pow(g, gamma), (float)Math.Pow(b, gamma));
                    }
            }
        }

        private void saveImagesButton_Click(object sender, RoutedEventArgs e)
        {
            string directoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Color_Extractor");
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            string fName1 = "img1_", fName2 = "img2_", fName3 = "img3_";
            
            MainViewModel mVM = this.DataContext as MainViewModel;
            if (mVM != null && mVM.SelectedItem != null) 
            {
                fName1 += mVM.SelectedItem.Label1Value + "_";
                fName2 += mVM.SelectedItem.Label2Value + "_";
                fName3 += mVM.SelectedItem.Label3Value + "_";
            }

            string now = DateTime.Now.ToString("hh_mm_ss") + ".png";
            fName1 += now;
            fName2 += now;
            fName3 += now;

            SaveBitmap(Var.child1WB, Path.Combine(directoryPath, fName1));
            SaveBitmap(Var.child2WB, Path.Combine(directoryPath, fName2));
            SaveBitmap(Var.child3WB, Path.Combine(directoryPath, fName3));
        }

        private static void SaveBitmap(WriteableBitmap bitmap, string filePath)
        {
            using (FileStream stream = new FileStream(filePath, FileMode.Create))
            {
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmap));

                encoder.Save(stream);
            }
        }
    }
}