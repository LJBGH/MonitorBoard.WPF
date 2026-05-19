using MonitorBoard.WPF.Common;
using System.Windows.Media;

namespace MonitorBoard.WPF.Models
{
    public class MessageModel : NotifyBase
    {
        private string _title;

        public string Title
        {
            get { return _title; }
            set { SetProperty<string>(ref _title, value); }
        }

        private DateTime _msgTime;

        public DateTime MsgTime
        {
            get { return _msgTime; }
            set { SetProperty<DateTime>(ref _msgTime, value); }
        }


        private string _msgColor;

        public string MsgColor
        {
            get { return _msgColor; }
            set { SetProperty<string>(ref _msgColor, value); }
        }


        private string _msgInfo;

        public string MsgInfo
        {
            get { return _msgInfo; }
            set { SetProperty<string>(ref _msgInfo, value); }
        }
    }
}
