using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortableEquipment.ViewModels
{
    public class ParameterSettingViewModel : Screen
    {
        #region 打开子窗体,提供方法
        private IWindowManager _windowManger;
        private VoltageTestViewModel _VoltageTestViewModel;
        public ParameterSettingViewModel(IWindowManager windowManager, VoltageTestViewModel voltageTestViewModel)
        {
            _windowManger = windowManager;
            _VoltageTestViewModel = voltageTestViewModel;
        }
        public void ShowVoltageTestViewModel() => _windowManger.ShowDialog(_VoltageTestViewModel);
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
}
