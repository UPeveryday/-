using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortableEquipment.ViewModels
{
    public class WithstandVoltageViewModel : Screen
    {
        private IWindowManager _windowManger;
        private TransformerViewModel _TransformerViewModel;
        public WithstandVoltageViewModel(IWindowManager windowManager, TransformerViewModel transformerViewModel)
        {
            _windowManger = windowManager;
            _TransformerViewModel = transformerViewModel;
        }

        public void ShowtransformerViewModel() => _windowManger.ShowDialog(_TransformerViewModel);
    }
}
