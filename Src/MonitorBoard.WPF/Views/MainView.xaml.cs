using MonitorBoard.WPF.ViewModels;
using System.Timers;
using System.Windows;
using System.Windows.Threading;

namespace MonitorBoard.WPF.Views
{
    /// <summary>
    /// MainView.xaml 的交互逻辑
    /// </summary>
    public partial class MainView : Window
    {
        private DispatcherTimer _timer;
        public MainView()
        {
            InitializeComponent();
            DataContext = new MainViewModel();

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1); // 每秒触发一次
            _timer.Tick += _timer_Tick;
            _timer.Start();
        }

        private void _timer_Tick(object? sender, EventArgs e)
        {
            UpdateTime();
        }

        private void UpdateTime()
        {
            currentTimeText.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        // 可选：窗口关闭时停止定时器，避免内存泄漏
        protected override void OnClosed(EventArgs e)
        {
            _timer?.Stop();
            base.OnClosed(e);
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnMin_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
    }
}
