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
    public partial class ManualVoltageViewModel : Screen, IHandle<TestByHand>
    {
        public IEventAggregator _eventAggregator;
        public ManualVoltageViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);
        }
        public void Handle(TestByHand message)
        {
            throw new NotImplementedException();
        }
     
    }

    public partial class ManualVoltageViewModel
    {
        #region Command
        #endregion
    }


    public partial class ManualVoltageViewModel
    {
        #region Bindings
        public string Voltage { get; set; } = "5.5kV";


        #endregion
    }
}
