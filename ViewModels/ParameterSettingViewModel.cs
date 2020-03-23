using PortableEquipment.TestParameters;
using Stylet;
using StyletIoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortableEquipment.ViewModels
{
    public partial class ParameterSettingViewModel : Screen, IHandle<MutualTranslator>
    {
        #region 打开子窗体,提供方法
        private IWindowManager _windowManger;
        private VoltageTestViewModel _VoltageTestViewModel;
        public IEventAggregator _eventAggregator;
        public ParameterSettingViewModel(IWindowManager windowManager, VoltageTestViewModel voltageTestViewModel, IEventAggregator eventAggregator)
        {
            _windowManger = windowManager;
            _VoltageTestViewModel = voltageTestViewModel;
            _eventAggregator = eventAggregator;
            //_eventAggregator.Subscribe(this);
        }
        public void ShowVoltageTestViewModel()
        {
            StartTest()
;            _windowManger.ShowDialog(_VoltageTestViewModel);
        }
        #endregion
        #region 页面使能
        public bool KeepVolateEnable
        {
            get
            {
                if (KeepVolateCheck)
                    KeepVoltageOpacity = 1;
                else
                    KeepVoltageOpacity = 0.5;
                return KeepVolateCheck;
            }
            set { }
        } //感应耐压参数使能
        public bool KeepVolateCheck { get; set; } = false;
        public double KeepVoltageOpacity { get; set; } = 0.5;

        public bool LiciEnable
        {
            get
            {
                if (LiciCheck)
                    LiciOpacity = 1;
                else
                    LiciOpacity = 0.5;
                return LiciCheck;
            }
            set { }
        } //感应耐压参数使能
        public bool LiciCheck { get; set; } = false;
        public double LiciOpacity { get; set; } = 0.5;

        public bool CurrentEnable
        {
            get
            {
                if (CurrentCheck)
                    CurrentOpacity = 1;
                else
                    CurrentOpacity = 0.5;
                return CurrentCheck;
            }
            set { }
        } //感应耐压参数使能
        public bool CurrentCheck { get; set; } = false;
        public double CurrentOpacity { get; set; } = 0.5;

        #endregion
    }

    public partial class ParameterSettingViewModel
    {
        #region command
        private MutualTranslator getMutualTranslator()
        {
            return new MutualTranslator
            {
                TestId = TestId == null ? "--未指定--" : TestId,
                TestLevel = TestLevel == null ? "--未指定--" : TestLevel,
                Humidity = Humidity == null ? "--未指定--" : Humidity,
                Temperature = Temperature == null ? "--未指定--" : Temperature,
                TestLocation = TestLocation == null ? "--未指定--" : TestLocation,
                Tester = Tester == null ? "--未指定--" : Tester,
                InducedOvervoltageR = new InducedOvervoltage
                {
                    Enable = KeepVolateCheck,
                    OutgoingTestVoltage = OutgoingTestVoltage,
                    detectionType = detectionType,
                    TestVoltage = TestVoltage,
                    HighOverVolate = HighOverVolate,
                    TestFre = TestFre,
                    TestTime = TestTime,
                    VariableThan = VariableThan,
                    Promotion = Promotion
                },
                ExcitationCharacteristicR = new ExcitationCharacteristic
                {
                    Enable = LiciCheck,
                    TestVolate = LcTestVolate,
                    OverCurrent = LcOverCurrent
                },
                NoLoadCurrentR = new NoLoadCurrent
                {
                    Enable = CurrentCheck,
                    TestVolate = KzTestVolate,
                    OverCurrent = KzOverCurrent
                }
            };
        }

        public void Handle(MutualTranslator message)
        {
        }

        private void StartTest() => _eventAggregator.Publish(getMutualTranslator());

        #endregion
    }
    public partial class ParameterSettingViewModel
    {
        #region 基本信息
        public string TestId { get; set; }
        public string TestLevel { get; set; }
        public string Humidity { get; set; }
        public string Temperature { get; set; }
        public string TestLocation { get; set; }
        public string Tester { get; set; }
        #endregion

        #region 感应耐压
        public bool HighOverVolateCheck { get; set; } = true;
        public bool LowOverVolateCheck { get; set; } = false;

        public double OutgoingTestVoltage { get; set; }
        public DetectionType detectionType
        {
            get
            {
                if (HighOverVolateCheck) return DetectionType.HighpressureResistanceTest;
                else return DetectionType.LowessureResistanceTest;
            }
        }
        public double TestVoltage { get; set; }
        public double HighOverVolate { get; set; }//高压过压值
        public double TestFre { get; set; }
        public double TestTime { get; set; }
        public double VariableThan { get; set; }//变比
        public double Promotion { get; set; }//荣升系数
        #endregion

        #region 励磁tex
        public double LcTestVolate { get; set; }
        public double LcOverCurrent { get; set; }
        #endregion

        #region 空载
        public double KzTestVolate { get; set; }
        public double KzOverCurrent { get; set; }
        #endregion
    }
}
