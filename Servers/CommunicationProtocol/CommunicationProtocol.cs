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
            var lc = new StackTrace(new StackFrame(true)).GetFrame(0);
            var rec = new byte[53];
            try
            {
                int recnum = Comport.Serial.serialport.SendCommand(new byte[2] { 0x03, 0xda }, ref rec, 100);
                if (rec[0] == 0xda && rec[1] == 0xda)
                    return _parsingdata.ParsingThree(rec);
                else
                    _logger.Writer(lc.GetFileName() + "  " + lc.GetFileLineNumber().ToString() + " 行" + "。 解析ReadStataThree数据头出错");
            }
            catch
            {
                _logger.Writer(lc.GetFileName() + "  " + lc.GetFileLineNumber().ToString() + " 行  ." + "SendComman出错");
            }
            return new StataThree { Checked = false };
        }
        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="testKind">选择实验类型</param>
        /// <param name="ClickNum">按键次数</param>
        /// <returns></returns>
        public bool SetTestPra(TestKind testKind, byte ClickNum)
        {
            try
            {
                var rec = new byte[2];
                byte TestKindByte = 0x00;
                byte Mark = 0x00;
                if (testKind == TestKind.ControlsVolateUP)
                {
                    TestKindByte = 0x01;
                    Mark = 0x01;
                }
                if (testKind == TestKind.ControlsVolateDown)
                {
                    TestKindByte = 0x01;
                    Mark = 0x02;
                }
                if (testKind == TestKind.ControlsFreUp)
                {
                    TestKindByte = 0x02;
                    Mark = 0x01;
                }
                if (testKind == TestKind.ControlsFreDown)
                {
                    TestKindByte = 0x02;
                    Mark = 0x02;
                }
                if (testKind == TestKind.Setting)
                {
                    TestKindByte = 0x03;
                    Mark = 0x05;
                }
                if (testKind == TestKind.Start)
                {
                    TestKindByte = 0x04;
                    Mark = 0x06;
                }
                if (testKind == TestKind.Stop)
                {
                    TestKindByte = 0x05;
                    Mark = 0x07;
                }
                byte[] sendc = new byte[5] { 0xA5, TestKindByte, ClickNum, Mark, (byte)(0xA5 + TestKindByte + ClickNum + Mark) };
                Comport.Serial.serialport.SendCommand(sendc, ref rec, 1000);
                return rec == new byte[2] { 0xAA, Mark };
            }
            catch
            {
                _logger.Writer("Class: CommunicationProtocol.SetTestPra出错");
                return false;
            }

        }

        public string GetCgfVolate()
        {
            var lc = new StackTrace(new StackFrame(true)).GetFrame(0);
            var rec = new byte[100];
            try
            {
                int recnum = Comport.Serial.Cgfserialport.SendCommand(new byte[3] { 0x46, 0x80,0x80 }, ref rec, 100);
                if (recnum > 1)
                    return Encoding.ASCII.GetString(rec.Skip(0).Take(recnum).ToArray()).Replace("F", "");
                else
                    _logger.Writer(lc.GetFileName() + "  " + lc.GetFileLineNumber().ToString() + " 行" + "。 解析GetCgfVolate数据头出错");
            }
            catch
            {
                _logger.Writer(lc.GetFileName() + "  " + lc.GetFileLineNumber().ToString() + " 行  ." + "SendComman出错");
            }
            return null;
        }
    }
    public enum TestKind
    {
        ControlsVolateUP = 0,
        ControlsVolateDown = 1,
        ControlsFreUp = 2,
        ControlsFreDown = 3,
        Setting = 4,
        Start = 5,
        Stop = 6
    }
}
