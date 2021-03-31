using Updater.Controls;
using Updater.ViewModels;

namespace Updater.Views
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : WindowEx
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += (s, e) => ((MainViewModel) DataContext).StartUpdating();
        }
    }
}