using MonitorBoard.Communication.Modbus;
using MonitorBoard.WPF.Common;
using MonitorBoard.WPF.Models;
using System.Collections.ObjectModel;
using System.IO.Ports;

namespace MonitorBoard.WPF.ViewModels
{
    public class MainViewModel : NotifyBase
    {
        public ObservableCollection<PointPositionItemModel> PointPositionItems { get; } = new ObservableCollection<PointPositionItemModel>();
        public ObservableCollection<LogModel> LogItems { get; } = new ObservableCollection<LogModel>();


        // 连接状态
        private bool _connState;

        public bool ConnState
        {
            get { return _connState; }
            set
            {
                if (!ConnState)
                {
                    try
                    {
                        OnConnent();
                        SetProperty<bool>(ref _connState, value);
                        ShowMessage("系统通知", "设备连接成功，正在监听设备状态...");
                    }
                    catch (Exception ex)
                    {
                        ShowMessage("异常通知", $"串口连接失败：{ex.Message}", "Red");
                    }
                }
                else
                {
                    try
                    {
                        OnClose();
                        SetProperty<bool>(ref _connState, value);
                        ShowMessage("系统通知", $"串口关闭成功");
                    }
                    catch (Exception ex)
                    {
                        ShowMessage("异常通知", $"串口关闭失败：{ex.Message}", "Red");
                    }
                }
            }
        }

        // 窗口名称
        private string _portName;

        private ModbusRtu modbusRtu;

        public string PortName
        {
            get { return _portName; }
            set { SetProperty<string>(ref _portName, value); }
        }

        private bool _isMainOpen;

        public bool IsMainOpen
        {
            get { return _isMainOpen; }
            set { SetProperty<bool>(ref _isMainOpen, value); }
        }

        public int BeudRate { get; set; } = 9600;
        public string Parity { get; set; } = "None";
        public int DataBits { get; set; } = 8;
        public string StopBits { get; set; } = "One";
        public List<string> PortList { get; set; }
        public List<int> BeudRateList { get; set; } = new List<int> { 2400, 4800, 9600, 19200, 38400 };
        public List<string> ParityList { get; set; } = new List<string>();
        public List<int> DataBitsList { get; set; } = new List<int>();
        public List<string> StopBitsList { get; set; } = new List<string>();

        private static Dictionary<int, List<double>> DeviceMeterData = new Dictionary<int, List<double>>(); // 仪表数据

        // 消息
        public MessageModel MessageModel { get; set; }

        // 设备
        public DeviceMeterModel DeviceMeterModel { get; set; }

        // 灯珠状态

        public ObservableCollection<DeviceLightModel> DeviceLights { get; set; }


        public CommandBase LightCommand { get; set; }

        public CommandBase MainLightCommand { get; set; }


        public MainViewModel()
        {
            MessageModel = new MessageModel();
            DeviceMeterModel = new DeviceMeterModel();
            DeviceLights = new ObservableCollection<DeviceLightModel>()
            {
                new DeviceLightModel{ IsChecked = false},
                new DeviceLightModel{ IsChecked = false},
                new DeviceLightModel{ IsChecked = false},
                new DeviceLightModel{ IsChecked = false},
                new DeviceLightModel{ IsChecked = false}
            };

            LightCommand = new CommandBase
            {
                DoExecute = new Action<object>((o) => DoLightChanged(o)),
                DoCanExecute = new Func<object, bool>((o) => { return true; })
            };


            MainLightCommand = new CommandBase
            {
                DoExecute = new Action<object>((o) => DoMainLightChanged(o)),
                DoCanExecute = new Func<object, bool>((o) => { return true; })
            };

            InitData();
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        private void InitData()
        {
            // 初始化数据
            PortList = SerialPort.GetPortNames().ToList();
            PortName = PortList.FirstOrDefault();
            ParityList = Enum.GetNames<Parity>().ToList();
            DataBitsList = new List<int> { 8 };
            StopBitsList = Enum.GetNames<StopBits>().ToList();

            DeviceMeterData.Add(0, new List<double> { });
            DeviceMeterData.Add(1, new List<double> { });
            DeviceMeterData.Add(2, new List<double> { });

            ShowMessage("系统提示", "等待连接设备");
        }

        /// <summary>
        /// 连接
        /// </summary>
        private void OnConnent()
        {
            if (PortName.IsNull())
            {
                throw new Exception("请选择串口");
            }

            OnClose();

            modbusRtu = new ModbusRtu(PortName, BeudRate, DataBits, (Parity)Enum.Parse(typeof(Parity), Parity), (StopBits)Enum.Parse(typeof(StopBits), StopBits), 2000);
            //serialPort.PortName = PortName;
            //serialPort.BaudRate = BeudRate;
            //serialPort.Parity = (Parity)Enum.Parse(typeof(Parity), Parity);
            //serialPort.DataBits = DataBits;
            //serialPort.StopBits = (StopBits)Enum.Parse(typeof(StopBits), StopBits);
            var result = modbusRtu.Open(1000);

            if (!result.Status)
            {
                throw new Exception(result.Message);
            }

            Task.Run(async () =>
            {
                while (modbusRtu != null)
                {
                    OnMonitor();
                    await Task.Delay(3000);
                }
            });
        }

        /// <summary>
        /// 关闭
        /// </summary>
        private void OnClose()
        {
            if (modbusRtu != null)
            {
                modbusRtu.Close();
            }

            modbusRtu = null;
        }

        // 显示消息
        private void ShowMessage(string title, string msgInfo, string msgColor = "#1FDCF5")
        {
            MessageModel.Title = title;
            MessageModel.MsgInfo = msgInfo;
            MessageModel.MsgColor = msgColor;
            MessageModel.MsgTime = DateTime.Now;
        }

        /// <summary>
        /// 灯珠状态切换
        /// </summary>
        /// <param name="obj"></param>
        private void DoLightChanged(object obj)
        {
            try
            {
                int index = int.Parse(obj?.ToString());
                var status = DeviceLights[index].IsChecked;

                var result = modbusRtu.WriteCoils(1, (ushort)index, new List<bool> { status });
                if (!result.Status)
                {
                    DeviceLights[index].IsChecked = status;
                    ShowMessage("操作异常", result.Message, "Red");
                    return;
                }

                bool allOn = DeviceLights.All(x => x.IsChecked);

                if (allOn)
                    IsMainOpen = true;
                else
                    IsMainOpen = false; 

                var str = status ? "关闭" : "打开";
                ShowMessage("操作成功", $"灯珠{str}成功!");
            }
            catch (Exception ex)
            {
                ShowMessage("操作异常", "灯珠切换失败", "Red");
            }
        }

        /// <summary>
        /// 统一控制灯珠
        /// </summary>
        /// <param name="obj"></param>
        private void DoMainLightChanged(object obj)
        {
            try
            {
                var status = IsMainOpen;
                if (status)
                {
                    var result = modbusRtu.WriteCoils(1, 0, DeviceLights.Select(x => true).ToList());
                    if (!result.Status)
                    {
                        IsMainOpen = !status;
                        ShowMessage("操作异常", result.Message, "Red");
                        return;
                    }

                    foreach (var item in DeviceLights)
                    {
                        item.IsChecked = true;
                    }
                }
                else 
                {
                    var result = modbusRtu.WriteCoils(1, 0, DeviceLights.Select(x => false).ToList());
                    if (!result.Status)
                    {
                        IsMainOpen = !status;
                        ShowMessage("操作异常", result.Message, "Red");
                        return;
                    }

                    foreach (var item in DeviceLights)
                    {
                        item.IsChecked = false;
                    }
                }

                var str = status ? "关闭" : "打开";
                ShowMessage("操作成功", $"灯珠全部{str}成功!");
            }
            catch (Exception ex)
            {
                ShowMessage("操作异常", "灯珠切换失败", "Red");
            }
        }
        

        #region 读取设备数据
        private void OnMonitor()
        {
            if (modbusRtu != null)
            {
                // 读取仪表数据
                ReadDeviceMeter();

                // 读取灯珠数据
                ReadLight();
            }
        }

        /// <summary>
        /// 读取仪表数据
        /// </summary>
        private void ReadDeviceMeter()
        {
            try
            {
                var result = modbusRtu.ReadRegisters<ushort>(1, 0x03, 0, 3);
                if (!result.Status)
                {
                    return;
                }

                if (result.Datas != null && result.Datas.Count == 3)
                {
                    double temperatureValue = Math.Round(result.Datas[0] / 10.0, 1);
                    double humidityValue = Math.Round(result.Datas[1] / 10.0, 1);
                    double luminanceValue = Math.Round(result.Datas[2] / 10.0, 1);


                    DeviceMeterModel.TemperatureValue = temperatureValue;
                    DeviceMeterModel.HumidityValue = humidityValue;
                    DeviceMeterModel.LuminanceValue = luminanceValue;

                    DeviceMeterData[0].Add(temperatureValue);
                    DeviceMeterData[1].Add(humidityValue);
                    DeviceMeterData[2].Add(luminanceValue);

                    DeviceMeterModel.TemperatureMin = DeviceMeterData[0].Min();
                    DeviceMeterModel.TemperatureMax = DeviceMeterData[0].Max();

                    DeviceMeterModel.HumidityMin = DeviceMeterData[1].Min();
                    DeviceMeterModel.HumidityMax = DeviceMeterData[1].Max();

                    DeviceMeterModel.LuminanceMin = DeviceMeterData[2].Min();
                    DeviceMeterModel.LuminanceMax = DeviceMeterData[2].Max();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// 读取灯珠数据
        /// </summary>
        private void ReadLight()
        {
            var result = modbusRtu.ReadCoils(1, 0x01, 0, 5);
            if (result.Datas != null && result.Datas.Count == 5)
            {
                for (int i = 0; i < result.Datas.Count; i++)
                {
                    if (DeviceLights[i].IsChecked != result.Datas[i])
                    {
                        DeviceLights[i].IsChecked = result.Datas[i];
                    }
                }
            }
        }
        #endregion
    }
}
