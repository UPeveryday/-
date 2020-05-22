using PortableEquipment.Servers.CommunicationProtocol;
using PortableEquipment.TestParameters;
using Stylet;
using StyletIoC;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace PortableEquipment.ViewModels
{
    public partial class TransformerViewModel : Screen, IHandle<Translator>, IHandle<OutTestResult>, IHandle<string>, IHandle<JfTestResult>, IDisposable
    {
        #region 依赖注入
        [Inject]
        public ICommunicationProtocol _communicationProtocol;
        [Inject]
        public Servers.CHangeVolate.ISetVolate _setVolate;
        [Inject]
        public Servers.Xmldata.IXmlconfig _xmlconfig;
        [Inject]
        public Servers.SqlDeel.ISqlHelp _sqlHelp;
        [Inject]
        public StyletLogger.ILogger _logger;
        [Inject]
        public TimeViewModel _TimeViewModel;
        [Inject]
        public IContainer _container;
        [Inject]
        public IWindowManager _windowManager;
        [Inject]
        public Servers.SelfCheck.ISelfCheck _SelfCheck;

        [Inject]
        public ViewModels.JfViewModel _JfViewModel;
        public IEventAggregator _eventAggregator;
        public TransformerViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);
        }

        public void Dispose()
        {
            this.OnClose();
        }

        //Ui更新
        public void Handle(OutTestResult message)
        {
            if (message.stataThree.Checked)
            {
                CurrentUi = message.stataThree.ACurrent;
                VolateUi = message.stataThree.AVolate;
                FreUi = message.stataThree.Fre;
                DataPower = message.stataThree.APower;
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

        public void Handle(JfTestResult jfc)
        {
            JfStataControl = jfc.Jffinish;
            JfData = jfc.Jfdata;
        }
        #endregion
    }

    public partial class TransformerViewModel
    {
        public void Handle(Translator message)
        {
            TestPra = message;
            TestId = message.TestId;
            RatedVoltage = Models.StaticClass.RetStringVolate(message.RatedVoltage);
            RatedCapacity = message.RatedCapacity;
            WindingGroup = message.WindingGroup;
            Temperature = message.Temperature;
            Humidity = message.Humidity;
            DatagridData = message.DatagridData;
            Fre = message.Frequency;
            InitDataGrid(message.DatagridData.ToArray());
        }
        public void SaveManagMent()
        {
            if (DatagridTestData != null)
                TestPra.TestResultData = DatagridTestData;
            if (_sqlHelp.SaveTransformerDataBase(TestPra))
            {
                _windowManager.ShowMessageBox("保存成功", "提示", MessageBoxButton.OK);
            }
            else
            {
                _windowManager.ShowMessageBox("保存失败", "提示", MessageBoxButton.OK);
            }
        }
        private void InitDataGrid(TransformerDataStep[] data)
        {
            DatagridTestData.Clear();
            for (int i = 0; i < data.Length; i++)
            {
                for (int j = 0; j < (int)(data[i].TestTime / 5); j++)
                {
                    Addtestdata(data[i].Stepname, ((j + 1) * 5).ToString(), data[i].TestVolate.ToString());
                }
                if (data[i].TestTime % 5 != 0)
                    Addtestdata(data[i].Stepname, data[i].TestTime.ToString(), data[i].TestVolate.ToString());
            }
        }
        private int GetIndexFromDataGrid(TransformerDataStep[] transformerDataStep, int currentvolateIndex, double currenttesttime)
        {
            int headnum = 0;
            for (int i = 0; i < currentvolateIndex; i++)
            {
                for (int j = 0; j < (int)(transformerDataStep[i].TestTime / 5); j++)
                {
                    headnum++;
                }
                if (transformerDataStep[i].TestTime % 5 != 0)
                    headnum++;
            }
            for (int j = 0; j < (int)currenttesttime / 5; j++)
            {
                headnum++;
            }
            if (currenttesttime % 5 != 0)
                headnum++;
            return headnum;
        }
        /// <summary>
        /// ADd 增加结果
        /// </summary>
        /// <param name="GetIndexFromDataGrid">获取需要添加的行</param>
        /// <param name="MarkPhama"></param>
        private void AddTestData(Translator TestPra, int currentvolateIndex, double currenttesttime, int MarkPhama, string Jf)
        {
            int p = 0;
            foreach (var item in DatagridTestData)
            {
                if (p == GetIndexFromDataGrid(TestPra.DatagridData.ToArray(), currentvolateIndex, currenttesttime) - 1)
                {
                    if (MarkPhama == 1)
                        item.UVoltage = Jf.ToString();
                    if (MarkPhama == 2)
                        item.VVoltage = Jf.ToString();
                    if (MarkPhama == 3)
                        item.WVoltage = Jf.ToString();
                    DatagridTestData.Refresh();
                }
                p++;
            }
        }
        /// <summary>
        /// 添加datagrid数据
        /// </summary>
        private void Addtestdata(string stepname, string testtime, string testvolate)
        {
            DatagridTestData.Add(new TransformerTestData
            {
                Stepname = stepname,
                TestTime = testtime,
                TestVoltage = testvolate
            });
        }
        public void StartJf()
        {
            SaveFlag = true;
            JfData = 0;
        }

        public async void TransformerClose()
        {

            WindowsStataIsOpen = false;
            foreach (var item in Process.GetProcessesByName("数字式局部放电检测系统7.0"))
            {
                item.Kill();
                await Task.Delay(10);
            }

            if (_container.Get<JfViewModel>().ScreenState == ScreenState.Active)
                _JfViewModel.RequestClose();
            try
            {
                if (IsRunning)
                {
                    if (tokenSource != null)
                        tokenSource.Cancel();
                }
                if (await _communicationProtocol.GetPowerStata())
                {
                    await SetHide("正在降压...", true);
                    await _setVolate.DownVolateZero(_communicationProtocol, _xmlconfig, new CancellationToken());
                    await SetHide("正在关闭电源", true, true);
                    await _setVolate.ControlsPowerStata(false, _communicationProtocol, new CancellationToken());
                }
            }
            catch
            {
                if (await _communicationProtocol.GetPowerStata())
                {
                    await SetHide("正在降压...", true);
                    await _setVolate.DownVolateZero(_communicationProtocol, _xmlconfig, new CancellationToken());
                    await SetHide("正在关闭电源", true, true);
                    await _setVolate.ControlsPowerStata(false, _communicationProtocol, new CancellationToken());
                }
                IsRunning = false;
                this.RequestClose();
            }

            this.RequestClose();
        }

    }
    public partial class TransformerViewModel
    {
        #region header提示
        public bool OpenOrcloseHeader { get; set; }
        public string HideText { get; set; }
        private async Task SetHide(string hidemessage, bool IsOpen, bool isTimeout = false, int timeout = 1000)
        {
            OpenOrcloseHeader = IsOpen;
            HideText = hidemessage;
            if (isTimeout)
            {
                await Task.Delay(timeout);
                IsOpen = false;
                OpenOrcloseHeader = false;
            }
        }
        #endregion
        #region 局放电源设置
        public bool SaveFlag { get; set; }
        public bool JfStataControl { get; set; }
        public double JfData { get; set; }
        public bool WindowsStataIsOpen { get; set; } = false;

        public bool ShowOrHide { get; set; }

        public string hidetest { get; set; } = "空闲中";
        #endregion 
        #region 试验状态控制
        /// <summary>
        /// 按下确定或者取消，是否开始试验
        /// </summary>
        private bool IsokOrCan;
        public string TesttingStata { get; set; }
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
                    BarVisibility = Visibility.Visible; ;
                }
                else
                {
                    StartOpacity = 1;
                    BarVisibility = Visibility.Collapsed;
                    StartTestText = "启动测量";
                }
            }
        }
        public bool OpenOrclose { get; set; }
        public string StartTestText { get; set; } = "启动测量";
        public double StartOpacity { get; set; } = 1;
        public Visibility BarVisibility { get; set; } = Visibility.Collapsed;

        /// <summary>
        /// 提示信息
        /// </summary>
        public string HideMessage { get; set; }
        public string AlarmText { get; set; } = "警告";
        public Visibility CancerVisibility { get; set; } = Visibility.Hidden;
        #endregion
        #region 实时显示
        public double UVolateUi { get; set; }
        public double VolateUi { get; set; }
        public double FreUi { get; set; }
        public double DataPower { get; set; }
        public double CurrentUi { get; set; }
        public double TimeUi { get; set; }
        public double Jf { get; set; } = 0.00;

        public string TimeMul { get; set; } = "--";

        #endregion
        #region bindinds
        public Translator TestPra;

        public double Fre { get; set; }
        public string TestId { get; set; }
        public string RatedVoltage { get; set; }
        public string WindingGroup { get; set; }
        public double Temperature { get; set; }
        public string RatedCapacity { get; set; }
        public double Humidity { get; set; }
        public BindableCollection<TransformerDataStep> DatagridData;
        public BindableCollection<TransformerTestData> DatagridTestData { get; set; } = new BindableCollection<TransformerTestData>();
        #endregion
    }

    public partial class TransformerViewModel
    {
        static CancellationTokenSource tokenSource = new CancellationTokenSource();
        CancellationToken token = tokenSource.Token;
        ManualResetEvent resetEvent = new ManualResetEvent(true);
        private Task task;
        public async Task StartAuto()
        {
            if (tokenSource != null && task != null)
            {
                tokenSource.Cancel();
                await Task.Delay(100);
                await Task.Run(() => { task.Wait(); });
                IsRunning = false;
            }
            await Task.Delay(1000);
            tokenSource = new CancellationTokenSource();
            token = tokenSource.Token;
            resetEvent = new ManualResetEvent(true);
            task = new Task(StartTest, token, TaskCreationOptions.LongRunning);//, TaskCreationOptions.LongRunning
            task.Start();
        }
        private async void StartTest()
        {
            try
            {
                if (!IsRunning && await _SelfCheck.SeleCheck(token))
                {
                    IsRunning = true;
                    hidetest = "正在测量中...";
                    var data = TestPra.DatagridData.ToArray();
                    InitDataGrid(data);
                    if (await _communicationProtocol.TestKindVolateOrHgq(true))
                    {
                        IsRunning = false;
                        return;
                    }
                    #region 调频
                    for (int TestPosition = 1; TestPosition < 4; TestPosition++)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                       {
                           IsokOrCan = _windowManager.ShowMessageBox("开始" + Models.StaticClass.GetPhame(TestPosition) + "测量？\t\n请确保接线正确",
                               "提示", MessageBoxButton.YesNo) == MessageBoxResult.Yes ? true : false;
                       });
                        if (IsokOrCan)
                        {
                            await _setVolate.ControlsPowerStata(true, _communicationProtocol, token);
                            if (await _setVolate.SettingFre(Fre, _communicationProtocol, token))
                            {
                                for (int i = 0; i < data.Length; i++)
                                {

                                    if (await _setVolate.SettindHighVolate(data[i].TestVolate, _communicationProtocol, _xmlconfig, token))
                                    {
                                        if (UVolateUi > TestPra.Volate)
                                        {
                                            Application.Current.Dispatcher.Invoke(() =>
                                            {
                                                _windowManager.ShowMessageBox("过压\t\n已经自动退出 试验", "提示", MessageBoxButton.OK);
                                            });
                                            hidetest = "空闲中";
                                            IsRunning = false;
                                            return;
                                        }
                                        for (int j = 0; j < (int)(data[i].TestTime / 5); j++)
                                        {
                                            for (int pi = 0; pi < data[i].TestTime * 60; pi++)//
                                            {
                                                if (token.IsCancellationRequested)
                                                {
                                                    IsRunning = false;
                                                    throw new OperationCanceledException();
                                                }
                                                await Cancer(token);
                                                await Task.Delay(1000);
                                                TimeMul = (data[i].TestTime * 60 - pi - 1).ToString();
                                            }
                                            ShowOrHide = true;
                                            while (true)
                                            {
                                                await Cancer(token);
                                                if (SaveFlag)
                                                {
                                                    AddTestData(TestPra, i, (j + 1) * 5, TestPosition, JfData.ToString());
                                                    ShowOrHide = false;
                                                    SaveFlag = false;
                                                    break;
                                                }
                                            }
                                        }
                                        if (data[i].TestTime % 5 != 0)
                                        {
                                            for (int pi = 0; pi < (data[i].TestTime % 5) * 60; pi++)
                                            {
                                                if (token.IsCancellationRequested)
                                                {
                                                    IsRunning = false;
                                                    throw new OperationCanceledException();

                                                }
                                                await Cancer(token);
                                                await Task.Delay(1000);
                                                TimeMul = ((data[i].TestTime % 5) * 60 - pi - 1).ToString();

                                            }
                                            ShowOrHide = true;
                                            while (true)
                                            {
                                                await Cancer(token);
                                                if (SaveFlag)
                                                {
                                                    AddTestData(TestPra, i, data[i].TestTime, TestPosition, JfData.ToString() + "pC，" + UVolateUi.ToString() + "kV，" + VolateUi.ToString() + "V");
                                                    SaveFlag = false;
                                                    ShowOrHide = false;
                                                    break;
                                                }

                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {

                                _logger.Writer("设置测量频率失败，结束试验");
                            }
                            IsokOrCan = false;
                        }
                        await _setVolate.DownAndClosePower(_communicationProtocol, _xmlconfig, token);
                    }

                    #endregion

                    IsRunning = false;
                }
                else
                {
                    IsRunning = false;
                    Application.Current.Dispatcher.Invoke(() =>
                   {
                       _windowManager.ShowMessageBox("启动测量失败\t\n请检查接线\t\n请手动结束为结束的测量",
                          "提示", MessageBoxButton.OK);
                   });
                }
                hidetest = "空闲中";
            }
            catch
            {
                IsRunning = false;
                WrongThing();

            }
            finally
            {
                IsRunning = false;
                StartOpacity = 1;

            }
        }

        private async Task Cancer(CancellationToken token)
        {
            if (VolateUi < 1)
            {
                await Task.Delay(1300, token);
                if (VolateUi < 1)
                {
                    IsRunning = false;
                    WrongThing();
                    token.ThrowIfCancellationRequested();
                }
            }
        }
        private void WrongThing()
        {
            var ret = _windowManager.ShowMessageBox("试验外部中断\t\n确认：关闭仪器\t\n取消:继续其他试验\t\n如果仪器出现警报声请点击确认", "警告", System.Windows.MessageBoxButton.YesNo);
            if (ret == System.Windows.MessageBoxResult.Yes)
            {
                int time = 0;    //单位为：秒
                Process.Start("c:/windows/system32/shutdown.exe", "-s -t " + time);
            }

        }


        private async Task JfControls(Translator TestPra, int currentvolateIndex, double currenttesttime, int MarkPhama, string Jf)
        {
            #region 局放试验控制
            JfStataControl = false;
            await Application.Current.Dispatcher.BeginInvoke(new Action(() =>
             {
                 if (!WindowsStataIsOpen)
                 {
                     _windowManager.ShowWindow(_JfViewModel);
                     WindowsStataIsOpen = true;
                 }
                 _eventAggregator.Publish(WindowState.Normal);
             }));
            while (!JfStataControl)
            {
                await Task.Delay(10);
            }
            AddTestData(TestPra, currentvolateIndex, currenttesttime, MarkPhama, JfData.ToString());
            #endregion
        }
        public void Canclick()
        {
            if (AlarmText == "测量提示")
            {
                IsokOrCan = false;
            }
            IsClicked = true;

        }
        public void Okclick()
        {
            if (AlarmText == "测量提示")
            {
                IsokOrCan = true;
            }
            IsClicked = true;
        }
        public void PauceTest()
        {
            resetEvent.Reset();
        }
        public void ContinueTest()
        {
            resetEvent.Set();
        }
        public async void CancelTest()
        {
            if (IsRunning)
            {
                tokenSource.Cancel();
                await _setVolate.DownAndClosePower(_communicationProtocol, _xmlconfig, token);
                TimeMul = "--";

            }
            //{
            //    _windowManager.ShowMessageBox("未开始试验",
            //                "提示", MessageBoxButton.OK);
            //}

            //  await SetDiaSata(true, "试验已经结束", System.Windows.Visibility.Hidden, alarmText: "警告");
        }
        private async Task SetDiaSata(bool openorclose, string hideMessage, Visibility cancerVisibility,
            string alarmText = "警告")
        {
            OpenOrclose = openorclose;
            CancerVisibility = cancerVisibility;
            HideMessage = hideMessage;
            AlarmText = alarmText;
            while (!IsClicked)
            {
                await Task.Delay(100);
            }
        }
        private bool IsClicked = false;

    }


    public class TransformerTestData
    {
        public string Stepname { get; set; }
        public string TestVoltage { get; set; }
        public string TestTime { get; set; }
        //U局放
        public string UVoltage { get; set; }
        //V局放

        public string VVoltage { get; set; }
        public string WVoltage { get; set; }
    }


}
