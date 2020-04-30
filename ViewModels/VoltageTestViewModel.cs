using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using PortableEquipment.Servers.CommunicationProtocol;
using PortableEquipment.TestParameters;
using Stylet;
using StyletIoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace PortableEquipment.ViewModels
{
    public partial class VoltageTestViewModel : Screen, IHandle<MutualTranslator>, IHandle<OutTestResult>, IHandle<string>
    {
        #region 
        public IEventAggregator _eventAggregator;
        private MutualTranslator _mutualTranslator;
        [Inject]
        Servers.SqlDeel.ISqlHelp _sqlHelp;
        [Inject]
        Servers.CHangeVolate.ISetVolate _setVolate;
        [Inject]
        public ICommunicationProtocol _communicationProtocol;
        [Inject]
        public Servers.Xmldata.IXmlconfig _xmlconfig;
        [Inject]
        public StyletLogger.ILogger _logger;
        [Inject]
        public IWindowManager _windowManager;
        [Inject]
        public Servers.Json.IJsondeel _jsondeel;
        [Inject]
        public Servers.SelfCheck.ISelfCheck _SelfCheck;
        public VoltageTestViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);
            InitCreateChart();
        }
        #endregion
        #region Handle

        public void Handle(MutualTranslator message)
        {
            _mutualTranslator = message;
            GetBasePra(message);
        }

        public void Handle(OutTestResult message)
        {
            if (message.stataThree.Checked)
            {
                VolateUi = message.stataThree.AVolate;
                CurrentUi = message.stataThree.ACurrent;
                FreUi = message.stataThree.Fre;
            }

        }
        public void Handle(string CGF)
        {
            if (CGF != null)
            {
                try
                {
                    UVolateUi = Convert.ToDouble(CGF);
                }
                catch
                {
                    _logger.Writer("Cgf电压解析出错");
                }
            }
        }
        #endregion
    }
    public partial class VoltageTestViewModel
    {
        #region Commands
        /// <summary>
        /// datagrid
        /// </summary>
        /// <param name="ec"></param>
        private void LcGetTestVolateRange(ExcitationCharacteristic ec)
        {
            LcDatagrid.Clear();
            for (int i = 0; i < ec.VolateRange.Length; i++)
            {
                LcDatagrid.Add(new LcdatagridColletion
                {
                    TestNum = (i + 1).ToString(),
                    TestVoltage = (ec.TestVolate * ec.VolateRange[i] / 100).ToString(),
                    TestCurrent = ec.OverCurrent.ToString()
                });
            }
        }

        private void GetBasePra(MutualTranslator mutualTranslator)
        {
            TestId = mutualTranslator.TestId;
            if (mutualTranslator.TestLevel == 0)
                TestLevel = "35KV";
            else if (mutualTranslator.TestLevel == 1)
                TestLevel = "66KV";
            else if (mutualTranslator.TestLevel == 2)
                TestLevel = "110KV";
            else
                TestLevel = "220KV";
            Humidity = mutualTranslator.Humidity;
            Temperature = mutualTranslator.Temperature;
            LcOverCurrent = mutualTranslator.ExcitationCharacteristicR.OverCurrent;
            KzTestVolate = 0.00;
            KzOverCurrent = 0.00;

            KeepTestVoltage = 0.00;
            KeepTestFre = 0.00;
            KeepTestTime = 0.00;

            KeepHighOverVolate = mutualTranslator.InducedOvervoltageR.HighOverVolate + "kV";
        }
        public async void SaveTestResult()
        {
            await SetHide("正在保存数据", true);
            _mutualTranslator.Chartvalues = TanEleVolatevalue;
            _mutualTranslator.LcDatagrid = LcDatagrid;
            if (TanEleVolatevalue != null)
                _mutualTranslator.excitationCharacteristicResult.Chartvalues = TanEleVolatevalue;
            if (LcDatagrid != null)
                _mutualTranslator.excitationCharacteristicResult.LcDatagrid = LcDatagrid;
            _mutualTranslator.excitationCharacteristicResult.GuaiCueent = GuaiPointCurrent;
            _mutualTranslator.excitationCharacteristicResult.GuaiVolate = GuaiPointVolate;
            _mutualTranslator.inducedOvervoltageResult.Cueent = KeepTestCurrent;
            _mutualTranslator.inducedOvervoltageResult.Fre = KeepTestFre;
            _mutualTranslator.inducedOvervoltageResult.Volate = KeepTestVoltage;
            _mutualTranslator.noLoadCurrentResult.Current = KzOverCurrent;
            _mutualTranslator.noLoadCurrentResult.Volate = KzTestVolate;
            //处理其他结果
            _sqlHelp.SaveMutualTranslatorTransformerDataBase(_mutualTranslator);
            await SetHide("数据保存完成", true, true);
        }
        /// <summary>
        /// 获取拐点信息
        /// </summary>
        /// <returns></returns>
        private Tuple<string, string> GetGuaiMessage(List<ObservablePoint> observablePoints)
        {
            string guaivol = string.Empty;
            string guaicurent = string.Empty;

            var chartvalues = observablePoints.ToArray().ToList();
        regetguai: for (int i = chartvalues.Count - 1; i > 0; i--)
            {
                var vol = chartvalues[chartvalues.Count - 1].Y;
                if (chartvalues[i].Y < vol * 0.9)
                {
                    if ((Math.Abs(chartvalues[chartvalues.Count - 1].X - chartvalues[i].X) / chartvalues[i].X / (Math.Abs(vol - chartvalues[i].Y) /
                        chartvalues[i].Y)) > (5))
                    {
                        GuaiPointVolate = chartvalues[i].Y.ToString("N2") + "V";
                        GuaiPointCurrent = chartvalues[i].X.ToString("N2") + "A";
                        guaivol = GuaiPointVolate;
                        guaicurent = GuaiPointCurrent;
                        chartvalues.RemoveAt(chartvalues.Count - 1);
                        if (chartvalues.Count > observablePoints.Count * 0.1)
                            goto regetguai;
                        else
                            return new Tuple<string, string>(GuaiPointVolate, GuaiPointCurrent);
                    }
                }
            }
            if (guaivol != string.Empty && guaicurent != string.Empty)
                return new Tuple<string, string>(GuaiPointVolate, GuaiPointCurrent);
            else
            {
                return new Tuple<string, string>("NONE", "NONE");
            }
        }
        /// <summary>
        /// 获取完整charts数据，计算拐点信息
        /// </summary>
        /// <param name="chartValues"></param>
        /// <returns></returns>
        private List<ObservablePoint> GetAllCharyValues(List<StataThree> chartValues)
        {
            List<ObservablePoint> points = new List<ObservablePoint>();
            foreach (var item in chartValues)
            {
                points.Add(new ObservablePoint { X = item.ACurrent, Y = item.AVolate });
            }
            return points;
        }
        public async void CloseVolate()
        {
            await SetHide("正在退出...", true);
            if (IsRunning)
            {
                if (tokenSource != null)
                    tokenSource.Cancel();
                IsRunning = false;
            }
            if (await _communicationProtocol.GetPowerStata())
            {
                await SetHide("正在降压...", true);
                await _setVolate.DownVolateZero(_communicationProtocol, _xmlconfig);
                await SetHide("正在关闭电源...", true);
                await _setVolate.ControlsPowerStata(false, _communicationProtocol);
            }
            await SetHide("退出成功.", true);

            this.RequestClose();
        }
        #endregion
        #region TestCommand

        static CancellationTokenSource tokenSource = new CancellationTokenSource();
        CancellationToken token = tokenSource.Token;
        ManualResetEvent resetEvent = new ManualResetEvent(true);
        public bool OpenOrclose { get; set; }
        public async Task StartTest()
        {
            InitUi();
            await SetHide("已经开始测量", true, true);
            tokenSource = new CancellationTokenSource();
            token = tokenSource.Token;
            resetEvent = new ManualResetEvent(true);
            StartTestVisibility = false;
            Finish = true;
            if (_mutualTranslator.NoLoadCurrentR.Enable & Finish)
            {
                var ret = _windowManager.ShowMessageBox("是否开始空载电流试验", "提示", MessageBoxButton.OKCancel);
                if (ret == MessageBoxResult.OK)
                {
                    KeepHighOverVolate = "--";
                    LcOverCurrent = _mutualTranslator.NoLoadCurrentR.OverCurrent;
                    ControlsCurrentTestStata(false, false, true);
                    await StartKz();
                }

            }
            if (_mutualTranslator.ExcitationCharacteristicR.Enable && Finish)
            {
                var ret = _windowManager.ShowMessageBox("是否开始励磁特性试验", "提示", MessageBoxButton.OKCancel);
                if (ret == MessageBoxResult.OK)
                {
                    KeepHighOverVolate = "--";
                    LcOverCurrent = _mutualTranslator.ExcitationCharacteristicR.OverCurrent;
                    ControlsCurrentTestStata(false, true, false);
                    await StartLc();
                }
            }
            if (_mutualTranslator.InducedOvervoltageR.Enable & Finish)
            {
                var ret = _windowManager.ShowMessageBox("是否开始感应耐压试验", "提示", MessageBoxButton.OKCancel);
                if (ret == MessageBoxResult.OK)
                {
                    KeepHighOverVolate = _mutualTranslator.InducedOvervoltageR.HighOverVolate + "kV";
                    LcOverCurrent = _mutualTranslator.InducedOvervoltageR.OverCurrent;
                    ControlsCurrentTestStata(true, false, false);
                    await StartVolate();
                }
            }
            StartTestVisibility = true;
            ControlsCurrentTestStata(false, false, false);
            await SetHide("试验已经结束", true, true);
        }
        public async Task StartLc()
        {
            MutualTranslator mutualTranslator = _mutualTranslator;
            var range = mutualTranslator.ExcitationCharacteristicR.VolateRange;
            double[] lcdatas = new double[range.Length];
            for (int i = 0; i < range.Length; i++)
            {
                lcdatas[i] = range[i] / 100 * mutualTranslator.ExcitationCharacteristicR.TestVolate * 1000 / mutualTranslator.ExcitationCharacteristicR.VariableThan;
            }
            List<StataThree> Lcret = new List<StataThree>();
            bool[] Lc = new bool[lcdatas.Length];
            var NeedJd = Convert.ToDouble(_xmlconfig.GetAddNodeValue("UpvolateNeeddouble"));
            bool TestLcFlag = false;
            if (!IsRunning && await _SelfCheck.SeleCheck())
            {
                IsRunning = true; int redoint = 0;
            redo: if (await _setVolate.ControlsPowerStata(true, _communicationProtocol) &&
           await _setVolate.SettindVolate(10, _communicationProtocol, _xmlconfig) &&
           await _setVolate.SettingFre(Convert.ToDouble(_xmlconfig.GetAddNodeValue("LcFre")),
           _communicationProtocol) && await _communicationProtocol.ThicknessAdjustable(false))
                {
                    while (!TestLcFlag)
                    {
                        #region 调压 记录曲线的点
                        if (VolateUi < lcdatas[lcdatas.Length - 1])
                        {
                            await _communicationProtocol.SetTestPra(TestKind.ControlsVolateUP, 1);
                        }
                        else
                        {
                            await _communicationProtocol.SetTestPra(TestKind.ControlsVolateDown, 1);
                        }
                        if (token.IsCancellationRequested)
                        {
                            IsRunning = false;
                            Finish = false;
                            return;
                        }
                        resetEvent.WaitOne();
                        if (CurrentUi > mutualTranslator.ExcitationCharacteristicR.OverCurrent)
                        {
                            _windowManager.ShowMessageBox("励磁特性过流", "警告", MessageBoxButton.OK);
                            await _setVolate.DownAndClosePower(_communicationProtocol, _xmlconfig);
                            IsRunning = false;
                            Finish = false;
                            return;
                        }
                        if (VolateUi >= 10)
                        {
                            int i = 0;
                        qs: var p = await _communicationProtocol.ReadStataThree();
                            if (p.Checked)
                            {
                                Lcret.Add(p);//记录所有的点
                                AddNodePoint(p.ACurrent, p.AVolate);
                            }
                            else
                            {
                                if (i++ > 10)
                                    break;
                                goto qs;
                            }
                        }
                        #endregion
                        #region 励磁特性
                        for (int i = 0; i < lcdatas.Length; i++)//获取五个标准店
                        {
                            if (!Lc[i] && mutualTranslator.ExcitationCharacteristicR.Enable)
                            {
                                if (Math.Abs(VolateUi - lcdatas[i]) / lcdatas[i] < NeedJd)
                                {
                                here: var p = await _communicationProtocol.ReadStataThree();
                                    if (p.Checked)
                                    {
                                        LcDatagrid.Add(new LcdatagridColletion { TestNum = (i + 1).ToString(), TestCurrent = p.ACurrent.ToString(), TestVoltage = p.AVolate.ToString() });
                                    }
                                    else
                                    {
                                        goto here;
                                    }
                                    Lc[i] = true;
                                }
                            }
                            if (i == lcdatas.Length - 1 && Lc[i] == true)
                            {
                                TestLcFlag = true;
                            }
                        }
                        #endregion
                    }

                }
                else
                {
                    if (redoint++ > 5)
                        _windowManager.ShowMessageBox("励磁特性开始试验失败\t\n试验初始化出错", "警告", MessageBoxButton.OK);
                    goto redo;
                }
                _logger.Writer(_jsondeel.GetJsonByclass(Lcret));
                var guaret = GetGuaiMessage(GetAllCharyValues(Lcret));
                GuaiPointVolate = guaret.Item1;
                GuaiPointCurrent = guaret.Item2;
            }
            await _setVolate.DownAndClosePower(_communicationProtocol, _xmlconfig);
            IsRunning = false;
        }
        public async Task StartVolate()
        {
            var mutualTranslator = _mutualTranslator;
            var Presuarvolate = mutualTranslator.InducedOvervoltageR.TestVoltage;
            var NeedJd = Convert.ToDouble(_xmlconfig.GetAddNodeValue("UpvolateNeeddouble"));

            #region 耐压
            if (!IsRunning && mutualTranslator.InducedOvervoltageR.Enable && await _SelfCheck.SeleCheck())
            {
                IsRunning = true;
                if (await _setVolate.SettindVolate(15, _communicationProtocol, _xmlconfig) && await _setVolate.SettingFre(mutualTranslator.InducedOvervoltageR.TestFre, _communicationProtocol))
                {
                    if (await _setVolate.SettindVolate(Presuarvolate, _communicationProtocol, _xmlconfig))
                    {

                        for (int i = 0; i < ((int)mutualTranslator.InducedOvervoltageR.TestTime); i++)
                        {
                            if (token.IsCancellationRequested)
                            {
                                IsRunning = false;
                                Finish = false;
                                return;
                            }
                            resetEvent.WaitOne();
                            var p1 = await _communicationProtocol.ReadStataThree();
                            if (p1.Checked)
                            {
                                if (p1.ACurrent > mutualTranslator.InducedOvervoltageR.OverCurrent)
                                {
                                    _windowManager.ShowMessageBox("交流耐压过流", "警告", MessageBoxButton.OK);
                                    await _setVolate.DownAndClosePower(_communicationProtocol, _xmlconfig);
                                    IsRunning = false;
                                    Finish = false;
                                    return;
                                }
                            }
                            if (VolateUi >= mutualTranslator.InducedOvervoltageR.HighOverVolate * 1000 / mutualTranslator.InducedOvervoltageR.VariableThan)
                            {
                                _windowManager.ShowMessageBox("过压保护", "警告", MessageBoxButton.OK);
                                await _setVolate.DownAndClosePower(_communicationProtocol, _xmlconfig);
                                IsRunning = false;
                                return;
                            }
                            PresurreTime = ((int)mutualTranslator.InducedOvervoltageR.TestTime - i - 1).ToString();
                            await Task.Delay(1000);
                        }
                        KeepTestVoltage = VolateUi;
                        KeepTestFre = FreUi;
                        KeepTestTime = (int)mutualTranslator.InducedOvervoltageR.TestTime;
                        KeepTestCurrent = CurrentUi;
                    }
                    else
                    {
                        _windowManager.ShowMessageBox("调节耐压值失败\t\n请检查", "警告", MessageBoxButton.OK);
                    }
                }
                else
                {
                    _windowManager.ShowMessageBox("调节频率失败\t\n请检查", "警告", MessageBoxButton.OK);
                }
            }
            await _setVolate.DownAndClosePower(_communicationProtocol, _xmlconfig);
            IsRunning = false;
            #endregion
        }
        public async Task StartKz()
        {
            var mutualTranslator = _mutualTranslator;
            var NoLoadCurrentRVolate = mutualTranslator.NoLoadCurrentR.TestVolate * 1000 / mutualTranslator.NoLoadCurrentR.VariableThan;
            var NeedJd = Convert.ToDouble(_xmlconfig.GetAddNodeValue("UpvolateNeeddouble"));
            #region 空载电流
            if (!IsRunning && mutualTranslator.NoLoadCurrentR.Enable)
            {
                IsRunning = true;
                PresurreTime = "30";
                if (await _setVolate.SettindVolate(15, _communicationProtocol, _xmlconfig) &&
                    await _setVolate.SettingFre(Convert.ToDouble(_xmlconfig.GetAddNodeValue("KzFre")), _communicationProtocol) &&
                    await _setVolate.SettindVolate(NoLoadCurrentRVolate, _communicationProtocol, _xmlconfig))
                {
                    var p = await _communicationProtocol.ReadStataThree();
                    if (p.ACurrent < mutualTranslator.NoLoadCurrentR.OverCurrent)
                    {
                        for (int i = 0; i < 30; i++)
                        {
                            if (token.IsCancellationRequested)
                            {
                                IsRunning = false;
                                Finish = false;
                                return;
                            }
                            resetEvent.WaitOne();
                            PresurreTime = (30 - i - 1).ToString();
                            await Task.Delay(1000);
                        }
                        KzOverCurrent = p.ACurrent;
                        KzTestVolate = p.AVolate;

                    }
                    else
                    {
                        KzOverCurrent = p.ACurrent;
                        KzTestVolate = p.AVolate;
                        _windowManager.ShowMessageBox("空载电流过流保护", "警告", MessageBoxButton.OK);
                        IsRunning = false;
                        Finish = false;
                        return;
                    }
                }
                else
                {
                    _windowManager.ShowMessageBox("调节电压值失败\t\n请检查", "警告", MessageBoxButton.OK);
                }
            }
            await _setVolate.DownAndClosePower(_communicationProtocol, _xmlconfig);
            IsRunning = false;
            #endregion
        }
        /// <summary>
        /// 中断测量过程
        /// </summary>
        public async void FinishTest()
        {
            await SetHide("正在结束试验...", true);
            tokenSource.Cancel();
            IsRunning = false;
            Finish = false;
            await _setVolate.DownAndClosePower(_communicationProtocol, _xmlconfig);
            await Task.Delay(1000);
            await SetHide("试验已结束", true, true);
            StartTestVisibility = true;
        }

        private void ControlsCurrentTestStata(bool ctrols1, bool ctrols2, bool ctrols3)
        {
            FirstTestEnable = ctrols1;
            SeconedTestEnable = ctrols2;
            ThirdTestEnable = ctrols3;
        }
        private async Task SetHide(string hidemessage, bool IsOpen, bool isTimeout = false, int timeout = 1000)
        {
            OpenOrclose = IsOpen;
            HideText = hidemessage;
            if (isTimeout)
            {
                await Task.Delay(timeout);
                IsOpen = false;
                OpenOrclose = false;
            }
        }
        #endregion

        public void InitUi()
        {
            InitCreateChart();
            LcDatagrid.Clear();
            GuaiPointVolate = "--";
            GuaiPointCurrent = "--";
            KzTestVolate = 0;
            KzOverCurrent = 0;
            KeepTestVoltage = 0;
            KeepTestFre = 0;
            KeepTestTime = 0;
        }
    }
    public partial class VoltageTestViewModel
    {
        #region TestFlag
        private bool _isrunning;
        public bool IsRunning
        {
            get
            {
                return _isrunning;
            }
            set
            {
                _isrunning = value;
                if (_isrunning)
                {
                    StartTestText = "测量中...";
                    StartOpacity = 0.5;
                    BarVisibilityEnable = Visibility.Visible; ;
                }
                else
                {
                    BarVisibilityEnable = Visibility.Collapsed;
                    StartOpacity = 1;
                    StartTestText = "启动测量";
                }
            }
        }
        private bool Finish = true;
        #endregion

        #region 实时ui
        public double UVolateUi { get; set; }
        public double VolateUi { get; set; }
        public double FreUi { get; set; }
        public double CurrentUi { get; set; }


        public bool FirstTestEnable { get; set; } = false;
        public bool SeconedTestEnable { get; set; } = false;
        public bool ThirdTestEnable { get; set; } = false;


        #endregion

        #region TestBinding
        private Flag _Flag;
        public Flag Flag
        {
            get { return _Flag; }
            set
            {
                _Flag = value;
                if ((int)_Flag >= 1 && (int)_Flag <= 3)
                {
                    StartTestVisibility = false;
                }
                else
                {
                    StartTestVisibility = true;
                }
            }
        }
        //耐压倒计时
        public string PresurreTime { get; set; } = "--";
        public bool StartTestVisibility { get; set; } = true;
        public string GuaiPointVolate { get; set; } = "0.00kV";
        public string GuaiPointCurrent { get; set; } = "0.00A";
        #endregion

        #region chart
        public Func<double, string> XFormatter { get; set; }
        public Func<double, string> YFormatter { get; set; }
        public SeriesCollection LcCurrentVolate { get; set; }
        public ChartValues<ObservablePoint> TanEleVolatevalue { get; set; }
        /// <summary>
        /// 初始化chart表格
        /// </summary>
        public void InitCreateChart()
        {
            TanEleVolatevalue = new ChartValues<ObservablePoint>();
            LineSeries t1 = new LineSeries
            {
                StrokeThickness = 2,
                Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(28, 142, 196)),
                Fill = System.Windows.Media.Brushes.Transparent,
                LineSmoothness = 10,//0为折现样式
                PointGeometrySize = 0,
                PointForeground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(34, 46, 49)),
                Values = TanEleVolatevalue
            };
            LcCurrentVolate = new SeriesCollection { };
            LcCurrentVolate.Add(t1);
            LcCurrentVolate[0].Values.Add(new ObservablePoint(0, 0));

            XFormatter = val => (val).ToString("N1") + "A";
            YFormatter = val => (val).ToString("N1") + " V";
        }

        public void AddNodePoint(double Xva, double yVa)
        {
            ObservablePoint a = new ObservablePoint(Xva, yVa);
            LcCurrentVolate[0].Values.Add(a);
        }
        #endregion

        #region Bindings
        public BindableCollection<LcdatagridColletion> LcDatagrid { get; set; } = new BindableCollection<LcdatagridColletion>();

        public string TestId { get; set; }
        public string TestLevel { get; set; }
        public double Humidity { get; set; }
        public double Temperature { get; set; }

        public double KzTestVolate { get; set; }
        public double KzOverCurrent { get; set; }
        public double LcOverCurrent { get; set; }

        public double KeepTestVoltage { get; set; }
        public double KeepTestFre { get; set; }
        public double KeepTestTime { get; set; }
        public double KeepTestCurrent { get; set; }
        public string KeepHighOverVolate { get; set; }

        public bool IsTopDrawerOpen { get; set; } = false;

        public string HideText { get; set; } = "保存成功";
        public string StartTestText { get; set; } = "启动测量";
        public Visibility BarVisibilityEnable { get; set; } = Visibility.Collapsed;
        public double StartOpacity { get; set; } = 1.00;
        #endregion
    }
    #region Pra
    public class LcdatagridColletion
    {
        public string TestNum { get; set; }
        public string TestVoltage { get; set; }
        public string TestCurrent { get; set; }
    }
    public struct MulTestResult
    {
        public MutualTranslator mutualTranslatorPra;
        public BindableCollection<LcdatagridColletion> LcDatagrid;
        public double TurningpointVolate;
        public double TurningpointCurrent;

        public double KzVolate;
        public double KzCurrent;

        public double PressureVolata;
        public double PressureFre;
        public double PressureTime;
    }
    public enum Flag
    {
        Free = 0,
        RunningPressure = 1,
        RunningLc = 2,
        RunningKz = 3,
        False_1 = 4,
        False_2 = 5,
        False_3 = 6,
        FinishPressure = 7,
        FinishLc = 8,
        FinishKz = 9,
        FinishByhand = 10

    }
    #endregion

}
