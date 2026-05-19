using MonitorBoard.WPF.Common;

namespace MonitorBoard.WPF.Models
{
    public class DeviceLightModel : NotifyBase
    {
        private bool _isChecked;

        public bool IsChecked
        {
            get { return _isChecked; }
            set { SetProperty<bool>(ref _isChecked, value); }
        }
    }
}
