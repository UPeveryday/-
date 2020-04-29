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
            if (voltage < 0.5)
            {
                await DownVolateZero(_communicationProtocol, _xmlconfig);
                return true;
            }
            await ControlsPowerStata(true, _communicationProtocol);
            await Task.Delay(100); int i = 0;
            double needdouble = Convert.ToDouble(_xmlconfig.GetAddNodeValue("UpvolateNeeddouble"));
            await _communicationProtocol.ThicknessAdjustable(true);
        here: while (Math.Abs(voltage - (await _communicationProtocol.ReadStataThree()).AVolate) >= 16)
            {
                await _communicationProtocol.SetTestPra(await GetUpOrdownAsync(voltage, _communicationProtocol), 1);
            }
            await _communicationProtocol.ThicknessAdjustable(false);
            while (Math.Abs(voltage - (await _communicationProtocol.ReadStataThree()).AVolate) < 16)
            {
                if (Math.Abs(voltage - (await _communicationProtocol.ReadStataThree()).AVolate) / voltage > needdouble)
                {
                    await _communicationProtocol.SetTestPra(await GetUpOrdownAsync(voltage, _communicationProtocol), 1);
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
        public async Task DownVolateZero(ICommunicationProtocol _communicationProtocol, Xmldata.IXmlconfig _xmlconfig)
        {
            await ControlsPowerStata(true, _communicationProtocol);
            await _communicationProtocol.ThicknessAdjustable(true);
            await _communicationProtocol.SetTestPra(TestKind.ControlsVolateDown, 10);
            while ((await _communicationProtocol.ReadStataThree()).AVolate > 5)
            {
                await _communicationProtocol.SetTestPra(TestKind.ControlsVolateDown, 2);
            }
            //byte num = (byte)((await _communicationProtocol.GetCgfVolateDouble()) * 1000 / Convert.ToDouble(_xmlconfig.GetAddNodeValue("Abs")) + 2);
            //await _communicationProtocol.SetTestPra(TestKind.ControlsVolateDown, num);
        }

        public async Task<bool> SettindHighVolate(double voltage, ICommunicationProtocol _communicationProtocol, Xmldata.IXmlconfig _xmlconfig, int TimeOver = 5)
        {
            if (voltage < 0.5)
            {
                await DownVolateZero(_communicationProtocol, _xmlconfig);
                return true;
            }
            await ControlsPowerStata(true, _communicationProtocol);
            await Task.Delay(100); int i = 0;
            double needdouble = Convert.ToDouble(_xmlconfig.GetAddNodeValue("UpvolateNeedHighdouble"));
            double Abs = Convert.ToDouble(_xmlconfig.GetAddNodeValue("Abs"));
            await _communicationProtocol.ThicknessAdjustable(true);
        here: while (Math.Abs(voltage - (await _communicationProtocol.GetCgfVolateDouble())) >= 2)
            {
                var needchangecgf = voltage - await _communicationProtocol.GetCgfVolateDouble();
                if (needchangecgf > 0)
                {
                    // byte ClickNum = (byte)(needchangecgf * 1000 / Abs / 8);
                    await _communicationProtocol.SetTestPra(TestKind.ControlsVolateUP, 1);
                }
                else
                {
                    //byte ClickNum = (byte)(Math.Abs(needchangecgf) * 1000 / Abs / 8);
                    await _communicationProtocol.SetTestPra(TestKind.ControlsVolateDown, 1);
                }
            }
            await _communicationProtocol.ThicknessAdjustable(false);
            while (Math.Abs(voltage - (await _communicationProtocol.GetCgfVolateDouble())) < 2)
            {
                if (Math.Abs(voltage - (await _communicationProtocol.GetCgfVolateDouble())) / voltage > needdouble)
                {
                    var needchangecgf = voltage - await _communicationProtocol.GetCgfVolateDouble();
                    if (needchangecgf > 0)
                    {
                        // byte ClickNum = (byte)(needchangecgf * 1000 / Abs / 1);
                        await _communicationProtocol.SetTestPra(TestKind.ControlsVolateUP, 1);
                    }
                    else
                    {
                        //byte ClickNum = (byte)(Math.Abs(needchangecgf) * 1000 / Abs / 1);
                        await _communicationProtocol.SetTestPra(TestKind.ControlsVolateDown, 1);
                    }
                }
                else
                    break;
            }
            if (Math.Abs(voltage - (await _communicationProtocol.GetCgfVolateDouble())) / voltage > needdouble)
            {
                if (++i > TimeOver)
                    return false;
                goto here;
            }
            else
                return true;
        }


        public async Task<bool> SetVolatedata(double voltage, ICommunicationProtocol _communicationProtocol, Xmldata.IXmlconfig _xmlconfig, int TimeOver = 5)
        {
            // await _communicationProtocol.SwitchThincness(true, _communicationProtocol);
            double needdouble = Convert.ToDouble(_xmlconfig.GetAddNodeValue("UpvolateNeeddouble"));
            await _communicationProtocol.ThicknessAdjustable(true);
            await Task.Delay(500); int i = 0;
        here: while (Math.Abs(voltage - (await _communicationProtocol.ReadStataThree()).AVolate) >= 8)
            {
                double currentvolate = (await _communicationProtocol.ReadStataThree()).AVolate;
                double ClickTime = (voltage - currentvolate) / 8;
                if (ClickTime >= 1)
                {
                    await _communicationProtocol.SetTestPra(TestKind.ControlsVolateUP, (byte)ClickTime);

                }
                if (ClickTime <= -1)
                {
                    var tf = await _communicationProtocol.SetTestPra(TestKind.ControlsVolateDown, (byte)Math.Abs(ClickTime));
                }

            }
            //await _communicationProtocol.SwitchThincness(false, _communicationProtocol);
            await _communicationProtocol.ThicknessAdjustable(false);
            while (Math.Abs(voltage - (await _communicationProtocol.ReadStataThree()).AVolate) < 8)
            {
                if (Math.Abs(voltage - (await _communicationProtocol.ReadStataThree()).AVolate) / voltage > needdouble)
                {
                    double ClickTime = (voltage - (await _communicationProtocol.ReadStataThree()).AVolate) / 1;
                    if (ClickTime >= 1)
                    {
                        await _communicationProtocol.SetTestPra(TestKind.ControlsVolateUP, (byte)ClickTime);

                    }
                    if (ClickTime <= -1)
                    {
                        await _communicationProtocol.SetTestPra(TestKind.ControlsVolateDown, (byte)Math.Abs(ClickTime));

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
        public async Task<bool> SettindHighVolateByLow(double voltage, ICommunicationProtocol _communicationProtocol, Xmldata.IXmlconfig _xmlconfig, int TimeOver = 5)
        {
            double lowvolate = voltage * 1000 / Convert.ToDouble(_xmlconfig.GetAddNodeValue("Abs"));
            return await SettindVolate(lowvolate, _communicationProtocol, _xmlconfig);
        }
        public async Task<bool> SettingFre(double Fre, ICommunicationProtocol _communicationProtocol, int TimeOver = 5)
        {
            await ControlsPowerStata(true, _communicationProtocol);
            await Task.Delay(100); int i = 0;
            await _communicationProtocol.ThicknessAdjustable(true);
        Here: while (Math.Abs(Fre - (await _communicationProtocol.ReadStataThree()).Fre) >= 1)
            {
            p1: var tpd = await _communicationProtocol.ReadStataThree();
                if (!tpd.Checked)
                    goto p1;
                var range = Fre - tpd.Fre;
                var data = (byte)(Math.Abs((int)(Math.Round(range, 1))));
                if (range > 0)
                    await _communicationProtocol.SetTestPra(TestKind.ControlsFreUp, data);
                else
                    await _communicationProtocol.SetTestPra(TestKind.ControlsFreDown, data);
                await Task.Delay(1000);
            }
            await _communicationProtocol.ThicknessAdjustable(false);
            while (Math.Abs(Fre - (await _communicationProtocol.ReadStataThree()).Fre) < 1)
            {
            p2: var tpd = await _communicationProtocol.ReadStataThree();
                if (!tpd.Checked)
                    goto p2;
                var range = Fre - tpd.Fre;
                var data = (byte)(Math.Abs((int)Math.Round((range * 10))));
                if (range > 0)
                    await _communicationProtocol.SetTestPra(TestKind.ControlsFreUp, data);
                else if (range < 0)
                    await _communicationProtocol.SetTestPra(TestKind.ControlsFreDown, data);
                else
                    return true;
                await Task.Delay(1000);
            }
            if (Math.Abs(Fre - (await _communicationProtocol.ReadStataThree()).Fre) == 0)
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

        public async Task DownAndClosePower(ICommunicationProtocol _communicationProtocolk, Xmldata.IXmlconfig _xmlconfig)
        {
            if ((await _communicationProtocolk.ReadStataThree()).AVolate > 10 && await _communicationProtocolk.GetPowerStata())
            {
                await DownVolateZero(_communicationProtocolk, _xmlconfig);
                await ControlsPowerStata(false, _communicationProtocolk);
            }
            else
            {
                await ControlsPowerStata(false, _communicationProtocolk);
            }
            await ControlsPowerStata(false, _communicationProtocolk);

        }

    }
}
