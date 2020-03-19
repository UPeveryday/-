using Stylet;
using StyletIoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace PortableEquipment.Servers.CommunicationProtocol
{
    public class CommunicationProtocol : Screen, ICommunicationProtocol
    {
        [Inject]
        public StyletLogger.ILogger _logger;
        [Inject]
        public IParsingdata _parsingdata;
        public StataTwo ReadStataTwo()
        {
            var rec = new byte[34];
            int num = 0;
            for (int i = 0; i < 10; i++)
            {
                if (num == 34)
                {
                    if (rec[0] == 0xda && rec[1] == 0xda)
                        return _parsingdata.ParsingTwo(rec);
                    else
                        _logger.Writer("CommunicationProtocol: " + new System.Diagnostics.StackTrace(1, true).GetFrame(0).GetFileLineNumber().ToString() + " 行" + "。 解析ReadStataTwo数据头出错");
                }
                else
                {
                    num = Comport.Serial.serialport.SendCommand(new byte[2] { 0x02, 0xda }, ref rec, 10);
                    _logger.Writer(rec);
                }
            }
            return new StataTwo { stata = Methonstata.False };
        }

        public StataThree ReadStataThree()
        {
            var lc = new StackTrace(new System.Diagnostics.StackFrame(true)).GetFrame(0);
            var rec = new byte[53];
            try
            {
               Comport.Serial.serialport.SendCommand(new byte[2] { 0x03, 0xda }, ref rec, 100);
                if (rec[0] == 0xda && rec[1] == 0xda)
                    return _parsingdata.ParsingThree(rec);
                else
                    _logger.Writer(lc.GetFileName() + "  " + lc.GetFileLineNumber().ToString() + " 行" + "。 解析ReadStataThree数据头出错");
            }
            catch
            {
                _logger.Writer(lc.GetFileName() + "  " + lc.GetFileLineNumber().ToString() + " 行  ." + "SendComman出错");
            }
            return new StataThree { };
        }

        public void SetUpCurrent()
        {
        }
    }
}
