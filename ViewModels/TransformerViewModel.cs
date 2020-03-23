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
    public partial class TransformerViewModel : Screen, IHandle<Translator>
    {
        public Translator TestPra { get; set; }
        public void Handle(Translator message)
        {
            TestPra = message;
            TestId = message.TestId;
            RatedVoltage = message.RatedVoltage;
            RatedCapacity = message.RatedCapacity;
            WindingGroup = message.WindingGroup;
            Temperature = message.Temperature;
            Humidity = message.Humidity;
        }
    }

    public partial class TransformerViewModel
    {
        public IEventAggregator _eventAggregator;
        public TransformerViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this, "Translator");
        }
    }


    public partial class TransformerViewModel
    {
        public string TestId { get; set; }
        public string RatedVoltage { get; set; }
        public string WindingGroup { get; set; }
        public string Temperature { get; set; }
        public string RatedCapacity { get; set; }
        public string Humidity { get; set; }
    }
}
