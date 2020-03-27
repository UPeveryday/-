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
            DatagridData = message.DatagridData;
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
        /// <summary>
        /// 添加datagrid数据
        /// </summary>
        public void AddDatagrid()
        {
            DatagridTestData.Add(new TransformerTestData { Stepname = "A", TestTime = "5", TestVoltage = "500", VVoltage = "256" });
        }
    }
    public partial class TransformerViewModel
    {
        public string TestId { get; set; }
        public double RatedVoltage { get; set; }
        public string WindingGroup { get; set; }
        public double Temperature { get; set; }
        public string RatedCapacity { get; set; }
        public double Humidity { get; set; }
        public BindableCollection<TransformerDataStep> DatagridData;
        public BindableCollection<TransformerTestData> DatagridTestData { get; set; } = new BindableCollection<TransformerTestData>();
    }


    public class TransformerTestData
    {
        public string Stepname { get; set; }
        public string TestVoltage { get; set; }
        public string TestTime { get; set; }
        public string UVoltage { get; set; }
        public string VVoltage { get; set; }
        public string WVoltage { get; set; }
    }
}
