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
    public partial class VoltageTestViewModel : Screen, IHandle<MutualTranslator>,IHandle<OutTestResult>,IHandle<string>
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
        public VoltageTestViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);

#if DEBUG
            InitCreateChart();
            AddNodePoint(new Random().NextDouble(), 2);
            AddNodePoint(2, 3);
            AddNodePoint(4, 5);
            AddNodePoint(2, 2);
#endif
        }
        public void Handle(MutualTranslator message)
        {
            _mutualTranslator = message;
            LcGetTestVolateRange(_mutualTranslator.ExcitationCharacteristicR);
            GetBasePra(message);
        }
        #endregion

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
            TestLevel = mutualTranslator.TestLevel;
            Humidity = mutualTranslator.Humidity;
            Temperature = mutualTranslator.Temperature;

            KzTestVolate = mutualTranslator.NoLoadCurrentR.TestVolate;
            KzOverCurrent = mutualTranslator.NoLoadCurrentR.OverCurrent;

            KeepTestVoltage = mutualTranslator.InducedOvervoltageR.TestVoltage;
            KeepTestFre = mutualTranslator.InducedOvervoltageR.TestFre;
            KeepTestTime = mutualTranslator.InducedOvervoltageR.TestTime;
        }

        public void SaveTestResult()
        {
            _sqlHelp.SaveMutualTranslatorTransformerDataBase(_mutualTranslator);
        }
        #endregion


        #region TestCommand

        static CancellationTokenSource tokenSource = new CancellationTokenSource();
        CancellationToken token = tokenSource.Token;
        ManualResetEvent resetEvent = new ManualResetEvent(true);
        public async Task TestPressureVolate(MutualTranslator mutualTranslator)
        {
            Flag = Flag.RunningPressure;
            PresurreTime = 0;
            await _setVolate.SettindVolate(mutualTranslator.InducedOvervoltageR.OutgoingTestVoltage, _communicationProtocol, _xmlconfig);
            for (int i = 0; i < (int)mutualTranslator.InducedOvervoltageR.TestTime; i++)
            {
                if (token.IsCancellationRequested)
                {
                    return;
                }
                resetEvent.WaitOne();
                PresurreTime = (int)mutualTranslator.InducedOvervoltageR.TestTime - i;
                await Task.Delay(1000);
            }
            var ret = await _communicationProtocol.ReadStataThree();
            Flag = Flag.FinishPressure;
        }
        public async Task TestLc(MutualTranslator mutualTranslator)
        {
            List<StataThree> Lcret = new List<StataThree>();
            LcDatagrid = new BindableCollection<LcdatagridColletion>();
            Flag = Flag.RunningLc;
            var volatelist = mutualTranslator.ExcitationCharacteristicR.VolateRange;
            for (int i = 0; i < volatelist.Length; i++)
            {
                if (token.IsCancellationRequested)
                {
                    return;
                }
                resetEvent.WaitOne();
                await _setVolate.SettindVolate(volatelist[i], _communicationProtocol, _xmlconfig);
                var p = await _communicationProtocol.ReadStataThree();
                Lcret.Add(p);
                LcDatagrid.Add(new LcdatagridColletion { TestNum = (i + 1).ToString(), TestCurrent = p.ACurrent.ToString(), TestVoltage = p.AVolate.ToString() });
                AddNodePoint(p.ACurrent, p.AVolate);
            }
            Flag = Flag.FinishLc;
        }
        public async Task TestKz(MutualTranslator mutualTranslator)
        {
            Flag = Flag.RunningKz;
            double kzvolate = mutualTranslator.NoLoadCurrentR.OverCurrent;
            await _setVolate.SettindVolate(kzvolate, _communicationProtocol, _xmlconfig);
            var ret = _communicationProtocol.ReadStataThree();
            Flag = Flag.FinishKz;
        }
        public async Task StartTest()
        {
            tokenSource = new CancellationTokenSource();
            token = tokenSource.Token;
            resetEvent = new ManualResetEvent(true);
            if (_mutualTranslator.InducedOvervoltageR.Enable)
                await TestPressureVolate(_mutualTranslator);
            if (_mutualTranslator.ExcitationCharacteristicR.Enable)
                await TestLc(_mutualTranslator);
            if (_mutualTranslator.NoLoadCurrentR.Enable)
                await TestKz(_mutualTranslator);
        }
        /// <summary>
        /// 中断测量过程
        /// </summary>
        public async void FinishTest()
        {
            tokenSource.Cancel();
            await StartTest();
            Flag = Flag.Free;
        }
        #endregion
    }
    public partial class VoltageTestViewModel
    {
        #region 实时ui
        public double UVolateUi { get; set; }
        public double VolateUi { get; set; }
        public double FreUi { get; set; }
        public double CurrentUi { get; set; }
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
        public int PresurreTime { get; set; }
        public bool StartTestVisibility { get; set; } = true;

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
                LineSmoothness = 1,//0为折现样式
                PointGeometrySize = 8,
                PointForeground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(34, 46, 49)),
                Values = TanEleVolatevalue
            };
            LcCurrentVolate = new SeriesCollection { };
            LcCurrentVolate.Add(t1);
            LcCurrentVolate[0].Values.Add(new ObservablePoint(0, 0));

            XFormatter = val => (val).ToString("N3") + "A";
            YFormatter = val => (val).ToString("N3") + " V";
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


        public double KeepTestVoltage { get; set; }
        public double KeepTestFre { get; set; }
        public double KeepTestTime { get; set; }

        public bool IsTopDrawerOpen { get; set; } = false;

        #endregion
    }
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
}
