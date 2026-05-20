using LiveCharts;
using MonitorBoard.Communication.Modbus;
using MonitorBoard.WPF.Common;
using MonitorBoard.WPF.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO.Ports;
using System.Management;
using System.Text;

namespace MonitorBoard.WPF.ViewModels
{
    public class MainViewModel : NotifyBase
    {
        public ObservableCollection<PointPositionItemModel> PointPositionItems { get; } = new ObservableCollection<PointPositionItemModel>();

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

        private double _sysMemory;

        public double SysMemory
        {
            get { return _sysMemory; }
            set { SetProperty<double>(ref _sysMemory, value); }
        }

        private double _sysCpu;

        public double SysCpu
        {
            get { return _sysCpu; }
            set { SetProperty<double>(ref _sysCpu, value); }
        }


        private string _oledText;

        public string OledText
        {
            get { return _oledText; }
            set { SetProperty<string>(ref _oledText, value); }
        }

        public string SendText { get; set; } = "Hello";

        public int BeudRate { get; set; } = 9600;
        public string Parity { get; set; } = "None";
        public int DataBits { get; set; } = 8;
        public string StopBits { get; set; } = "One";
        public List<string> PortList { get; set; }
        public List<int> BeudRateList { get; set; } = new List<int> { 2400, 4800, 9600, 19200, 38400 };
        public List<string> ParityList { get; set; } = new List<string>();
        public List<int> DataBitsList { get; set; } = new List<int>();
        public List<string> StopBitsList { get; set; } = new List<string>();



        public ChartValues<double> TemperatureChartValues { get; set; } = new ChartValues<double> { 38, 70, 57, 62, 67, 27, 75, 56, 79, 20, 77, 46, 33, 63, 49, 56, 79, 20, 77, 46 };
        public ChartValues<double> HumidityChartValues { get; set; } = new ChartValues<double> { 46, 20, 30, 56, 57, 33, 76, 54, 74, 65, 66, 24, 71, 77, 58, 20, 30, 56, 57, 33 };
        public ChartValues<double> LuminanceChartValues { get; set; } = new ChartValues<double> { 56, 40, 20, 86, 17, 33, 56, 34, 74, 95, 16, 44, 11, 97, 18, 86, 17, 33, 56, 34 };
        public ObservableCollection<string> XLabels { get; set; } = new ObservableCollection<string> { };

        // 消息
        public MessageModel MessageModel { get; set; }

        // 设备
        public DeviceMeterModel DeviceMeterModel { get; set; }

        // 灯珠状态

        public ObservableCollection<DeviceLightModel> DeviceLights { get; set; }


        public CommandBase LightCommand { get; set; }

        public CommandBase MainLightCommand { get; set; }
        public CommandBase SendTextCommand { get; set; }
        public CommandBase ReSendTextCommand { get; set; }

        private static Dictionary<int, List<double>> DeviceMeterData = new Dictionary<int, List<double>>(); // 仪表数据
        public ObservableCollection<LogModel> LogItems { get; set; } = new ObservableCollection<LogModel>(); // 文本发送记录
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

            SendTextCommand = new CommandBase
            {
                DoExecute = new Action<object>((o) => OnSendText(o)),
                DoCanExecute = new Func<object, bool>((o) => { return true; })
            };

            ReSendTextCommand = new CommandBase
            {
                DoExecute = new Action<object>((o) => OnReSendText(o)),
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

            LogItems.Add(new LogModel { LogContext = "Hello001", SendTime = DateTime.Now });
            LogItems.Add(new LogModel { LogContext = "Hello002", SendTime = DateTime.Now });
            LogItems.Add(new LogModel { LogContext = "Hello003", SendTime = DateTime.Now });
            LogItems.Add(new LogModel { LogContext = "Hello004", SendTime = DateTime.Now });
            LogItems.Add(new LogModel { LogContext = "Hello005", SendTime = DateTime.Now });
            LogItems.Add(new LogModel { LogContext = "Hello006", SendTime = DateTime.Now });
            LogItems.Add(new LogModel { LogContext = "Hello007", SendTime = DateTime.Now });
            LogItems.Add(new LogModel { LogContext = "Hello008", SendTime = DateTime.Now });
            LogItems.Add(new LogModel { LogContext = "Hello009", SendTime = DateTime.Now });
            LogItems.Add(new LogModel { LogContext = "Hello010", SendTime = DateTime.Now });
            LogItems.Add(new LogModel { LogContext = "Hello011", SendTime = DateTime.Now });

            var now = DateTime.Now;
            // 初始化图标Lbale
            for (int i = 19; i > 0; i--)
            {
                XLabels.Add(now.AddMilliseconds(-(i * 3)).ToString("ss"));
            }

            Task.Run(async () =>
            {
                while (true)
                {
                    await Task.Delay(5000);

                    MonitorSystemSource();
                }
            });

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

        #region 发送文本
        private void OnSendText(object obj)
        {
            try
            {
                if (SendText.IsNull())
                {
                    throw new Exception("请输入消息内容");
                }

                if (SendText.Length > 60)
                {
                    throw new Exception("消息内容不能超过60");
                }

                SendOledText(SendText);
            }
            catch (Exception ex)
            {
                this.ShowMessage("发送异常", ex.Message, "Orange");
            }
        }
        private void SendOledText(string oledText)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(oledText);
            var result = modbusRtu.WriteBytes(1, 08, bytes.ToList());
            if (!result.Status)
            {
                throw new Exception(result.Message);
            }

            LogItems.Insert(0, new LogModel { LogContext = oledText, SendTime = DateTime.Now });
            OledText = oledText;
        }

        private void OnReSendText(object obj)
        {
            SendOledText(obj.ToString());
        }
        #endregion

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

                    if (DeviceMeterModel.IsOpenTemperatureSimulated)
                    {
                        var random = Random(DeviceMeterModel.TemperatureSimulatedMin, DeviceMeterModel.TemperatureSimulatedMax);
                        DeviceMeterModel.TemperatureValue = random;
                        DeviceMeterData[0].Add(random);
                        TemperatureChartValues.Add(random);
                    }
                    else
                    {
                        DeviceMeterModel.TemperatureValue = temperatureValue;
                        DeviceMeterData[0].Add(temperatureValue);
                        TemperatureChartValues.Add(temperatureValue);
                    }

                    if (DeviceMeterModel.IsOpenHumiditySimulated)
                    {
                        var random = Random(DeviceMeterModel.HumiditySimulatedMin, DeviceMeterModel.HumiditySimulatedMax);
                        DeviceMeterModel.HumidityValue = random;
                        DeviceMeterData[1].Add(random);
                        HumidityChartValues.Add(random);
                    }
                    else
                    {
                        DeviceMeterModel.HumidityValue = humidityValue;
                        DeviceMeterData[1].Add(humidityValue);
                        HumidityChartValues.Add(humidityValue);
                    }

                    if (DeviceMeterModel.IsOpenLuminanceSimulated)
                    {
                        var random = Random(DeviceMeterModel.LuminanceSimulatedMin, DeviceMeterModel.LuminanceSimulatedMax);
                        DeviceMeterModel.LuminanceValue = random;
                        DeviceMeterData[2].Add(random);
                        LuminanceChartValues.Add(random);
                    }
                    else
                    {
                        DeviceMeterModel.LuminanceValue = luminanceValue;
                        DeviceMeterData[2].Add(luminanceValue);
                        LuminanceChartValues.Add(luminanceValue);
                    }



                    // 移除第一个
                    if (DeviceMeterData[0].Count > 20) DeviceMeterData[0].RemoveAt(0);
                    if (DeviceMeterData[1].Count > 20) DeviceMeterData[1].RemoveAt(0);
                    if (DeviceMeterData[2].Count > 20) DeviceMeterData[2].RemoveAt(0);


                    if (TemperatureChartValues.Count > 20) TemperatureChartValues.RemoveAt(0);
                    if (HumidityChartValues.Count > 20) HumidityChartValues.RemoveAt(0);
                    if (LuminanceChartValues.Count > 20) LuminanceChartValues.RemoveAt(0);

                    XLabels.RemoveAt(0);
                    XLabels.Add(DateTime.Now.ToString("ss"));

                    // 计算最大最小值
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

        private Random random = new Random();

        private double Random(double min, double max)
        {
            return random.Next((int)min, (int)max);
        }
        #endregion

        #region 系统资源监控
        // 系统资源监控
        private async Task MonitorSystemSource()
        {
            try
            {
                // CPU
                using var cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
                cpuCounter.NextValue(); // 初始调用
                await Task.Delay(1000);

                SysCpu = (double)Math.Round(cpuCounter.NextValue(), 2);

                // 内存（WMI）
                // 获取总物理内存（来自 Win32_ComputerSystem）
                ulong totalMemory = 0;
                using (var searcher1 = new ManagementObjectSearcher("SELECT TotalPhysicalMemory FROM Win32_ComputerSystem"))
                {
                    foreach (var obj in searcher1.Get())
                    {
                        totalMemory = Convert.ToUInt64(obj["TotalPhysicalMemory"]);
                        break;
                    }
                }

                // 获取可用内存（来自性能计数器，单位：KB）
                ulong availableKBytes = 0;
                using (var searcher2 = new ManagementObjectSearcher("SELECT AvailableKBytes FROM Win32_PerfFormattedData_PerfOS_Memory"))
                {
                    foreach (var obj in searcher2.Get())
                    {
                        availableKBytes = Convert.ToUInt64(obj["AvailableKBytes"]);
                        break;
                    }
                }

                if (totalMemory > 0)
                {
                    double availableRatio = (double)(availableKBytes * 1024) / (double)totalMemory * 100; // 可用内存比例
                    SysMemory = Math.Round(availableRatio, 2);
                }
                else
                {
                    SysMemory = 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取系统资源信息失败: {ex}");
            }

        }
        #endregion
    }
}
