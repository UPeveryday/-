using LiveCharts;
using PortableEquipment.TestParameters;
using Stylet;
using StyletIoC;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
                RatedVoltage = RatedVoltage,
                RatedCapacity = RatedCapacity == null ? "--未指定--" : RatedCapacity,
                WindingGroup = WindingGroup == null ? "--未指定--" : WindingGroup,
                Temperature = Temperature == null ? "--未指定--" : Temperature,
                Humidity = Humidity == null ? "--未指定--" : Humidity,
                TestLocation = TestLocation == null ? "--未指定--" : TestLocation,
                Tester = Tester == null ? "--未指定--" : Tester,
                Frequency = Frequency,
                Volate = Volate,
                Current = Current,
                DatagridData = DatagridData
            };
        }
        public void StartTest() => _eventAggregator.Publish(GetTranslatorTest(), "Translator");
        #endregion
    }
    public partial class WithstandVoltageViewModel
    {
        #region Command
        private BindableCollection<TransformerDataStep> GetdatagridData()
        {
            var needvolate = (int)_ratavoltage;
            BindableCollection<TransformerDataStep> tempdata = new BindableCollection<TransformerDataStep>();
            tempdata.Add(GetTransformerDataStep("A", 5, 1.1));
            tempdata.Add(GetTransformerDataStep("B", 5, 1.3));
            tempdata.Add(GetTransformerDataStep("C", 5, 1.5));
            tempdata.Add(GetTransformerDataStep("D", 5, 1.3));
            tempdata.Add(GetTransformerDataStep("E", 5, 1.1));
            return tempdata;
        }

        private TransformerDataStep GetTransformerDataStep(string name, int time, double needs)
        {
            TransformerDataStep transformerData = new TransformerDataStep();
            transformerData.Stepname = name;
            transformerData.TestTime = time;
            transformerData.TestVolate = (int)(needs * _ratavoltage / Math.Sqrt(3));
            return transformerData;
        }
        #endregion
    }
    public partial class WithstandVoltageViewModel
    {
        #region Bindings
        public string TestId { get; set; }

        private double _ratavoltage;

        public double RatedVoltage
        {
            get { return _ratavoltage; }
            set
            {
                _ratavoltage = value;
                DatagridData = GetdatagridData();
            }
        }
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
        #region Data
        public BindableCollection<TransformerDataStep> DatagridData { get; set; }
        #endregion
    }

    public class TransformerDataStep
    {
        public string Stepname { get; set; }
        public int TestTime { get; set; }
        public double TestVolate { get; set; }
    }


}
