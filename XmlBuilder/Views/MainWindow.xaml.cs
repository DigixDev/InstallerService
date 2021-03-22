using System.Windows;

namespace XmlBuilder.Views
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += (s, e) =>
            {
                Left = (SystemParameters.FullPrimaryScreenWidth - ActualWidth) / 2;
                Top = 100;
            };
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}