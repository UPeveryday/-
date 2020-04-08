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
using System.Windows.Media;

namespace PortableEquipment.ViewModels
{
    public partial class TransformerViewModel : Screen, IHandle<Translator>,IHandle<StataThree>
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
        public TimeViewModel _TimeViewModel;

        [Inject]
        public IWindowManager _windowManager;
        public IEventAggregator _eventAggregator;
        public TransformerViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);
        }

        //Ui更新
        public void Handle(StataThree message)
        {
            if (message.Checked)
            {
                CurrentUi = message.ACurrent;
                VolateUi = message.AVolate;
                FreUi = message.Fre;
            }
        }
        #endregion
    }

    public partial class TransformerViewModel
    {
        public void Handle(Translator message)
        {
            TestPra = message;
            TestId = message.TestId;
            RatedVoltage = message.RatedVoltage;
            RatedCapacity = message.RatedCapacity;
            WindingGroup = message.WindingGroup;
            Temperature = message.Temperature;
            Humidity = message.Humidity;
            DatagridData = message.DatagridData;
            InitDataGrid(message.DatagridData.ToArray());
        }
        public void SaveManagMent()
        {
            if (DatagridTestData != null)
                TestPra.TestResultData = DatagridTestData;
            if (_sqlHelp.SaveTransformerDataBase(TestPra))
            {
                OpenOrclose = true;
                CancerVisibility = Visibility.Hidden;
                HideMessage = "保存成功";
            }
            else
            {
                OpenOrclose = true;
                CancerVisibility = Visibility.Hidden;
                HideMessage = "保存失败，请重试";
            }
        }
        private void InitDataGrid(TransformerDataStep[] data)
        {
            DatagridTestData.Clear();
            for (int i = 0; i < data.Length; i++)
            {
                for (int j = 0; j < data[i].TestTime / 5; j++)
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
                for (int j = 0; j < transformerDataStep[i].TestTime / 5; j++)
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
        private void AddTestData(Translator TestPra, int currentvolateIndex, double currenttesttime, int MarkPhama)
        {
            int p = 0;
            foreach (var item in DatagridTestData)
            {
                if (p == GetIndexFromDataGrid(TestPra.DatagridData.ToArray(), currentvolateIndex, currenttesttime) - 1)
                {
                    if (MarkPhama == 1)
                        item.UVoltage = UVolateUi.ToString();
                    if (MarkPhama == 2)
                        item.VVoltage = UVolateUi.ToString();
                    if (MarkPhama == 3)
                        item.WVoltage = UVolateUi.ToString();
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

    }
    public partial class TransformerViewModel
    {
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
                    statacolor = Brushes.LightGreen;
                else
                    statacolor = Brushes.Red;
            }
        }
        public bool OpenOrclose { get; set; }
        /// <summary>
        /// 提示信息
        /// </summary>
        public string HideMessage { get; set; }
        public string AlarmText { get; set; } = "警告";

        public System.Windows.Visibility CancerVisibility { get; set; } = System.Windows.Visibility.Hidden;

        public Brush statacolor { get; set; } = Brushes.Red;
        #endregion
        #region 实时显示
        public double UVolateUi { get; set; }
        public double VolateUi { get; set; }
        public double FreUi { get; set; }
        public double CurrentUi { get; set; }
        public double TimeUi { get; set; }
        #endregion
        #region bindinds
        public Translator TestPra;
        public string TestId { get; set; }
        public double RatedVoltage { get; set; }
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
        public async void StartAuto()
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
            if (!IsRunning)
            {
                IsRunning = true;
                var data = TestPra.DatagridData.ToArray();
                InitDataGrid(data);
                for (int TestPosition = 1; TestPosition < 4; TestPosition++)
                {
                    IsClicked = false;
                    await SetDiaSata(true, "开始" + Models.StaticClass.GetPhame(TestPosition) + "测量？\t\n请确保接线正确",
                        System.Windows.Visibility.Visible, alarmText: "测量提示");
                    if (IsokOrCan)
                    {
                        for (int i = 0; i < data.Length; i++)
                        {
                            //  await _setVolate.SettindVolate(data[i].TestVolate, _communicationProtocol, _xmlconfig);
                            for (int j = 0; j < data[i].TestTime / 5; j++)
                            {
                                for (int pi = 0; pi < 5; pi++)
                                {
                                    if (token.IsCancellationRequested)
                                    {
                                        IsRunning = false;
                                        return;
                                    }
                                    resetEvent.WaitOne();
                                    await Task.Delay(1000);
                                }
                                AddTestData(TestPra, i, (j + 1) * 5, TestPosition);
                            }
                            if (data[i].TestTime % 5 != 0)
                            {
                                for (int pi = 0; pi < 5/*(data[i].TestTime % 5) * 60*/; pi++)
                                {
                                    if (token.IsCancellationRequested)
                                    {
                                        IsRunning = false;
                                        return;
                                    }
                                    resetEvent.WaitOne();
                                    await Task.Delay(1000);
                                }
                                AddTestData(TestPra, i, data[i].TestTime, TestPosition);
                            }
                        }
                    }
                }
                IsRunning = false;
            }
            else
            {
                await SetDiaSata(true, "启动测量失败\t\n可能未结束测量\t\n请手动结束为结束的测量", System.Windows.Visibility.Hidden, alarmText: "警告");
            }
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
        public void CancelTest()
        {
            tokenSource.Cancel();
            SetDiaSata(true, "试验已经结束", System.Windows.Visibility.Hidden, alarmText: "警告");
        }
        private async Task SetDiaSata(bool openorclose, string hideMessage, System.Windows.Visibility cancerVisibility,
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
        public string UVoltage { get; set; }
        public string VVoltage { get; set; }
        public string WVoltage { get; set; }
    }


}
