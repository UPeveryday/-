using PortableEquipment.TestParameters;
using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortableEquipment.ViewModels
{
    public partial class VoltageTestViewModel : Screen, IHandle<MutualTranslator>
    {
        public IEventAggregator _eventAggregator;
        private MutualTranslator _mutualTranslator;

        public VoltageTestViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);
        }
        public void Handle(MutualTranslator message)
        {
            _mutualTranslator = message;
        }
    }
    public partial class VoltageTestViewModel
    {
        #region Bindings

        #endregion
    }
}
