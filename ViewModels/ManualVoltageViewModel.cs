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

namespace PortableEquipment.ViewModels
{
    public partial class ManualVoltageViewModel : Screen, IHandle<TestByHand>, IHandle<OutTestResult>, IHandle<string>
    {
        public IEventAggregator _eventAggregator;

        [Inject]
        public ICommunicationProtocol _communicationProtocol;
        [Inject]
        public Servers.CHangeVolate.ISetVolate _setVolate;
        [Inject]
        public Servers.Xmldata.IXmlconfig _xmlconfig;
        [Inject]
        public ICommunicationProtocol _CommunicationProtocol;

        [Inject]
        public IWindowManager _windowManager;

        [Inject]
        public StyletLogger.ILogger _logger;
        public ManualVoltageViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);
        }
        public void Handle(TestByHand message)
        {
            if (message.Current == CurrentKind.Big)
            {
                OverVolate = "190V";
                OverCurrent = "30A";
            }
            else
            {
                OverVolate = "380V";
                OverCurrent = "15A";
            }
        }


    }

    public partial class ManualVoltageViewModel
    {
        #region Command
        public async Task ChnageVolate(double volate)
        {
            if (!TestStata)
            {
                TestStata = true;
                AddHideList("正在开始调压中...");
                if (await _setVolate.SettindVolate(volate, _communicationProtocol, _xmlconfig, new CancellationTokenSource().Token))
                {
                    AddHideList("调压至 " + volate + "成功");
                }
                else
                {
                    AddHideList("调压至 " + volate + "失败");
                }
            }
            else
            {
                AddHideList("处于调压模式中，无法开始调压");
            }
            AddHideList("调压至 " + volate + "已结束");
            TestStata = false;

        }

        public async Task ChnageHighVolate(double volate)
        {
            if (!TestStata)
            {
                TestStata = true;
                AddHideList("正在开始调压中...");
                if (await _setVolate.SettindHighVolate(VolateNeed, _communicationProtocol, _xmlconfig, new CancellationTokenSource().Token))
                {
                    AddHideList("调压至 " + volate + "kV成功");
                }
                else
                {
                    AddHideList("调压至 " + volate + "kV失败");
                }
            }
            else
            {
                AddHideList("处于调压模式中，无法开始调压");
            }
            AddHideList("调压至 " + volate + "kV已结束");
            TestStata = false;

        }
        public async Task ChnageFre(double fre)
        {
            if (!TestStata)
            {
                TestStata = true;
                while (VolateUi < 10)
                {
                    if (!await _communicationProtocol.GetPowerStata())
                        await _setVolate.ControlsPowerStata(true, _communicationProtocol, new CancellationTokenSource().Token);
                    await _communicationProtocol.SetTestPra(TestKind.ControlsVolateUP, 2, new CancellationTokenSource().Token);
                    await Task.Delay(20);
                }
                AddHideList("正在开始调频率中...");
                if (await _setVolate.SettingFre(fre, _communicationProtocol, new CancellationTokenSource().Token))
                {
                    AddHideList("调频率至 " + fre + "成功");
                }
                else
                {
                    AddHideList("调频率至 " + fre + "失败");
                }
            }
            else
            {
                AddHideList("处于调频率模式中，无法开始调频率");
            }
            TestStata = false;
        }

        public async void ConfireOutVolate()
        {
            if (!TestStata)
            {
                OpenOrclose = true;
                AddHideList("开始试验...");
                await ChnageFre(Fre);
                await ChnageVolate(VolateNeed);
                AddHideList("试验已结束...");
                OpenOrclose = false;
            }
            else
            {
                _windowManager.ShowMessageBox("正在试验中，请等待结结束", "提示", System.Windows.MessageBoxButton.OK);
            }

        }
        public void Handle(OutTestResult message)
        {
            if (message.stataThree.Checked)
            {
                VolateUi = message.stataThree.AVolate;
                CurrentUi = message.stataThree.ACurrent;
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
        public async Task AddFre(string addfrestring)
        {
            TestKind ts = addfrestring.Contains("+") ? TestKind.ControlsFreUp : TestKind.ControlsFreDown;
            double addfre = Convert.ToDouble(addfrestring.Replace("+", "").Replace("-", ""));
            int LargeChange = (int)addfre;
            int SmallChange = (int)((addfre - (int)addfre) * 10); ;
            if (!TestStata)
            {
                TestStata = true;
                AddHideList("开始调整频率...");
                if (addfre >= 1)
                {
                    if (await _communicationProtocol.ThicknessAdjustable(true))
                    {
                        await _communicationProtocol.SetTestPra(ts, (byte)LargeChange, new CancellationTokenSource().Token);
                    }
                }
                else
                {
                    if (await _communicationProtocol.ThicknessAdjustable(false))
                    {
                        await _communicationProtocol.SetTestPra(ts, (byte)SmallChange, new CancellationTokenSource().Token);
                    }
                }
                AddHideList("调整频率完成");

            }
            else
            {
                AddHideList("正在试验中，无法操作");
            }
            TestStata = false;
        }


        public async Task AddVolatage(string addvolatestring)
        {
            double changevolate = Convert.ToDouble(addvolatestring);
            double needvolate = changevolate + VolateUi;
            if (!TestStata)
            {
                TestStata = true;
                AddHideList("开始调整电压...");
                await _setVolate.SettindVolate(needvolate, _communicationProtocol, _xmlconfig, new CancellationTokenSource().Token);
                AddHideList("调整电压完成");

            }
            else
            {
                AddHideList("正在试验中，无法操作");
            }
            TestStata = false;
        }
        private void AddHideList(string content)
        {
            if (HideList == null)
                HideList = new BindableCollection<string>();
            HideList.Insert(0, DateTime.Now.ToString() + " :" + content);
        }

        public async void ShowHeader(string par)
        {
            try
            {
                IsRunning = true;
                tokenSource = new CancellationTokenSource();
                token = tokenSource.Token;
                for (int i = 0; i < MulTime; i++)
                {
                    OverVolatetime = (MulTime - i - 1).ToString();
                    await Task.Delay(1000, token);
                }
                await _setVolate.DownAndClosePower(_communicationProtocol, _xmlconfig, token);
                OverVolatetime = "FINISH";
            }
            finally
            {
                IsRunning = false;
            }


        }
        public bool CanShowHeader
        {
            get
            {
                return MulTime != 0 && IsRunning;
            }
        }

        public async void TransformerClose()
        {
            if (IsRunning)
            {
                if (tokenSource != null)
                    tokenSource.Cancel();
                IsRunning = false;
            }
            if (await _communicationProtocol.GetPowerStata())
            {
                await _setVolate.DownVolateZero(_communicationProtocol, _xmlconfig, token);
                await _setVolate.ControlsPowerStata(false, _communicationProtocol, token);
            }
            this.RequestClose();
        }





        public bool IsRunning { get; set; } = false;
        #endregion
    }


    public partial class ManualVoltageViewModel
    {

        static CancellationTokenSource tokenSource = new CancellationTokenSource();
        CancellationToken token = tokenSource.Token;

        #region Bindings
        public string Voltage { get; set; } = "5.5kV";
        public double VolateNeed { get; set; } = 100;
        public double Fre { get; set; } = 50.0;
        public double DataPower { get; set; }

        public string OverVolate { get; set; }
        public string OverCurrent { get; set; }

        public int MulTime { get; set; } = 60;
        public string OverVolatetime { get; set; } = "--";

        #region 实时显示
        public double UVolateUi { get; set; }
        public double VolateUi { get; set; }
        public double FreUi { get; set; }
        public double CurrentUi { get; set; }
        public double TimeUi { get; set; }
        #endregion


        #endregion

        #region Ui状态控制
        public bool GridEnable { get; set; }
        public BindableCollection<string> HideList { get; set; }
        public bool PowerStata { get; set; }

        public bool TestStata { get; set; }
        public bool OpenOrclose { get; set; }
        #endregion
    }
}
