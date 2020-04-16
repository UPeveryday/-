﻿using PortableEquipment.Servers.CommunicationProtocol;
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
        public StyletLogger.ILogger _logger;
        public ManualVoltageViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);
        }
        public void Handle(TestByHand message)
        {
            //throw new NotImplementedException();
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
                ConfirePowerStata();
                if (await _setVolate.SettindVolate(volate, _communicationProtocol, _xmlconfig))
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
        public async Task ChnageFre(double fre)
        {
            if (!TestStata)
            {
                TestStata = true;
                AddHideList("正在开始调频率中...");
                await ConfirePowerStata();
                if (await _setVolate.SettingFre(fre, _communicationProtocol))
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
            await ChnageVolate(VolateNeed);
            await ChnageFre(Fre);

            //var p =await _communicationProtocol.ThicknessAdjustable(true);
            //int a = 0;

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
        public async Task AddFre(string addfrestring)
        {
            TestKind ts = addfrestring.Contains("+") ? TestKind.ControlsFreUp : TestKind.ControlsFreDown;
            double addfre = Convert.ToDouble(addfrestring.Replace("+", "").Replace("-", ""));
            int LargeChange = (int)addfre;
            int SmallChange = (int)((addfre - (int)addfre) * 10); ;
            if (!TestStata)
            {
                TestStata = true;
                AddHideList("开始增加频率...");
                if (addfre >= 1)
                {
                    if (await _communicationProtocol.ThicknessAdjustable(true))
                    {
                        await _communicationProtocol.SetTestPra(ts, (byte)LargeChange);
                    }
                }
                else
                {
                    if (await _communicationProtocol.ThicknessAdjustable(false))
                    {
                        await _communicationProtocol.SetTestPra(ts, (byte)SmallChange);
                    }
                }
                AddHideList("增加频率完成");

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
        private async Task ConfirePowerStata()
        {
            AddHideList("正在确认电源状态...");
            if (!PowerStata)
            {
                PowerStata = await _communicationProtocol.SetTestPra(TestKind.Start, 2);
                await Task.Delay(6500);
                PowerStata = true;
            }
            AddHideList("电源状态确认完成");
        }
        #endregion
    }


    public partial class ManualVoltageViewModel
    {
        #region Bindings
        public string Voltage { get; set; } = "5.5kV";
        public double VolateNeed { get; set; } = 100;
        public double Fre { get; set; } = 50.0;
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
        #endregion
    }
}
