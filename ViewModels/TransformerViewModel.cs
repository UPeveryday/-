using PortableEquipment.Servers.CommunicationProtocol;
using PortableEquipment.TestParameters;
using Stylet;
using StyletIoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PortableEquipment.ViewModels
{
    public partial class TransformerViewModel : Screen, IHandle<Translator>
    {
        [Inject]
        public ICommunicationProtocol _communicationProtocol;
        [Inject]
        public Servers.CHangeVolate.ISetVolate _setVolate;
        [Inject]
        public Servers.Xmldata.IXmlconfig _xmlconfig;
        public IEventAggregator _eventAggregator;
        public TransformerViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this, "Translator");
        }
    }

    public partial class TransformerViewModel
    {
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

            ShowDataUi();
        }
        public async void StartTest(Translator TestPra)
        {
            var data = TestPra.DatagridData.ToArray();
            int i = 0;
            while (i < data.Length)
            {
                await _setVolate.SettindVolate(data[i].TestVolate, _communicationProtocol, _xmlconfig);
                for (int j = 0; j < data[j].TestTime / 5; j++)
                {
                    await Task.Delay(300000);
                    AddDatagrid(data[i].Stepname, ((j + 1) * 5).ToString(), UVolateUi.ToString());
                }
                await Task.Delay(data[i].TestTime % 5 * 60 * 1000);
                AddDatagrid(data[i].Stepname, data[i].TestTime.ToString(), UVolateUi.ToString());
                i++;
            }
        }
        //实时更新Ui
        private void ShowDataUi()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    if (_xmlconfig.GetAddNodeValue("UpdataTransFornerUi") != "False")
                    {
                        StataThree testdata = _communicationProtocol.ReadStataThree();
                        CurrentUi = testdata.ACurrent;
                        VolateUi = testdata.AVolate;
                        FreUi = testdata.Fre;
                        Thread.Sleep(Convert.ToInt32(_xmlconfig.GetAddNodeValue("UpdataTransFormerSpeedUI")));
                    }
                }
            });
        }
        /// <summary>
        /// 添加datagrid数据
        /// </summary>
        public void AddDatagrid(string Stepname, string testtime, string testvolate, string vvolate = "", string Uvolate = "", string wvolate = "")
        {
            DatagridTestData.Add(new TransformerTestData { Stepname = Stepname, TestTime = testtime, TestVoltage = testvolate, VVoltage = vvolate, UVoltage = Uvolate, WVoltage = wvolate });
        }
    }
    public partial class TransformerViewModel
    {
        #region 实时显示
        public double UVolateUi { get; set; }
        public double VolateUi { get; set; }
        public double FreUi { get; set; }
        public double CurrentUi { get; set; }
        public double TimeUi { get; set; }
        #endregion
        #region bindinds
        public Translator TestPra { get; set; }
        public string TestId { get; set; }
        public double RatedVoltage { get; set; }
        public string WindingGroup { get; set; }
        public double Temperature { get; set; }
        public string RatedCapacity { get; set; }
        public double Humidity { get; set; }
        public BindableCollection<TransformerDataStep> DatagridData;
        public BindableCollection<TransformerTestData> DatagridTestData { get; set; } = new BindableCollection<TransformerTestData>();
        #endregion
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
