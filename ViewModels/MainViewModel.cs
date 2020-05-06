using PortableEquipment.Models;
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
    public class MainViewModel : Screen, IHandle<Stata>
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
        [Inject]
        public Servers.SelfCheck.ISelfCheck _SelfCheck;

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
        public async void CommitRecData()
        {
            var p = await _SelfCheck.ComPortCheck();
            if (p.IsOk)
            {
                Task powerdataupdata = new Task(async () =>
                {
                    while (true)
                    {
                        if (StaticFlag.UI_FRESH)
                        {
                            _eventAggregator.Publish(new OutTestResult { stataThree = await _communicationProtocol.ReadStataThree(0) });
                            Thread.Sleep(System.Convert.ToInt32(_xmlconfig.GetAddNodeValue("UpdataTransFormerSpeedUI")));
                        }
                    }
                }, TaskCreationOptions.LongRunning);
                powerdataupdata.Start();

                Task cgfdataupdata = new Task(async () =>
                {
                    while (true)
                    {
                        if (StaticFlag.CFG_FRESH)
                        {
                            var cgddata = await _communicationProtocol.GetCgfVolate();
                            if (cgddata != string.Empty)
                                _eventAggregator.Publish(cgddata);
                            Thread.Sleep(50);
                        }
                    }
                }, TaskCreationOptions.LongRunning);
                cgfdataupdata.Start();

                Task BoomTest = new Task(async () =>
                {
                    while (true)
                    {
                        await _CommunicationProtocol.Boom();
                        await Task.Delay(1000);
                    }
                }, TaskCreationOptions.LongRunning);
                BoomTest.Start();

            }
            else
            {
                System.Windows.MessageBoxResult c = _windowManger.ShowMessageBox(p.hidemessage, "警告", System.Windows.MessageBoxButton.OK);
                if (c == System.Windows.MessageBoxResult.OK)
                    this.RequestClose();
            }

             
        }
        public void Handle(Stata message)
        {
            if (message == Stata.Redo)
            {

            }
        }

        public async void Upvolate()
        {
            await _communicationProtocol.SetTestPra(TestKind.Start, 2);
            await Task.Delay(6500);
            var p = await _setVolate.SettindVolate(System.Convert.ToDouble(_xmlconfig.GetAddNodeValue("needvolatetemp")), _communicationProtocol, _xmlconfig);
            var c = p;
        }

        public string Age { get; set; } = "手动调压";


        static CancellationTokenSource tokenSource = new CancellationTokenSource();
        CancellationToken token = tokenSource.Token;
        ManualResetEvent resetEvent = new ManualResetEvent(true);
    }

    public struct OutTestResult
    {
        public StataThree stataThree;
        public string CgfVolate;
    }
}
