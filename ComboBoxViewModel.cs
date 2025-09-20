using System.Collections.ObjectModel;
using System.ComponentModel;

namespace GK_Proj_3
{
    public class ComboBoxItemModel
    {
        public string DisplayName { get; set; }
        public string Label1Value { get; set; }
        public string Label2Value { get; set; }
        public string Label3Value { get; set; }

        public override string ToString()
        {
            return DisplayName;
        }
    }

    public class MainViewModel : INotifyPropertyChanged
    {
        private ComboBoxItemModel selectedItem;

        public ObservableCollection<ComboBoxItemModel> ComboBoxItems { get; set; }

        public ComboBoxItemModel SelectedItem
        {
            get => selectedItem;
            set
            {
                selectedItem = value;
                OnPropertyChanged(nameof(SelectedItem));
            }
        }

        public MainViewModel()
        {
            ComboBoxItems = new ObservableCollection<ComboBoxItemModel>
        {
            new ComboBoxItemModel { DisplayName = "YCbCr", Label1Value = "Y", Label2Value = "Cb", Label3Value = "Cr" },
            new ComboBoxItemModel { DisplayName = "HSV", Label1Value = "H", Label2Value = "S", Label3Value = "V" },
            new ComboBoxItemModel { DisplayName = "Lab", Label1Value = "L", Label2Value = "a", Label3Value = "b" },
        };
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
