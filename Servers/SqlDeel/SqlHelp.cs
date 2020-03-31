using PortableEquipment.TestParameters;
using StyletIoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortableEquipment.Servers.SqlDeel
{
    public class SqlHelp : ISqlHelp
    {
        [Inject]
        public Json.IJsondeel _jsondeel;
        [Inject]
        public Servers.IEntityServer entityServer;
        [Inject]//特性注入
        public StyletLogger.ILogger _logger;
        public bool SaveTransformerDataBase(Translator testmessage)
        {
            try
            {
                Model.Transformer trs = new Model.Transformer
                {
                    TestId = testmessage.TestId,
                    RatedVoltage = testmessage.RatedVoltage,
                    RatedCapacity = testmessage.RatedCapacity,
                    WindingGroup = testmessage.WindingGroup,
                    Temperature = testmessage.Temperature,
                    Humidity = testmessage.Humidity,
                    TestLocation = testmessage.TestLocation,
                    Tester = testmessage.Tester,
                    Frequency = testmessage.Frequency,
                    Volate = testmessage.Volate,
                    currentnum = testmessage.Current,
                    DateTime = testmessage.DateTime,
                    TestKind = "配电变压器试验",
                    DatagridData = _jsondeel.GetJsonByclass(testmessage.DatagridData),
                    TestResult = _jsondeel.GetJsonByclass(testmessage.TestResultData)
                };
                entityServer.EfModel.Transformers.Add(trs);
                entityServer.EfModel.SaveChanges();
                return true;
            }
            catch
            {
                _logger.Writer("WithstandVoltageViewModel,保存任务单EF错误");
            }
            return false;
        }


        public bool SaveMutualTranslatorTransformerDataBase(MutualTranslator testmessage)
        {
            try
            {
                Model.MutualTranslator trs = new Model.MutualTranslator
                {
                    TestLevel = testmessage.TestLevel,
                    TestId = testmessage.TestId,
                    Humidity = testmessage.Humidity,
                    Temperature = testmessage.Temperature,
                    TestLocation = testmessage.TestLocation,
                    Tester = testmessage.Tester,
                    TestKind = "互感器试验",
                    DateTime = testmessage.DateTime,
                    Parameters = _jsondeel.GetJsonByclass(testmessage)
                };
                entityServer.EfModel.MutualTranslators.Add(trs);
                entityServer.EfModel.SaveChanges();
                return true;
            }
            catch
            {
                _logger.Writer("ParameterSettingViewModel,保存任务单EF错误");
            }
            return false;
        }

    }
}
