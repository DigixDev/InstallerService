using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using InstallerService.Annotations;
using Shared.Core;

namespace InstallerService.Views
{
    /// <summary>
    /// Interaction logic for GetUrlView.xaml
    /// </summary>
    public partial class GetUrlView : INotifyPropertyChanged
    {

        private string _dataUrl;

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

        public GetUrlView()
        {
            InitializeComponent();

            OkCommand=new RelayCommand(ExecuteOkCommand, CanExecuteOkCommand);
            CancelCommand=new RelayCommand(ExecuteCancelCommand);
            this.SizeChanged += GetUrlView_SizeChanged;
        }

        private void GetUrlView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.Left = Application.Current.MainWindow.Left - this.ActualWidth - 3;
            this.Top = Application.Current.MainWindow.Top + Application.Current.MainWindow.ActualHeight -
                       this.ActualHeight;
        }

        private void ExecuteCancelCommand(object obj)
        {
            this.DialogResult = false;
            this.Close();
        }

        private bool CanExecuteOkCommand(object arg)
        {
            return String.IsNullOrEmpty(_dataUrl) == false;
        }

        private void ExecuteOkCommand(object obj)
        {
            DialogResult = true;
            Close();
        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
