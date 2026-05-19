 using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MonitorBoard.WPF.Common
{
    public class NotifyBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// 判断值都不为Null且不相等时才更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <param name="propName"></param>
        public void SetProperty<T>(ref T field, T value, [CallerMemberName] string propName = "") 
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                this.DoNotify(propName);
            }
        }

        /// <summary>
        /// 手动更新
        /// </summary>
        /// <param name="propertyName"></param>
        public void DoNotify([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
