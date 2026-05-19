using System.Windows.Input;

namespace MonitorBoard.WPF.Common
{
    /// <summary>
    ///  委托命令基类，提供了一个简单的实现，可以通过设置 DoExecute 和 DoCanExecute 来定义命令的行为。
    /// </summary>
    public class CommandBase : ICommand
    {
        public event EventHandler? CanExecuteChanged;
        // 自动触发CanExecuteChanged，可能会影响性能
        //{
        //    add => CommandManager.RequerySuggested += value;
        //    remove => CommandManager.RequerySuggested -= value;
        //}

        public Action<object> DoExecute { get; set; }
        public Func<object, bool> DoCanExecute { get; set; }

        /// <summary>
        /// 手动触发 CanExecuteChanged 事件，通知 UI 重新查询命令状态
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 执行命令前调用此方法，触发 DoCanExecute 委托，判断命令是否可以执行。
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object? parameter)
        {
            return DoCanExecute?.Invoke(parameter) ?? true;
        }

        /// <summary>
        /// 执行命令时调用此方法，触发 OnExecute 委托，执行具体的命令逻辑。
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object? parameter)
        {
            DoExecute?.Invoke(parameter);
        }
    }
}
