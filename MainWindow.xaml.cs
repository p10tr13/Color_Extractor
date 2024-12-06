using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Numerics;

namespace GK_Proj_3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
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

                        for (int i = 0; i < pixels.Length; i += 4)
                        {
                            b = pixels[i];
                            g = pixels[i + 1];
                            r = pixels[i + 2];

                            float xR, yR, xB, yB, xG, yG, xW, yW, gamma, LabL, Laba, Labb;
                            if (!float.TryParse(xWhitePointTextBox.Text, out xW) || !float.TryParse(yWhitePointTextBox.Text, out yW))
                                return;
                            if (yW == 0)
                                return;
                            if (!float.TryParse(xRedTextBox.Text, out xR) || !float.TryParse(yRedTextBox.Text,out yR))
                                return;
                            if (!float.TryParse(xGreenTextBox.Text, out xG) || !float.TryParse(yGreenTextBox.Text, out yG))
                                return;
                            if (!float.TryParse(xBlueTextBox.Text, out xB) || !float.TryParse(yBlueTextBox.Text, out yB))
                                return;
                            if (!float.TryParse(GammaTextBox.Text, out gamma))
                                return;
                            float Yn = 100, Xn = (xW / yW) * Yn, Zn = ((1 - xW - yW)/yW) * Yn;
                            (float X, float Y, float Z) = ConvertRGBToXYZ(r, g, b, xR, yR, xG, yG, xB, yB, xW, yW);

                            if (Y / Yn > 0.008856)
                                LabL = 116f * (float)Math.Cbrt(Y / Yn) - 16f;
                            else
                                LabL = 903.3f * Y / Yn;
                            Laba = 500 * ((float)Math.Cbrt(X/ Xn) - (float)Math.Cbrt(Y / Yn));
                            Labb = 200 * ((float)Math.Cbrt(Y/Yn) - (float)Math.Cbrt(Z/ Zn));
                            Laba = Math.Clamp(Laba, -127, 127);
                            Labb = Math.Clamp(Labb, -127, 127);
                            Laba /= 127;
                            Labb /= 127;
                            Laba = ((Laba + 1) / 2) * 255;
                            Labb = ((Labb + 1) / 2) * 255;
                            channel1[i] = channel1[i + 1] = channel1[i + 2] = (byte)LabL;
                            channel2[i + 2] = (byte) Laba;
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
                    yWhitePointTextBox.Text= values[1];
                }
            }
        }

        private (float X, float Y, float Z) ConvertRGBToXYZ(float r, float g, float b, float xR, float yR, float xG, float yG, float xB, float yB, float xW, float yW)
        {
            float zR = 1 - xR - yR;
            float zG = 1 - xG - yG;
            float zB = 1 - xB - yB;

            //Matrix4x4 matrix = new Matrix4x4()
            //{
            //    M11 = xR/yR, M12 = xG/yG, M13 = xB/yB, M14 = 0,
            //    M21 = 1, M22 = 1, M23 = 1, M24 = 0,
            //    M31 = zR/yR, M32 = zG/yG, M33 = zB/yB, M34 = 0,
            //    M41 = 0, M42 = 0, M43 = 0, M44 = 1
            //};

            Matrix4x4 matrix = new Matrix4x4()
            {
                M11 = xR,
                M12 = xG,
                M13 = xB,
                M14 = 0,
                M21 = yR,
                M22 = yG,
                M23 = yB,
                M24 = 0,
                M31 = zR,
                M32 = zG,
                M33 = zB,
                M34 = 0,
                M41 = 0,
                M42 = 0,
                M43 = 0,
                M44 = 1
            };



            float XW = xW / yW;
            float YW = 1.0f;
            float ZW = (1 - xW - yW) / yW;
            Vector3 vector = new Vector3() { X = XW, Y = YW, Z = ZW};

            Matrix4x4 matrixinverse;
            if (!Matrix4x4.Invert(matrix, out matrixinverse))
                throw new Exception();
            var S = Vector4.Transform(vector, matrixinverse);


            var M_RGB_to_XYZ = new float[,] {
                { matrix.M11 * S.X, matrix.M12 * S.Y, matrix.M13 * S.Z },
                { matrix.M12 * S.X, matrix.M22 * S.Y, matrix.M23 * S.Z },
                { matrix.M13 * S.X, matrix.M23 * S.Y, matrix.M33 * S.Z }};

            r /= 255.0f;
            g /= 255.0f;
            b /= 255.0f;

            float X = r * M_RGB_to_XYZ[0, 0] + g * M_RGB_to_XYZ[0, 1] + b * M_RGB_to_XYZ[0, 2];
            float Y = r * M_RGB_to_XYZ[1, 0] + g * M_RGB_to_XYZ[1, 1] + b * M_RGB_to_XYZ[1, 2];
            float Z = r * M_RGB_to_XYZ[2, 0] + g * M_RGB_to_XYZ[2, 1] + b * M_RGB_to_XYZ[2, 2];

            return (X, Z, Y);
        }
    }
}