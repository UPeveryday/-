using PortableEquipment.Servers.CommunicationProtocol;
using Stylet;
using Stylet.Logging;
using StyletIoC;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PortableEquipment.ViewModels
{
    public class MainViewModel : Screen, IHandle<string>, IHandle<Stata>
    {
        private IEventAggregator _eventAggregator;
        private IWindowManager _windowManger;
        private DataManagementViewModel _ChildDialog;
        private ManuallySetParametersViewModel _ManuallySetParametersViewModel;
        private ManualVoltageViewModel _ManualVoltageViewModel;
        private ParameterSettingViewModel _ParameterSettingViewModel;
        private TransformerViewModel _TransformerViewModel;
        private VoltageTestViewModel _VoltageTestViewModel;
        private WithstandVoltageViewModel _WithstandVoltageViewModel;
        private StyletLogger.ILogger _logger;
        private TimeViewModel _timeViewModel;
        [Inject]
        public ICommunicationProtocol _communicationProtocol;
        [Inject]
        public Servers.CHangeVolate.ISetVolate _setVolate;
        [Inject]
        public Servers.Xmldata.IXmlconfig _xmlconfig;
        [Inject]
        public ICommunicationProtocol _CommunicationProtocol;

        public MainViewModel(IWindowManager windowManager, DataManagementViewModel ChildDialog,
            ManuallySetParametersViewModel manuallySetParametersViewModel, ManualVoltageViewModel manualVoltageViewModel,
            ParameterSettingViewModel parameterSettingViewModel,
           WithstandVoltageViewModel withstandVoltageViewModel, IEventAggregator eventAggregator, StyletLogger.ILogger logger
            , TimeViewModel timeViewModel)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);

            _windowManger = windowManager;
            _ChildDialog = ChildDialog;
            _ManuallySetParametersViewModel = manuallySetParametersViewModel;
            _ManualVoltageViewModel = manualVoltageViewModel;
            _ParameterSettingViewModel = parameterSettingViewModel;
            _WithstandVoltageViewModel = withstandVoltageViewModel;
            _logger = logger;
            _timeViewModel = timeViewModel;
        }
        public string FName { get; set; } = "ly";

        public void BtnCommand()
        {
            FName = DateTime.Now.ToString();
        }

        public bool CanBtnCommand
        {
            get
            {
                return !string.IsNullOrWhiteSpace(FName);
            }
        }
        public void ShowMessage() => _windowManger.ShowMessageBox(FName);
        public void ShowDataMannege() => _windowManger.ShowDialog(_ChildDialog);
        public void ShowManuallySet() => _windowManger.ShowDialog(_ManuallySetParametersViewModel);
        public void ShowManualVoltage() => _windowManger.ShowDialog(_ManualVoltageViewModel);
        public void ShowParameter() => _windowManger.ShowDialog(_ParameterSettingViewModel);
        public void ShowTransformer() => _windowManger.ShowDialog(_TransformerViewModel);
        public void ShowVoltageTest() => _windowManger.ShowDialog(_VoltageTestViewModel);
        public void ShowWithstand() => _windowManger.ShowDialog(_WithstandVoltageViewModel);
        public void Sendcomman() => _CommunicationProtocol.ReadStataThree();
        public void showdata()
        {
            _eventAggregator.Publish(new HideMessage
            {
                Cancertext = "取消",
                CancerVisibility = System.Windows.Visibility.Visible,
                ConfireVisibility = System.Windows.Visibility.Visible,
                HeaderText = "警告",
                Messages = "无法开始试验"
            });
            _windowManger.ShowDialog(_timeViewModel);
        }
        public void CommitRecData()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    if (_xmlconfig.GetAddNodeValue("UpdataTransFornerUi") != "False")
                    {
                       
                        _eventAggregator.Publish(new OutTestResult { stataThree = _communicationProtocol.ReadStataThree(), CgfVolate = _communicationProtocol.GetCgfVolate() });
                        Thread.Sleep(Convert.ToInt32(_xmlconfig.GetAddNodeValue("UpdataTransFormerSpeedUI")));
                    }
                }
            });
        }
        public void Handle(string message)
        {
            Age = message;
        }

        public void Handle(Stata message)
        {
            if (message == Stata.Redo)
            {

            }
        }

        public string Age { get; set; } = "手动调压";
    }

    public struct OutTestResult
    {
        public StataThree stataThree;
        public string CgfVolate;
    }
}
