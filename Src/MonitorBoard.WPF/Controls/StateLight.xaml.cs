
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MonitorBoard.WPF.Controls
{
    public partial class StateLight : UserControl
    {
        // Command事件
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(StateLight), new PropertyMetadata(null));


        // Command参数
        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        public static readonly DependencyProperty CommandParameterProperty =
             DependencyProperty.Register(nameof(CommandParameter), typeof(object), typeof(StateLight),new PropertyMetadata(null));



        public bool IsChecked
        {
            get { return (bool)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }

        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register(nameof(IsChecked), typeof(bool), typeof(StateLight), new PropertyMetadata(false));



        public Brush BackColor
        {
            get { return (Brush)GetValue(BackColorProperty); }
            set { SetValue(BackColorProperty, value); }
        }

        public static readonly DependencyProperty BackColorProperty =
            DependencyProperty.Register(nameof(BackColor), typeof(Brush), typeof(StateLight), new PropertyMetadata(Brushes.Gray));


        public bool IsMain
        {
            get { return (bool)GetValue(IsMainProperty); }
            set { SetValue(IsMainProperty, value); }
        }

        public static readonly DependencyProperty IsMainProperty =
            DependencyProperty.Register(nameof(IsMain), typeof(bool), typeof(StateLight), new PropertyMetadata(false, new PropertyChangedCallback(OnValueChanged)));

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as StateLight;
            if (control == null) return;

            if (control.IsMain)
            {
                control.mainText.Visibility = Visibility.Visible;
                control.text.Visibility = Visibility.Hidden;
                control.lightText.Visibility = Visibility.Hidden;
            }
            else
            {
                control.mainText.Visibility = Visibility.Hidden;
                control.text.Visibility = Visibility.Visible;
                control.lightText.Visibility = Visibility.Visible;
            }
        }

        public StateLight()
        {
            InitializeComponent();
        }
    }
}
