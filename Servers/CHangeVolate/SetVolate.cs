using PortableEquipment.Servers.CommunicationProtocol;
using StyletIoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortableEquipment.Servers.CHangeVolate
{
    public class SetVolate : ISetVolate
    {

        private async Task<TestKind> GetUpOrdownAsync(double volate, ICommunicationProtocol _communicationProtocol)
        {
            if (volate - (await _communicationProtocol.ReadStataThree()).AVolate > 0)
                return TestKind.ControlsVolateUP;
            else
                return TestKind.ControlsVolateDown;
        }
        private async Task<TestKind> GetUpOrdownHighAsync(double volate, ICommunicationProtocol _communicationProtocol)
        {
            if (volate - (await _communicationProtocol.GetCgfVolateDouble()) > 0)
                return TestKind.ControlsVolateUP;
            else
                return TestKind.ControlsVolateDown;
        }
        private byte GetTpd(double tpd)
        {
            if (tpd >= 1 && tpd < 2)
            {
                return 1;
            }
            else
                return (byte)((int)(tpd * 0.5));
        }
        public async Task<bool> SettindVolate(double voltage, ICommunicationProtocol _communicationProtocol, Xmldata.IXmlconfig _xmlconfig, int TimeOver = 5)
        {
            await ControlsPowerStata(true, _communicationProtocol);
            await Task.Delay(100); int i = 0;
            double needdouble = Convert.ToDouble(_xmlconfig.GetAddNodeValue("UpvolateNeeddouble"));
        here: while (Math.Abs(voltage - (await _communicationProtocol.ReadStataThree()).AVolate) >= 10)
            {
                if (await _communicationProtocol.ThicknessAdjustable(true))
                {
                    await _communicationProtocol.SetTestPra(await GetUpOrdownAsync(voltage, _communicationProtocol), 1);
                }
            }

            while (Math.Abs(voltage - (await _communicationProtocol.ReadStataThree()).AVolate) < 10)
            {
                if (Math.Abs(voltage - (await _communicationProtocol.ReadStataThree()).AVolate) / voltage > needdouble)
                {
                    if (await _communicationProtocol.ThicknessAdjustable(false))
                    {
                        await _communicationProtocol.SetTestPra(await GetUpOrdownAsync(voltage, _communicationProtocol), 1);
                    }
                }
                else
                    break;
            }
            if (Math.Abs(voltage - (await _communicationProtocol.ReadStataThree()).AVolate) / voltage > needdouble)
            {
                if (++i > TimeOver)
                    return false;
                goto here;
            }
            return true;
        }


        public async Task<bool> SettindHighVolate(double voltage, ICommunicationProtocol _communicationProtocol, Xmldata.IXmlconfig _xmlconfig, int TimeOver = 5)
        {
            if (voltage == 0) voltage = 0.1;
            await ControlsPowerStata(true, _communicationProtocol);
            await Task.Delay(100); int i = 0;
            double needdouble = Convert.ToDouble(_xmlconfig.GetAddNodeValue("UpvolateNeedHighdouble"));
        here: while (Math.Abs(voltage - (await _communicationProtocol.GetCgfVolateDouble())) >= 0.5)
            {
                if (await _communicationProtocol.ThicknessAdjustable(true))
                {
                    var p = await GetUpOrdownHighAsync(voltage, _communicationProtocol);
                    await _communicationProtocol.SetTestPra(p, 1);
                }
            }

            while (Math.Abs(voltage - (await _communicationProtocol.GetCgfVolateDouble())) < 0.5)
            {
                if (Math.Abs(voltage - (await _communicationProtocol.GetCgfVolateDouble())) / voltage > needdouble)
                {
                    if (await _communicationProtocol.ThicknessAdjustable(false))
                    {
                        await _communicationProtocol.SetTestPra(await GetUpOrdownHighAsync(voltage, _communicationProtocol), 1);
                    }
                }
                else
                    break;
            }
            if (Math.Abs(voltage - (await _communicationProtocol.GetCgfVolateDouble())) > 0.1 || Math.Abs(voltage - (await _communicationProtocol.GetCgfVolateDouble())) / voltage > needdouble)
            {
                if (++i > TimeOver)
                    return false;
                goto here;
            }
            else
                return true;
        }




        public async Task<bool> SettingFre(double Fre, ICommunicationProtocol _communicationProtocol, int TimeOver = 5)
        {
            await ControlsPowerStata(true, _communicationProtocol);
            await Task.Delay(100); int i = 0;
        Here: while (Math.Abs(Fre - (await _communicationProtocol.ReadStataThree()).Fre) >= 1)
            {
                if (await _communicationProtocol.ThicknessAdjustable(true))
                {
                    var range = Fre - (await _communicationProtocol.ReadStataThree()).Fre;
                    if (range > 0)
                        await _communicationProtocol.SetTestPra(TestKind.ControlsFreUp, (byte)Math.Abs(range));
                    else
                        await _communicationProtocol.SetTestPra(TestKind.ControlsFreDown, (byte)Math.Abs(range));
                }
            }
            while (Math.Abs(Fre - (await _communicationProtocol.ReadStataThree()).Fre) > 0.2)
            {
                if (await _communicationProtocol.ThicknessAdjustable(false))
                {
                    var range = Fre - (await _communicationProtocol.ReadStataThree()).Fre;
                    if (range > 0.1)
                        await _communicationProtocol.SetTestPra(TestKind.ControlsFreUp, (byte)(Math.Abs(range) * 10));
                    else if (range < -0.1)
                        await _communicationProtocol.SetTestPra(TestKind.ControlsFreDown, (byte)(Math.Abs(range) * 10));
                    else
                        return true;

                }
            }
            if (Math.Abs(Fre - (await _communicationProtocol.ReadStataThree()).Fre) < 0.2)
            {
                return true;
            }
            else
            {
                if (++i > TimeOver)
                    return false;
                goto Here;
            }
        }

        public async Task<bool> ControlsPowerStata(bool Open, ICommunicationProtocol _communicationProtocol)
        {
            if (Open)
            {
                if (await _communicationProtocol.GetPowerStata())
                {
                    return true;
                }
                else
                {
                    await _communicationProtocol.SetTestPra(TestKind.Start, 2);
                    await Task.Delay(7000);
                    return await _communicationProtocol.GetPowerStata();
                }
            }
            else
            {
                if (await _communicationProtocol.GetPowerStata())
                {
                    return await _communicationProtocol.SetTestPra(TestKind.Stop, 2);
                }
                else
                {
                    return true;
                }
            }
        }

    }
}
