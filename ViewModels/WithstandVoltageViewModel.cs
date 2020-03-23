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
    public partial class WithstandVoltageViewModel : Screen 
    {
        private IWindowManager _windowManger;
        private TransformerViewModel _TransformerViewModel;
        private IEventAggregator _eventAggregator;

        public WithstandVoltageViewModel(IWindowManager windowManager, TransformerViewModel transformerViewModel, IEventAggregator eventAggregator)
        {
            _windowManger = windowManager;
            _TransformerViewModel = transformerViewModel;
            _eventAggregator = eventAggregator;
        }
        public void ShowtransformerViewModel()
        {
            StartTest();
            _windowManger.ShowDialog(_TransformerViewModel);
        }
    }
    public partial class WithstandVoltageViewModel
    {
        #region Command
        public Translator GetTranslatorTest()
        {
            return new Translator
            {
                TestId = TestId == null ? "--未指定--" : TestId,
                RatedVoltage = RatedVoltage == null ? "--未指定--" : RatedVoltage,
                RatedCapacity = RatedCapacity == null ? "--未指定--" : RatedCapacity,
                WindingGroup = WindingGroup == null ? "--未指定--" : WindingGroup,
                Temperature = Temperature == null ? "--未指定--" : Temperature,
                Humidity = Humidity == null ? "--未指定--" : Humidity,
                TestLocation = TestLocation == null ? "--未指定--" : TestLocation,
                Tester = Tester == null ? "--未指定--" : Tester,
                Frequency = Frequency,
                Volate = Volate,
                Current = Current
            };
        }
        public void StartTest() => _eventAggregator.Publish(GetTranslatorTest(), "Translator");
        #endregion
    }


    public partial class WithstandVoltageViewModel
    {
        #region Bindings
        public string TestId { get; set; }
        public string RatedVoltage { get; set; }
        public string WindingGroup { get; set; }
        public string Temperature { get; set; }
        public string RatedCapacity { get; set; }
        public string Humidity { get; set; }
        public string TestLocation { get; set; }
        public string Tester { get; set; }
        public double Frequency { get; set; }
        public double Volate { get; set; }
        public double Current { get; set; }
        #endregion
    }


}
