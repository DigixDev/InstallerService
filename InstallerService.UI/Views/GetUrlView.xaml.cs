using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using InstallerService.UI.Annotations;
using Shared.Core;

namespace InstallerService.UI.Views
{
    /// <summary>
    ///     Interaction logic for GetUrlView.xaml
    /// </summary>
    public partial class GetUrlView : INotifyPropertyChanged
    {
        private string _dataUrl;

        public GetUrlView()
        {
            InitializeComponent();

            OkCommand = new RelayCommand(ExecuteOkCommand, CanExecuteOkCommand);
            CancelCommand = new RelayCommand(ExecuteCancelCommand);
            SizeChanged += GetUrlView_SizeChanged;
        }

        public string DataUrl
        {
            get => _dataUrl;
            set
            {
                if (_dataUrl != value)
                {
                    _dataUrl = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand OkCommand { get; set; }
        public ICommand CancelCommand { get; set; }


        public event PropertyChangedEventHandler PropertyChanged;

        private void GetUrlView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Left = Application.Current.MainWindow.Left - ActualWidth - 3;
            Top = Application.Current.MainWindow.Top + Application.Current.MainWindow.ActualHeight -
                  ActualHeight;
        }

        private void ExecuteCancelCommand(object obj)
        {
            DialogResult = false;
            Close();
        }

        private bool CanExecuteOkCommand(object arg)
        {
            return string.IsNullOrEmpty(_dataUrl) == false;
        }

        private void ExecuteOkCommand(object obj)
        {
            DialogResult = true;
            Close();
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}