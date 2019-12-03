using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortableEquipment.ViewModels
{
    public class ManuallySetParametersViewModel:Screen
    {
        private IWindowManager _windowManger;
        private ManualVoltageViewModel _ManualVoltageViewModel;
        public ManuallySetParametersViewModel(IWindowManager windowManager,ManualVoltageViewModel manualVoltageViewModel)
        {
            _windowManger = windowManager;
            _ManualVoltageViewModel = manualVoltageViewModel;
        }

        public void ShowManualVoltageViewModel() => _windowManger.ShowDialog(_ManualVoltageViewModel);
    }
}
