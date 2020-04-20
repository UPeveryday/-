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
    public partial class ParameterSettingViewModel : Screen
    {
        #region 打开子窗体,提供方法
        private IWindowManager _windowManger;
        [Inject]
        public VoltageTestViewModel _VoltageTestViewModel;
        public IEventAggregator _eventAggregator;
        [Inject]
        public StyletLogger.ILogger _logger;
        [Inject]
        public Servers.Xmldata.IXmlconfig _xmlconfig;
        [Inject]
        public Servers.Json.IJsondeel _jsondeel;
        [Inject]
        public Servers.IEntityServer entityServer;
        public ParameterSettingViewModel(IWindowManager windowManager, IEventAggregator eventAggregator)
        {
            _windowManger = windowManager;
            _eventAggregator = eventAggregator;
        }

        #endregion
    }

    public partial class ParameterSettingViewModel
    {
        #region command
        /// <summary>
        /// 添加单独的测试等级 35%
        /// </summary>
        public void Dialogstata()
        {
            double[] pdata = GetBackGroupdata();
            if (pdata != null)
            {
                List<double> tpd = new List<double>();
                tpd.AddRange(pdata);
                if (tpd.Any(p => p == Conetnt))
                {
                    _logger.Writer("已经存在此电压点");
                }
                else
                {
                    tpd.Add(Conetnt);
                    double[] ret = tpd.ToArray();
                    Array.Sort(ret);
                    GetDefaultGroupdata(ret);
                }

            }
        }
        public void Deletedata()
        {
            if (VolataGroup.Count > 0)
                VolataGroup.Remove(Selectdata);
        }
        private MutualTranslator getMutualTranslator()
        {
            return new MutualTranslator
            {
                TestId = TestId == null ? "--未指定--" : TestId,
                TestLevel = TestLevel == null ? "--未指定--" : TestLevel,
                Humidity = Humidity,
                Temperature = Temperature,
                TestLocation = TestLocation == null ? "--未指定--" : TestLocation,
                Tester = Tester == null ? "--未指定--" : Tester,
                DateTime = DateTime.Now,
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
                    OverCurrent = LcOverCurrent,
                    VolateRange = GetBackGroupdata()


                },
                NoLoadCurrentR = new NoLoadCurrent
                {
                    Enable = CurrentCheck,
                    TestVolate = KzTestVolate,
                    OverCurrent = KzOverCurrent
                }
            };
        }
        private void SendPra() => _eventAggregator.Publish(getMutualTranslator());
        public void ShowVoltageTestViewModels()
        {
            SendPra();
            _windowManger.ShowDialog(_VoltageTestViewModel);
        }

        /// <summary>
        /// 需要的百分比点
        /// </summary>
        /// <param name="pdatas"></param>
        private void GetDefaultGroupdata(double[] pdatas)
        {
            VolataGroup.Clear();
            for (int i = 0; i < pdatas.Length; i++)
            {
                VolataGroup.Add((i + 1) + "、" + GetSingalVolateGroup(pdatas[i]));
            }


        }
        /// <summary>
        /// 反向获取List中测量等级
        /// </summary>
        /// <returns></returns>
        private double[] GetBackGroupdata()
        {
            try
            {
                if (VolataGroup != null)
                {
                    List<double> pencentdata = new List<double>();
                    foreach (var item in VolataGroup)
                    {
                        pencentdata.Add(Convert.ToDouble(item.Split('、')[1].Split('%')[0].Replace("\t电压点在", "")));
                    }
                    double[] ret = pencentdata.ToArray();
                    Array.Sort(ret);
                    return ret;
                }
            }
            catch
            {
                _logger.Writer("解析GetBackGroupdata失败");
            }
            return null;


        }
        private string GetSingalVolateGroup(double Percent)
        {
            return "\t电压点在" + Percent + "% 处 :" + (int)_lctestvolate * Percent / 100 + "V";
        }
        /// <summary>
        /// 解析配置文件的电塔等级
        /// </summary>
        /// <returns></returns>
        private double[] GetXmlVoltageRange()
        {
            List<double> retlist = new List<double>();
            var c = _xmlconfig.GetAddNodeValue("VolatageRange");
            var ret = c.Split(',');
            try
            {
                foreach (var item in ret)
                {
                    retlist.Add(Convert.ToDouble(item));
                }
                double[] data = retlist.ToArray();
                Array.Sort(data);
                return data;
            }
            catch
            {
                _logger.Writer("解析配置文件 VolatageRange 错误，请检查配置文件");
            }
            return null;

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
        public double LiciOpacity { get; set; } = 1;

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
        #region 基本信息
        public string TestId { get; set; }
        public string TestLevel { get; set; }
        public double Humidity { get; set; }
        public double Temperature { get; set; }
        public string TestLocation { get; set; }
        public string Tester { get; set; }
        #endregion

        #region 感应耐压

        private int _ratadvolateselectindex;

        public int RatadVolataSelectIndex
        {
            get
            {
                if (_ratadvolateselectindex == 0)
                    Promotion = 0.03;
                if (_ratadvolateselectindex == 1)
                    Promotion = 0.04;
                if (_ratadvolateselectindex == 2)
                    Promotion = 0.05;
                if (_ratadvolateselectindex == 3)
                    Promotion = 0.08;
                return _ratadvolateselectindex;
            }
            set { _ratadvolateselectindex = value; }
        }

        private double _OutgoingTestVoltage;
        public double OutgoingTestVoltage
        {
            get { return _OutgoingTestVoltage; }
            set
            {
                _OutgoingTestVoltage = value;
                HighOverVolate = value;
                if (VariableThan != 0)
                {
                    if (HighOrLow == 0)
                        TestVoltage = Math.Round(_OutgoingTestVoltage / (VariableThan * (1 + Promotion)) * 0.8, 2);
                    else
                        TestVoltage = Math.Round(_OutgoingTestVoltage * 0.8,2);
                }
            }
        }
        private int _highorlow = 0;
        public int HighOrLow
        {
            get { return _highorlow; }
            set
            {
                _highorlow = value;
                if(_highorlow==0)
                {
                    if (VariableThan != 0)
                    {
                        TestVoltage = Math.Round(_OutgoingTestVoltage / (VariableThan * (1 + Promotion)) * 0.8, 2);
                    }
                }
                else
                {
                    if (VariableThan != 0)
                    {
                        TestVoltage = Math.Round(_OutgoingTestVoltage * 0.8,2);
                    }
                }
            }
        }

        public DetectionType detectionType
        {
            get
            {
                if (HighOrLow == 1) return DetectionType.HighpressureResistanceTest;
                else return DetectionType.LowessureResistanceTest;
            }
        }
        public double TestVoltage { get; set; }
        public double HighOverVolate { get; set; }//高压过压值
        public double TestFre { get; set; } = 50;
        public double TestTime { get; set; } = 60;

        private double _VariableThan = 1.0;
        public double VariableThan
        {
            get { return _VariableThan; }
            set
            {
                _VariableThan = value;
                if (_VariableThan != 0)
                {
                    if (HighOrLow == 0)
                        TestVoltage = Math.Round(_OutgoingTestVoltage / (_VariableThan * (1 + Promotion)) * 0.8, 2);
                    else
                        TestVoltage = Math.Round(_OutgoingTestVoltage * 0.8);
                }
            }
        }

        public double Promotion { get; set; }//荣升系数

        public System.Windows.Visibility OverVolateVisibility { get; set; } = System.Windows.Visibility.Visible;
        #endregion

        #region 励磁tex
        /// <summary>
        /// 获取电压等级并设定
        /// </summary>
        private double _lctestvolate;
        public double LcTestVolate
        {
            get { return _lctestvolate; }
            set
            {
                _lctestvolate = value;
                if (GetXmlVoltageRange() != null)
                    GetDefaultGroupdata(GetXmlVoltageRange());
            }
        }

        public double LcOverCurrent { get; set; }

        public System.Windows.Visibility VolataGroups { get; set; } = System.Windows.Visibility.Collapsed;

        public BindableCollection<string> VolataGroup { get; set; } = new BindableCollection<string>();
        private bool _volateRange;
        public bool VolateRange
        {
            get { return _volateRange; }
            set
            {
                _volateRange = value;
                if (_volateRange)
                {
                    VolataGroups = System.Windows.Visibility.Visible;
                    OverVolateVisibility = System.Windows.Visibility.Collapsed;
                }
                else
                {
                    VolataGroups = System.Windows.Visibility.Collapsed;
                    OverVolateVisibility = System.Windows.Visibility.Visible;
                }
            }
        }
        public double Conetnt { get; set; }
        public string Selectdata { get; set; }
        #endregion

        #region 空载
        public double KzTestVolate { get; set; }
        public double KzOverCurrent { get; set; }
        #endregion
    }
}
