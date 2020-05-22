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
        public async Task<StataThree> ReadStataThree(CancellationToken token)
        {

            await Task.Delay(10);
            var lc = new StackTrace(new StackFrame(true)).GetFrame(0);
            var rec = new byte[53]; int i = 0;
            try
            {
            p: await Task.Run(() =>
            {
                Models.StaticFlag.UI_FRESH = false;
                int recnum = Comport.Serial.serialport.SendCommand(new byte[2] { 0x03, 0xda }, ref rec, 20);
                Models.StaticFlag.UI_FRESH = true;
            });
                token.ThrowIfCancellationRequested();
                if (rec[0] == 0xda && rec[1] == 0xda)
                {
                    var ret = _parsingdata.ParsingThree(rec);
                    if (ret.Checked)
                    {
                        Models.StaticFlag.UI_FRESH = true;
                        return ret;
                    }
                    else
                    {
                        if (++i < 50)
                            goto p;
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
            return new StataThree { Checked = false };

        }
        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="testKind">选择实验类型</param>
        /// <param name="ClickNum">按键次数</param>
        /// <returns></returns>
        public async Task<bool> SetTestPra(TestKind testKind, byte ClickNum, CancellationToken token)
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
                    Comport.Serial.upserialport.SendCommand(sendc, ref rec, ClickNum * 1000);
                });
                if (rec[0] == 0xaa && rec[1] == Mark)
                {
                    return true;
                }

                return false;
            }
            catch
            {
                _logger.Writer("Class: CommunicationProtocol.SetTestPra出错");
                await Task.Delay(100);
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
                recnum = Comport.Serial.Cgfserialport.SendCommand(new byte[3] { 0x46, 0x80, 0x80 }, ref rec, 5);
            });
                if (recnum > 1)
                    return Encoding.ASCII.GetString(rec.Skip(0).Take(recnum).ToArray()).Replace("F", "").Trim().Replace("?", "");
                else
                {
                    //_logger.Writer(lc.GetFileName() + "  " + lc.GetFileLineNumber().ToString() + " 行" + "。 解析GetCgfVolate数据头出错");
                    goto Start;
                }
            }
            catch
            {
                _logger.Writer(lc.GetFileName() + "  " + lc.GetFileLineNumber().ToString() + " 行  ." + "SendComman出错");
            }
            return string.Empty;
        }

        public async Task<double> GetCgfVolateDouble()
        {
            var lc = new StackTrace(new StackFrame(true)).GetFrame(0);
            var rec = new byte[100];
            Models.StaticFlag.UI_FRESH = false;
        Start: try
            {
                int recnum = 0;
                await Task.Run(() =>
                 {
                     recnum = Comport.Serial.Cgfserialport.SendCommand(new byte[3] { 0x46, 0x80, 0x80 }, ref rec, 3);
                 });
                if (recnum > 1)
                {
                    Models.StaticFlag.UI_FRESH = true;
                    return System.Convert.ToDouble(Encoding.ASCII.GetString(rec.Skip(0).Take(recnum).ToArray()).Replace("F", "").Trim().Replace("?", ""));

                }
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
            Models.StaticFlag.UI_FRESH = true;
            goto Start;
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
                _logger.Writer(lc.GetFileName() + "  " + lc.GetFileLineNumber().ToString() + " 行  " + "粗细条异常");
            }
            return false;
        }

        public async Task<bool> Boom()
        {
            var lc = new StackTrace(new StackFrame(true)).GetFrame(0);
            var rec = new byte[2];
            byte[] comman = new byte[2] { 0xa5, 0x0e };
            try
            {
                int recnum = 0;
                await Task.Run(() =>
                 {
                     recnum = Comport.Serial.upserialport.SendCommandBoom(comman, ref rec, 50);
                 });
                if (recnum == 2 && rec[0] == 0xaa && rec[1] == 0x0e)
                    return true;
                else
                    return false;
            }
            catch
            {
                _logger.Writer(lc.GetFileName() + "  " + lc.GetFileLineNumber().ToString() + " 行  ." + "粗细条异常");
            }
            return false;
        }

        public async Task<bool> PressThincness()
        {
            var lc = new StackTrace(new StackFrame(true)).GetFrame(0);
            var rec = new byte[2];
            byte[] comman = new byte[3] { 0xa6, 0x01, 0xa7 };
            try
            {
                await Task.Run(() =>
                {
                    int recnum = Comport.Serial.upserialport.SendCommand(comman, ref rec, 100);
                });
                if (rec[0] == 0xAA && rec[1] == 0x05)
                    return true;
                else
                    _logger.Writer(lc.GetFileName() + "  " + lc.GetFileLineNumber().ToString() + " 行" + "Press False");

            }
            catch
            {
                _logger.Writer(lc.GetFileName() + "  " + lc.GetFileLineNumber().ToString() + " 行  ." + "Press异常");
            }
            return false;
        }

        public async Task<StataThree> ReadStataThree(int num)
        {
            var lc = new StackTrace(new StackFrame(true)).GetFrame(0);
            var rec = new byte[53]; int i = 0;
            try
            {
            readdo: await Task.Run(() =>
            {
                int recnum = Comport.Serial.serialport.SendCommand(new byte[2] { 0x03, 0xda }, ref rec, 100);
            });
                if (rec[0] == 0xda && rec[1] == 0xda)
                {
                    var ret = _parsingdata.ParsingThree(rec);
                    if (ret.Checked)
                        return ret;
                    else
                    {
                        if (++i < 50)
                            goto readdo;
                    }
                }
                else
                {
                    if (++i < 50)
                        goto readdo;
                }
            }
            catch
            {
                _logger.Writer(lc.GetFileName() + "  " + lc.GetFileLineNumber().ToString() + " 行  ." + "SendComman出错");
            }
            return new StataThree { Checked = false };
        }

        public async Task<bool> GetPowerStata()
        {
            var lc = new StackTrace(new StackFrame(true)).GetFrame(0);
            var rec = new byte[1];
            byte[] comman = new byte[3] { 0xa5, 0x0a, 0xaf };
            try
            {
                await Task.Run(() =>
                {
                    int recnum = Comport.Serial.upserialport.SendCommand(comman, ref rec, 100);
                });
                if (rec[0] == 0xAA)
                    return true;
                else if (rec[0] == 0xAB)
                    return false;
                else
                {
                    _logger.Writer(lc.GetFileName() + "  " + lc.GetFileLineNumber().ToString() + " 行" + "获取电源状态指令异常");
                }
            }
            catch
            {
                _logger.Writer(lc.GetFileName() + "  " + lc.GetFileLineNumber().ToString() + " 行  ." + "获取电源状态异常");
            }
            return false;
        }

        public async Task<int> GetCurrentThickness(ICommunicationProtocol _communicationProtocol, CancellationToken token)
        {
            double startvolate = (await _communicationProtocol.ReadStataThree(token)).AVolate;
            double finishvolate = 0;
            bool cf = await _communicationProtocol.SetTestPra(TestKind.ControlsVolateUP, 3, token);
            if (cf)
            {
                finishvolate = (await _communicationProtocol.ReadStataThree(token)).AVolate;
                await _communicationProtocol.SetTestPra(TestKind.ControlsVolateDown, 3, token);
                if (Math.Abs(startvolate - finishvolate) > 5)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            return -1;
        }

        public async Task<int> SwitchThincness(bool open, ICommunicationProtocol _communicationProtocol, CancellationToken token)
        {
            if (open)
            {
                if ((await GetCurrentThickness(_communicationProtocol, token)) == 1)
                {
                    return 1;
                }
                else
                {
                    await _communicationProtocol.ThicknessAdjustable(true);
                    if ((await GetCurrentThickness(_communicationProtocol, token)) == 1)
                    {
                        return 1;
                    }
                    await _communicationProtocol.ThicknessAdjustable(false);
                    if ((await GetCurrentThickness(_communicationProtocol, token)) == 1)
                    {
                        return 1;
                    }
                }
            }
            else
            {
                if ((await GetCurrentThickness(_communicationProtocol, token)) == 0)
                {
                    return 0;
                }
                else
                {
                    await _communicationProtocol.ThicknessAdjustable(false);
                    if ((await GetCurrentThickness(_communicationProtocol, token)) == 0)
                    {
                        return 0;
                    }
                    await _communicationProtocol.ThicknessAdjustable(true);
                    if ((await GetCurrentThickness(_communicationProtocol, token)) == 0)
                    {
                        return 0;
                    }
                }

            }
            return -1;
        }

        public async Task<bool> TestKindVolateOrHgq(bool TestKind)
        {
            var rec = new byte[2] { 0xfd, 0xfd };
            byte[] comman = new byte[3];
            int recnum = 0;
            if (TestKind)
            {
                comman = new byte[] { 0xc1, 0x01, 0xc2 };
                await Task.Run(() =>
                {
                    recnum = Comport.Serial.upserialport.SendCommand(comman, ref rec, 100);
                });
                if (recnum == rec.Length && rec[0] == 0xac)
                {
                    return true;
                }
            }
            else
            {
                comman = new byte[] { 0xc1, 0x00, 0xc1 };
                await Task.Run(() =>
                {
                    recnum = Comport.Serial.upserialport.SendCommand(comman, ref rec, 100);
                });
                if (recnum == rec.Length && rec[0] == 0xac)
                {
                    return true;
                }
            }
            return false;
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
