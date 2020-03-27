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
            LcGetTestVolateRange(_mutualTranslator.ExcitationCharacteristicR);
            GetBasePra(message);
        }
    }
    public partial class VoltageTestViewModel
    {
        #region Commands
        /// <summary>
        /// datagrid
        /// </summary>
        /// <param name="ec"></param>
        private void LcGetTestVolateRange(ExcitationCharacteristic ec)
        {
            LcDatagrid.Clear();
            for (int i = 0; i < ec.VolateRange.Length; i++)
            {
                LcDatagrid.Add(new LcdatagridColletion { TestNum = (i + 1).ToString(), TestVoltage = (ec.TestVolate * ec.VolateRange[i] / 100).ToString(), TestCurrent = ec.OverCurrent.ToString() });
            }
        }

        private void GetBasePra(MutualTranslator mutualTranslator)
        {
            TestId = mutualTranslator.TestId;
            TestLevel = mutualTranslator.TestLevel;
            Humidity = mutualTranslator.Humidity;
            Temperature = mutualTranslator.Temperature;

            KzTestVolate = mutualTranslator.NoLoadCurrentR.TestVolate;
            KzOverCurrent = mutualTranslator.NoLoadCurrentR.OverCurrent;

            KeepTestVoltage = mutualTranslator.InducedOvervoltageR.TestVoltage;
            KeepTestFre = mutualTranslator.InducedOvervoltageR.TestFre;
            KeepTestTime = mutualTranslator.InducedOvervoltageR.TestTime;
        }
        #endregion
    }
    public partial class VoltageTestViewModel
    {
        #region Bindings
        public BindableCollection<LcdatagridColletion> LcDatagrid { get; set; } = new BindableCollection<LcdatagridColletion>();

        public string TestId { get; set; } 
        public string TestLevel { get; set; }
        public double Humidity { get; set; }
        public double Temperature { get; set; }

        public double KzTestVolate { get; set; }
        public double KzOverCurrent { get; set; }


        public double KeepTestVoltage { get; set; }
        public double KeepTestFre { get; set; }
        public double KeepTestTime { get; set; }
        #endregion
    }
    public class LcdatagridColletion
    {
        public string TestNum { get; set; }
        public string TestVoltage { get; set; }
        public string TestCurrent { get; set; }
    }
}
