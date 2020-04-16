using Stylet;
using StyletIoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;

namespace PortableEquipment.Servers.CommunicationProtocol
{
    public class CommunicationProtocol : Screen, ICommunicationProtocol
    {
        [Inject]
        public StyletLogger.ILogger _logger;
        [Inject]
        public IParsingdata _parsingdata;
        [Inject]
        public Xmldata.IXmlconfig _xmlconfig;
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
        public async Task<StataThree> ReadStataThree()
        {
            Models.StaticFlag.UI_FRESH = false;
            await Task.Delay(10);
            var lc = new StackTrace(new StackFrame(true)).GetFrame(0);
            var rec = new byte[53]; int i = 0;
            try
            {
            p: await Task.Run(() =>
            {
                Thread.Sleep(200);
                int recnum = Comport.Serial.serialport.SendCommand(new byte[2] { 0x03, 0xda }, ref rec, 1000);
            });
                if (rec[0] == 0xda && rec[1] == 0xda)
                {
                    var ret = _parsingdata.ParsingThree(rec);
                    if (ret.Checked)
                    {
                        Models.StaticFlag.UI_FRESH = true;
                        return ret;
                    }
                }
                else
                {
                    if (++i < 50)
                        goto p;
                }
            }
            catch
            {
                _logger.Writer(lc.GetFileName() + "  " + lc.GetFileLineNumber().ToString() + " 行  ." + "SendComman出错");
            }
            Models.StaticFlag.UI_FRESH = true;
            return new StataThree { Checked = false };
        }
        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="testKind">选择实验类型</param>
        /// <param name="ClickNum">按键次数</param>
        /// <returns></returns>
        public async Task<bool> SetTestPra(TestKind testKind, byte ClickNum)
        {
            try
            {
                await Task.Delay(500);
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
                    Mark = 0x03;
                }
                if (testKind == TestKind.ControlsFreDown)
                {
                    TestKindByte = 0x02;
                    Mark = 0x04;
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
                await Task.Run(() =>
                {
                    Thread.Sleep(200);
                    Comport.Serial.upserialport.SendCommand(sendc, ref rec, 10000);
                });
                if (rec[0] == 0xaa && rec[1] == Mark)
                    return true;
                return false;
            }
            catch
            {
                _logger.Writer("Class: CommunicationProtocol.SetTestPra出错");
                return false;
            }

        }

        public async Task<string> GetCgfVolate()
        {
            var lc = new StackTrace(new StackFrame(true)).GetFrame(0);
            var rec = new byte[100];
            try
            {
                int recnum = 0;
            Start: await Task.Run(() =>
            {
                Thread.Sleep(200);
                recnum = Comport.Serial.Cgfserialport.SendCommand(new byte[3] { 0x46, 0x80, 0x80 }, ref rec, 1000);
            });
                if (recnum > 1)
                    return Encoding.ASCII.GetString(rec.Skip(0).Take(recnum).ToArray()).Replace("F", "").Trim().Replace("?", "");
                else
                {
                    _logger.Writer(lc.GetFileName() + "  " + lc.GetFileLineNumber().ToString() + " 行" + "。 解析GetCgfVolate数据头出错");
                    goto Start;
                }
            }
            catch
            {
                _logger.Writer(lc.GetFileName() + "  " + lc.GetFileLineNumber().ToString() + " 行  ." + "SendComman出错");
            }
            return string.Empty;
        }

        public async Task<bool> ThicknessAdjustable(bool Adjustt)
        {
            var lc = new StackTrace(new StackFrame(true)).GetFrame(0);
            var rec = new byte[2];
            byte[] comman = new byte[3];
            if (Adjustt) comman = new byte[] { 0xA5, 0x08, 0xAD };
            else comman = new byte[] { 0xA5, 0x09, 0xAE };
            try
            {
                await Task.Run(() =>
                {
                    int recnum = Comport.Serial.upserialport.SendCommand(comman, ref rec, 100);
                });
                if (rec[0] == 0xAA && rec[1] == 0x05)
                    return true;
                else if (rec[0] == 0xEE && rec[1] == 0x05)
                    _logger.Writer(lc.GetFileName() + "  " + lc.GetFileLineNumber().ToString() + " 行" + "粗细条失败");
                else
                    _logger.Writer(lc.GetFileName() + "  " + lc.GetFileLineNumber().ToString() + " 行" + "接受粗细条数据失败");

            }
            catch
            {
                _logger.Writer(lc.GetFileName() + "  " + lc.GetFileLineNumber().ToString() + " 行  ." + "粗细条异常");
            }
            return false;
        }

        public async Task<StataThree> ReadStataThree(int num)
        {
            var lc = new StackTrace(new StackFrame(true)).GetFrame(0);
            var rec = new byte[53]; int i = 0;
            try
            {
            p: await Task.Run(() =>
            {
                Thread.Sleep(200);
                int recnum = Comport.Serial.serialport.SendCommand(new byte[2] { 0x03, 0xda }, ref rec, 1000);
            });
                if (rec[0] == 0xda && rec[1] == 0xda)
                {
                    var ret = _parsingdata.ParsingThree(rec);
                    if (ret.Checked)
                        return ret;
                }
                else
                {
                    if (++i < 50)
                        goto p;
                }
            }
            catch
            {
                _logger.Writer(lc.GetFileName() + "  " + lc.GetFileLineNumber().ToString() + " 行  ." + "SendComman出错");
            }
            return new StataThree { Checked = false };
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
