using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PortableEquipment.ViewModels
{
    public class MainViewModel : Screen,IHandle<string>
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
        public MainViewModel(IWindowManager windowManager, DataManagementViewModel ChildDialog,
            ManuallySetParametersViewModel manuallySetParametersViewModel, ManualVoltageViewModel manualVoltageViewModel,
            ParameterSettingViewModel parameterSettingViewModel, TransformerViewModel transformerViewModel,
            VoltageTestViewModel voltageTestViewModel, WithstandVoltageViewModel withstandVoltageViewModel, IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);

            _windowManger = windowManager;
            _ChildDialog = ChildDialog;
            _ManuallySetParametersViewModel = manuallySetParametersViewModel;
            _ManualVoltageViewModel = manualVoltageViewModel;
            _ParameterSettingViewModel = parameterSettingViewModel;
            _TransformerViewModel = transformerViewModel;
            _VoltageTestViewModel = voltageTestViewModel;
            _WithstandVoltageViewModel = withstandVoltageViewModel;
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

        public void Handle(string message)
        {
            Age = message;
        }

        public string Age { get; set; }

    }
}
