using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MonitorBoard.WPF.Controls
{
    /// <summary>
    /// PointPosition.xaml 的交互逻辑
    /// </summary>
    public partial class PointPosition : UserControl
    {

        // 值
        public int Value
        {
            get { return (int)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty =
         DependencyProperty.Register(nameof(Value), typeof(int), typeof(PointPosition),
             new PropertyMetadata(default(int)));

        // 值
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty =
         DependencyProperty.Register(nameof(Title), typeof(string), typeof(PointPosition),
             new PropertyMetadata(string.Empty));


        // 中心颜色
        public Color CenterColor
        {
            get { return (Color)GetValue(CenterColorProperty); }
            set { SetValue(CenterColorProperty, value); }
        }

        public static readonly DependencyProperty CenterColorProperty =
            DependencyProperty.Register(nameof(CenterColor), typeof(Color), typeof(PointPosition),
                new PropertyMetadata(Colors.Blue));

        // 下边框颜色
        public Color BottomColor
        {
            get { return (Color)GetValue(BottomColorProperty); }
            set { SetValue(BottomColorProperty, value); }
        }

        public static readonly DependencyProperty BottomColorProperty =
            DependencyProperty.Register(nameof(BottomColor), typeof(Color), typeof(PointPosition),
                new PropertyMetadata(Colors.LightBlue));

        // 标题颜色
        public Brush TitleColor
        {
            get { return (Brush)GetValue(TitleColorProperty); }
            set { SetValue(TitleColorProperty, value); }
        }

        public static readonly DependencyProperty TitleColorProperty =
            DependencyProperty.Register(nameof(TitleColor), typeof(Brush), typeof(PointPosition),
                new PropertyMetadata(Brushes.Blue));

        // 数字颜色
        public Brush NumberColor
        {
            get { return (Brush)GetValue(NumberColorProperty); }
            set { SetValue(NumberColorProperty, value); }
        }

        public static readonly DependencyProperty NumberColorProperty =
            DependencyProperty.Register(nameof(NumberColor), typeof(Brush), typeof(PointPosition),
                new PropertyMetadata(Brushes.LightBlue));



        public PointPosition()
        {
            InitializeComponent();
        }
    }
}
