using LiveCharts;
using PortableEquipment.TestParameters;
using Stylet;
using StyletIoC;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortableEquipment.ViewModels
{
    public partial class WithstandVoltageViewModel : Screen
    {
        public IWindowManager _windowManger;
        public TransformerViewModel _TransformerViewModel;
        public IEventAggregator _eventAggregator;
        [Inject]
        public Servers.IEntityServer entityServer;
        public Model.jsEntities _jsEntities;
        [Inject]//特性注入
        public Servers.Json.IJsondeel _jsondeel;
        [Inject]//特性注入
        public StyletLogger.ILogger _logger;

        public IContainer _container;
        public WithstandVoltageViewModel(IWindowManager windowManager, TransformerViewModel transformerViewModel, 
            IEventAggregator eventAggregator,IContainer container)
        {
            _container = container;
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
                Temperature = Temperature,
                Humidity = Humidity,
                TestLocation = TestLocation == null ? "--未指定--" : TestLocation,
                Tester = Tester == null ? "--未指定--" : Tester,
                Frequency = Frequency,
                Volate = Volate,
                Current = Current,
                DatagridData = DatagridData == null ? new BindableCollection<TransformerDataStep>() : DatagridData,
                DateTime = DateTime.Now
            };
        }
        public void StartTest() => _eventAggregator.Publish(GetTranslatorTest());
        #endregion
    }
    public partial class WithstandVoltageViewModel
    {
        #region Command
        private BindableCollection<TransformerDataStep> GetdatagridData()
        {
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
            double Um = 0.00;
            if (_ratavoltage == 10000)
                Um = 12;
            if (_ratavoltage == 35000)
                Um = 40.5;
            if (_ratavoltage == 110000)
                Um = 126;
            if (_ratavoltage == 220000)
                Um = 252;
            if (_ratavoltage == 330000)
                Um = 363;
            if (_ratavoltage == 500000)
                Um = 550;
            TransformerDataStep transformerData = new TransformerDataStep();
            transformerData.Stepname = name;
            transformerData.TestTime = time;
            if (name == "C")
                transformerData.TestTime = Math.Round((120 * (50 / Frequency) / 60), 2);
            transformerData.TestVolate = Math.Round(needs * Um / Math.Sqrt(3), 2);
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

        private int _ratadselectindex = 1;

        public int RatadvolateSelectIndex
        {
            get
            {
                if (_ratadselectindex == 0)
                    RatedVoltage = 10000;
                if (_ratadselectindex == 1)
                    RatedVoltage = 35000;
                if (_ratadselectindex == 2)
                    RatedVoltage = 110000;
                if (_ratadselectindex == 3)
                    RatedVoltage = 220000;
                if (_ratadselectindex == 4)
                    RatedVoltage = 330000;
                if (_ratadselectindex == 5)
                    RatedVoltage = 500000;
                return _ratadselectindex;
            }
            set { _ratadselectindex = value; }
        }


        public string WindingGroup { get; set; }
        public double Temperature { get; set; }
        public string RatedCapacity { get; set; }
        public double Humidity { get; set; }
        public string TestLocation { get; set; }
        public string Tester { get; set; }
        public double Frequency { get; set; } = 50.0;
        public double Volate { get; set; } = 100.00;
        public double Current { get; set; } = 5.0;
        #endregion
        #region Data
        public BindableCollection<TransformerDataStep> DatagridData { get; set; }
        #endregion
    }

    public class TransformerDataStep
    {
        public string Stepname { get; set; }
        public double TestTime { get; set; }
        public double TestVolate { get; set; }
    }


}
