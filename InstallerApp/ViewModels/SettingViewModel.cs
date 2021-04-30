using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Shared.Core;
using Shared.Models;
using Shared.Tools;

namespace InstallerApp.ViewModels
{
    public class SettingViewModel: ViewModelBase
    {
        private readonly Window _parent;
        private string _dataUrl;
        private int _interval;
        private int _port;

        public int Port
        {
            get => _port;
            set
            {
                _port = value;
                OnPropertyChanged();
            } 
        }

        public int Interval
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
            if (Interval < 0)
                return false;

            if (String.IsNullOrEmpty(DataUrl))
                return false;

            return DataUrl.ToLower().Contains("apppack.json");
        }

        private void ExecuteSaveCommand(object obj)
        {
            if (DataUrl.Equals(SettingManager.Setting.JsonDataUrl) == false)
                SettingManager.UpdateLocalDataPack(DataUrl);

            var setting = new SettingModel()
            {
                JsonDataUrl = DataUrl,
                Port = Port,
                Interval = Interval
            };
            SettingManager.WriteSetting(setting);
            _parent.DialogResult = true;
            _parent.Close();
        }

        public SettingViewModel(Window parent)
        {
            _parent = parent;
            SaveCommand=new RelayCommand(ExecuteSaveCommand, CanExecuteSaveCommand);    

            DataUrl = SettingManager.Setting.JsonDataUrl;
            Interval= SettingManager.Setting.Interval;
            Port = SettingManager.Setting.Port;
        }
    }
}
