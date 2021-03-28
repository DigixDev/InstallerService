using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Shared.Core;
using Shared.Tools;

namespace InstallerApp.ViewModels
{
    public class SettingViewModel: ViewModelBase
    {
        private readonly Window _parent;
        private string _dataUrl;
        private double _interval;

        public double Interval
        {
            get => _interval;
            set
            {
                _interval = value;
                OnPropertyChanged();
            }
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

        public ICommand SaveCommand { get; set; }

        private bool CanExecuteSaveCommand(object arg)
        {
            return String.IsNullOrEmpty(DataUrl) == false;
        }

        private void ExecuteSaveCommand(object obj)
        {
            SettingManager.SetDataPackUrl(DataUrl);
            SettingManager.SetUpdateInterval(Interval);
            _parent.Close();
        }

        public SettingViewModel(Window parent)
        {
            _parent = parent;
            SaveCommand=new RelayCommand(ExecuteSaveCommand, CanExecuteSaveCommand);    
            DataUrl = SettingManager.GetDataPackUrl();
            Interval = SettingManager.GetUpdateInterval();
        }
    }
}
