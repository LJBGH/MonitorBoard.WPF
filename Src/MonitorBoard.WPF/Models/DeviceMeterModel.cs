using MonitorBoard.WPF.Common;

namespace MonitorBoard.WPF.Models
{
    public class DeviceMeterModel : NotifyBase
    {

        public bool IsOpenTemperatureSimulated { get; set; } = false;

        public double TemperatureSimulatedMin { get; set; } = -10;
        public double TemperatureSimulatedMax { get; set; } = 80;


        public bool IsOpenHumiditySimulated { get; set; } = false;

        public double HumiditySimulatedMin { get; set; } = 0;
        public double HumiditySimulatedMax { get; set; } = 100;


        public bool IsOpenLuminanceSimulated { get; set; } = false;

        public double LuminanceSimulatedMin { get; set; } = -1;
        public double LuminanceSimulatedMax { get; set; } = 100;


        private double _temperatureValue;

        /// <summary>
        /// 当前温度
        /// </summary>
        public double TemperatureValue
        {
            get { return _temperatureValue; }
            set { SetProperty<double>(ref _temperatureValue, value); }
        }

        private double _temperatureMin;

        /// <summary>
        /// 最低温度
        /// </summary>
        public double TemperatureMin
        {
            get { return _temperatureMin; }
            set { SetProperty<double>(ref _temperatureMin, value); }
        }

        private double _temperatureMax;

        /// <summary>
        /// 最高温度
        /// </summary>
        public double TemperatureMax
        {
            get { return _temperatureMax; }
            set { SetProperty<double>(ref _temperatureMax, value); }
        }

        private double _humidityValue;

        /// <summary>
        /// 当前湿度
        /// </summary>
        public double HumidityValue
        {
            get { return _humidityValue; }
            set { SetProperty<double>(ref _humidityValue, value); }
        }

        private double _humidityMin;

        /// <summary>
        ///最低湿度
        /// </summary>
        public double HumidityMin
        {
            get { return _humidityMin; }
            set { SetProperty<double>(ref _humidityMin, value); }
        }

        private double _humidityMax;

        /// <summary>
        /// 最高湿度
        /// </summary>
        public double HumidityMax
        {
            get { return _humidityMax; }
            set { SetProperty<double>(ref _humidityMax, value); }
        }


        private double _luminanceValue;


        /// <summary>
        /// 最低亮度
        /// </summary>
        public double LuminanceValue
        {
            get { return _luminanceValue; }
            set { SetProperty<double>(ref _luminanceValue, value); }
        }


        private double _luminanceMin;


        /// <summary>
        /// 最低亮度
        /// </summary>
        public double LuminanceMin
        {
            get { return _luminanceMin; }
            set { SetProperty<double>(ref _luminanceMin, value); }
        }

        private double _luminanceMax;


        /// <summary>
        /// 最高亮度
        /// </summary>
        public double LuminanceMax
        {
            get { return _luminanceMax; }
            set { SetProperty<double>(ref _luminanceMax, value); }
        }
    }
}
