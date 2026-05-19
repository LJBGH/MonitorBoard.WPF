using System.Windows;
using System.Windows.Controls;

namespace MonitorBoard.WPF.Controls
{
    /// <summary>
    /// DataBlock.xaml 的交互逻辑
    /// </summary>
    public partial class DataBlock : UserControl
    {


        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(double), typeof(DataBlock), new PropertyMetadata(0.0));




        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register(nameof(Header), typeof(string), typeof(DataBlock), new PropertyMetadata(string.Empty));



        public DataBlock()
        {
            InitializeComponent();
        }
    }
}
