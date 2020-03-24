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
    public partial class ManuallySetParametersViewModel : Screen
    {
        private IWindowManager _windowManger;
        private ManualVoltageViewModel _ManualVoltageViewModel;
        [Inject]
        public IEventAggregator _eventAggregator;
        public ManuallySetParametersViewModel(IWindowManager windowManager, ManualVoltageViewModel manualVoltageViewModel)
        {
            _windowManger = windowManager;
            _ManualVoltageViewModel = manualVoltageViewModel;
        }


    }
    public partial class ManuallySetParametersViewModel
    {
        #region Command
        private TestByHand GetByHandData()
        {
            return new TestByHand
            {
                Volate = Volate,
                Current = Current,
                OverVolate = OverVolate,
                OverCurrent = OverCurrent,
                VariableThan = VariableThan,
                Promotion = Promotion
            };
        }

        public void ShowManualVoltageViewModel()
        {
            _eventAggregator.Publish(GetByHandData());
            _windowManger.ShowDialog(_ManualVoltageViewModel);
        }
        #endregion
    }

    public partial class ManuallySetParametersViewModel
    {
        public int CurrentKindNum { get; set; }
        public int VolateKindNum { get; set; }
        #region Buhand
        public VolateKind Volate
        {
            get
            {
                if (VolateKindNum == 1)
                    return VolateKind.High;
                else
                    return VolateKind.Low;
            }
        }
        public CurrentKind Current
        {
            get
            {
                if (CurrentKindNum == 1)
                    return CurrentKind.Big;
                else
                    return CurrentKind.Small;
            }
        }
        public double OverVolate { get; set; }
        public double OverCurrent { get; set; }
        public double VariableThan { get; set; }//变比s
        public double Promotion { get; set; }//荣升系数
        #endregion
    }
}
