using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
            }
        }
    }
}