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
    public partial class DataManagementViewModel : Screen
    {
        #region  依赖注入
        [Inject]
        public Servers.IEntityServer _entityServer;
        [Inject]
        public Servers.Xmldata.IXmlconfig _xmlconfig;
        [Inject]
        public StyletLogger.ILogger _logger;
        [Inject]
        public Servers.Json.IJsondeel _jsondeel;
        [Inject]
        public IWindowManager _windowManager;
        #endregion
    }

    public partial class DataManagementViewModel
    {
        #region COMMAN
        public async void GetAllTestTask()
        {
            await Task.Run(() =>
            {
                try
                {
                    TestMessage = new BindableCollection<TestDataKind>();
                    foreach (var item in _entityServer.EfModel.Transformers)
                    {
                        TestMessage.Add(new TestDataKind
                        {
                            DataBaseId = item.id,
                            Id = item.TestId,
                            TestKind = item.TestKind,
                            VolateRange = item.RatedVoltage.ToString(),
                            Tester = item.Tester,
                            Humidity = item.Humidity.ToString(),
                            Temperature = item.Temperature.ToString(),
                            TestLocation = item.TestLocation,
                            TestTime = item.DateTime.ToString()
                        });
                    }
                    foreach (var item in _entityServer.EfModel.MutualTranslators)
                    {
                        TestMessage.Add(new TestDataKind
                        {
                            DataBaseId = item.id,
                            Id = item.TestId,
                            TestKind = item.TestKind,
                            VolateRange = item.TestLevel,
                            Tester = item.Tester,
                            Humidity = item.Humidity.ToString(),
                            Temperature = item.Temperature.ToString(),
                            TestLocation = item.TestLocation,
                            TestTime = item.DateTime.ToString()
                        });
                    }
                }
                catch
                {
                    _logger.Writer("初始化测量任务失败");
                }
            });
        }
        /// <summary>
        /// 删除选中的节点
        /// </summary>
        public async void DeleteNeedNotdata()
        {
            await Task.Run(() =>
            {
                foreach (var item in TestMessage)
                {
                    if (item.IsCkecked)
                    {
                        if (item.TestKind == "配电变压器试验")
                        {
                            var c = _entityServer.EfModel.Transformers.Where(p => p.id == item.DataBaseId);
                            _entityServer.EfModel.Transformers.RemoveRange(c);
                        }
                        if (item.TestKind == "互感器试验")
                        {
                            var c = _entityServer.EfModel.MutualTranslators.Where(p => p.id == item.DataBaseId);
                            _entityServer.EfModel.MutualTranslators.RemoveRange(c);
                        }
                    }
                }
                _entityServer.EfModel.SaveChanges();
                GetAllTestTask();
            });

        }

        private void LcGetTestVolateRange(ExcitationCharacteristic ec)
        {
            LcDatagrid.Clear();
            FirstName = "序号";
            SecondName = "电压(V)";
            ThirdName = "电流(A)";
            for (int i = 0; i < 11; i++)
            {
                if (i < ec.VolateRange.Length)
                {
                    LcDatagrid.Add(new LcdatagridColletion
                    {
                        TestNum = (i + 1).ToString(),
                        TestVoltage = (ec.TestVolate * ec.VolateRange[i] / 100).ToString(),
                        TestCurrent = ec.OverCurrent.ToString()
                    });
                }
            }
        }

        private void GetUpVolate(BindableCollection<ViewModels.TransformerDataStep> ec, BindableCollection<TransformerTestData> ret = null)
        {
            LcDatagrid.Clear();
            FirstName = "步骤";
            SecondName = "试验电压(V)";
            ThirdName = "试验时间(Min)";
            foreach (var item in ec)
            {
                LcDatagrid.Add(new LcdatagridColletion
                {
                    TestNum = item.Stepname,
                    TestVoltage = item.TestVolate.ToString(),
                    TestCurrent = item.TestTime.ToString(),

                });
            }

            if (ret != null)
                DatagridTestData = ret;
            else
            {
                DatagridTestData = null;
            }
        }
        #endregion
    }

    public partial class DataManagementViewModel
    {
        #region
        public BindableCollection<TestDataKind> TestMessage { get; set; }
        public BindableCollection<LcdatagridColletion> LcDatagrid { get; set; } = new BindableCollection<LcdatagridColletion>();
        public BindableCollection<TransformerTestData> DatagridTestData { get; set; } = new BindableCollection<TransformerTestData>();
        public System.Windows.Visibility TransformerVisibility { get; set; } = System.Windows.Visibility.Hidden;
        private TestDataKind testDataKind;
        public TestDataKind Selectdata
        {
            get
            {
                return testDataKind;
            }
            set
            {
                testDataKind = value;
                try
                {
                    if (Selectdata != null && Selectdata.TestKind == "互感器试验")
                    {
                        TransformerVisibility = System.Windows.Visibility.Hidden;
                        var c = _entityServer.EfModel.MutualTranslators.Where(p => p.id == Selectdata.DataBaseId);//查找Id对应任务单
                        var mc = _jsondeel.GetClassFromStr<MutualTranslator>(c.ToArray()[0].Parameters);//序列化回类
                        KzVolate = mc.NoLoadCurrentR.TestVolate + " V";
                        KzCurrent = mc.NoLoadCurrentR.OverCurrent + " A";
                        KeepVolate = mc.InducedOvervoltageR.TestVoltage + " V";
                        KeepFre = mc.InducedOvervoltageR.TestFre + " Hz";
                        KeepTime = mc.InducedOvervoltageR.TestTime + " S";
                        LcGetTestVolateRange(mc.ExcitationCharacteristicR);//画图表
                    }
                    if (Selectdata != null && Selectdata.TestKind == "配电变压器试验")
                    {
                        TransformerVisibility = System.Windows.Visibility.Visible;
                        var c = _entityServer.EfModel.Transformers.Where(p => p.id == Selectdata.DataBaseId);//查找Id对应任务单
                        var mc = _jsondeel.GetClassFromStr<BindableCollection<TransformerDataStep>>(c.ToArray()[0].DatagridData);//序列化回类
                        var ret = _jsondeel.GetClassFromStr<BindableCollection<TransformerTestData>>(c.ToArray()[0].TestResult);//序列化回类
                        TrsCaptance = c.ToArray()[0].RatedCapacity;
                        TrsFre = (double)c.ToArray()[0].Frequency;
                        TrsOverCurrent = (double)c.ToArray()[0].currentnum;
                        TrsOverVolata = (double)c.ToArray()[0].Volate;
                        TrsWindGroup = c.ToArray()[0].WindingGroup;
                        GetUpVolate(mc, ret);
                    }
                }
                catch
                {
                    _logger.Writer("励磁特性图表数据获取出错，EF database可能未连接");
                }
            }
        }

        public string FirstName { get; set; } = "序号";
        public string SecondName { get; set; } = "电压(V)";
        public string ThirdName { get; set; } = "电流(A)";
        public string KzVolate { get; set; }
        public string KzCurrent { get; set; }
        public string KeepVolate { get; set; }
        public string KeepFre { get; set; }
        public string KeepTime { get; set; }

        public string TrsCaptance { get; set; }
        public double TrsFre { get; set; }
        public double TrsOverCurrent { get; set; }
        public double TrsOverVolata { get; set; }
        public string TrsWindGroup { get; set; }


        #endregion

    }

    public class TestDataKind
    {
        public bool IsCkecked { get; set; }
        public string Id { get; set; }
        public int DataBaseId { get; set; }
        public string TestKind { get; set; }
        public string VolateRange { get; set; }
        public string Tester { get; set; }
        public string Humidity { get; set; }
        public string Temperature { get; set; }
        public string TestLocation { get; set; }
        public string TestTime { get; set; }

    }
}
