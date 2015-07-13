using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace FacebookTokenGenerator.App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public FacebookTokenGeneratorViewModel ViewModel;

        public MainWindow()
        {
            ViewModel = new FacebookTokenGeneratorViewModel();
            DataContext = ViewModel;
            ViewModel.ShowFacebookPages = false;
            InitializeComponent();
        }

        private void Button_GenerateToken(object sender, RoutedEventArgs e)
        {
            ViewModel.TokenGenerator();            
        }

        private void Button_CopyToClipboard(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(ViewModel.PageToken);
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox selectBox = (ComboBox)sender;

            var value = "";
            if (selectBox.SelectedIndex >= 0)
                value = ((ComboBoxItem)selectBox.SelectedValue).Content.ToString();

            ViewModel.GetPermanentPageAccessToken(value);
        }
    }
}
