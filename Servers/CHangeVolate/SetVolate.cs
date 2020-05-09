using PortableEquipment.Servers.CommunicationProtocol;
using StyletIoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PortableEquipment.Servers.CHangeVolate
{
    public class SetVolate : ISetVolate
    {

        //private async Task<TestKind> GetUpOrdownAsync(double volate, ICommunicationProtocol _communicationProtocol)
        //{
        //    if (volate - (await _communicationProtocol.ReadStataThree()).AVolate > 0)
        //        return TestKind.ControlsVolateUP;
        //    else
        //        return TestKind.ControlsVolateDown;
        //}
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
        public async Task<bool> SettindVolate(double voltage, ICommunicationProtocol _communicationProtocol, Xmldata.IXmlconfig _xmlconfig, CancellationToken token, int TimeOver = 5)
        {
            if (voltage < 0.5)
            {
                await DownVolateZero(_communicationProtocol, _xmlconfig, token);
                return true;
            }
            await ControlsPowerStata(true, _communicationProtocol, token);
            await Task.Delay(100, token); int i = 0;
            double needdouble = Convert.ToDouble(_xmlconfig.GetAddNodeValue("UpvolateNeeddouble"));
            await _communicationProtocol.ThicknessAdjustable(true);
        heres: while (true)
            {
                TestKind ts;
            ReReadPower: var data = await _communicationProtocol.ReadStataThree(token);
                token.ThrowIfCancellationRequested();
                if (data.Checked)
                {
                    ts = (voltage - data.AVolate) > 0 ? TestKind.ControlsVolateUP : TestKind.ControlsVolateDown;
                    if (Math.Abs(voltage - data.AVolate) >= 16)
                        await _communicationProtocol.SetTestPra(ts, 1, token);
                    else
                        break;
                }
                else
                    goto ReReadPower;

            }
            await _communicationProtocol.ThicknessAdjustable(false);
            while (true)
            {
                TestKind ts;
            ReReadPowerTwo: var data = await _communicationProtocol.ReadStataThree(token);
                token.ThrowIfCancellationRequested();

                if (data.Checked)
                {
                    ts = (voltage - data.AVolate) > 0 ? TestKind.ControlsVolateUP : TestKind.ControlsVolateDown;
                    if (Math.Abs(voltage - data.AVolate) < 16)
                    {
                        if (Math.Abs(voltage - data.AVolate) / voltage > needdouble)
                        {
                            await _communicationProtocol.SetTestPra(ts, 1, token);
                        }
                        else
                            break;
                    }
                    else
                        break;
                }
                else
                    goto ReReadPowerTwo;
            }

            if (Math.Abs(voltage - (await _communicationProtocol.ReadStataThree(token)).AVolate) / voltage > needdouble)
            {
                token.ThrowIfCancellationRequested();

                if (++i > TimeOver)
                    return false;
                goto heres;
            }
            return true;
        }
        public async Task DownVolateZero(ICommunicationProtocol _communicationProtocol, Xmldata.IXmlconfig _xmlconfig, CancellationToken token)
        {
            await ControlsPowerStata(true, _communicationProtocol, token);
            await _communicationProtocol.ThicknessAdjustable(true);
            await _communicationProtocol.SetTestPra(TestKind.ControlsVolateDown, 5, token);
            while (true)
            {
            newst: var data = await _communicationProtocol.ReadStataThree(token);
                token.ThrowIfCancellationRequested();
                if (data.Checked)
                {
                    if (data.AVolate > 5)
                        await _communicationProtocol.SetTestPra(TestKind.ControlsVolateDown, 2, token);
                    else
                    {
                        break;
                    }

                }
                else
                    goto newst;
            }

            //byte num = (byte)((await _communicationProtocol.GetCgfVolateDouble()) * 1000 / Convert.ToDouble(_xmlconfig.GetAddNodeValue("Abs")) + 2);
            //await _communicationProtocol.SetTestPra(TestKind.ControlsVolateDown, num);
        }

        public async Task<bool> SettindHighVolate(double voltage, ICommunicationProtocol _communicationProtocol, Xmldata.IXmlconfig _xmlconfig, CancellationToken token, int TimeOver = 5)
        {
            if (voltage < 0.5)
            {
                await DownVolateZero(_communicationProtocol, _xmlconfig, token);
                return true;
            }
            await ControlsPowerStata(true, _communicationProtocol, token);
            await Task.Delay(100); int i = 0;
            double needdouble = Convert.ToDouble(_xmlconfig.GetAddNodeValue("UpvolateNeedHighdouble"));
            double Abs = Convert.ToDouble(_xmlconfig.GetAddNodeValue("Abs"));
            await _communicationProtocol.ThicknessAdjustable(true);
        herel: while (Math.Abs(voltage - (await _communicationProtocol.GetCgfVolateDouble())) >= 1)
            {
                token.ThrowIfCancellationRequested();

                var needchangecgf = voltage - await _communicationProtocol.GetCgfVolateDouble();
                if (needchangecgf > 0)
                {
                    await _communicationProtocol.SetTestPra(TestKind.ControlsVolateUP, 1, token);
                }
                else
                {
                    await _communicationProtocol.SetTestPra(TestKind.ControlsVolateDown, 1, token);
                }
            }
            await _communicationProtocol.ThicknessAdjustable(false);
            while (Math.Abs(voltage - (await _communicationProtocol.GetCgfVolateDouble())) < 1)
            {
                token.ThrowIfCancellationRequested();

                if (Math.Abs(voltage - (await _communicationProtocol.GetCgfVolateDouble())) / voltage > needdouble)
                {
                    var needchangecgf = voltage - await _communicationProtocol.GetCgfVolateDouble();
                    if (needchangecgf > 0)
                    {
                        await _communicationProtocol.SetTestPra(TestKind.ControlsVolateUP, 1, token);
                    }
                    else
                    {
                        await _communicationProtocol.SetTestPra(TestKind.ControlsVolateDown, 1, token);
                    }
                }
                else
                    break;
            }
            if (Math.Abs(voltage - (await _communicationProtocol.GetCgfVolateDouble())) / voltage > needdouble)
            {
                token.ThrowIfCancellationRequested();

                if (++i > TimeOver)
                    return false;
                goto herel;
            }
            else
                return true;
        }


        public async Task<bool> SetVolatedata(double voltage, ICommunicationProtocol _communicationProtocol, Xmldata.IXmlconfig _xmlconfig, CancellationToken token, int TimeOver = 5)
        {
            // await _communicationProtocol.SwitchThincness(true, _communicationProtocol);
            double needdouble = Convert.ToDouble(_xmlconfig.GetAddNodeValue("UpvolateNeeddouble"));
            await _communicationProtocol.ThicknessAdjustable(true);
            await Task.Delay(100); int i = 0;
        herep: while (Math.Abs(voltage - (await _communicationProtocol.ReadStataThree(token)).AVolate) >= 8)
            {
                token.ThrowIfCancellationRequested();

                double currentvolate = (await _communicationProtocol.ReadStataThree(token)).AVolate;
                double ClickTime = (voltage - currentvolate) / 8;
                if (ClickTime >= 1)
                {
                    await _communicationProtocol.SetTestPra(TestKind.ControlsVolateUP, (byte)ClickTime, token);

                }
                if (ClickTime <= -1)
                {
                    var tf = await _communicationProtocol.SetTestPra(TestKind.ControlsVolateDown, (byte)Math.Abs(ClickTime), token);
                }

            }
            //await _communicationProtocol.SwitchThincness(false, _communicationProtocol);
            await _communicationProtocol.ThicknessAdjustable(false);
            while (Math.Abs(voltage - (await _communicationProtocol.ReadStataThree(token)).AVolate) < 8)
            {
                token.ThrowIfCancellationRequested();

                if (Math.Abs(voltage - (await _communicationProtocol.ReadStataThree(token)).AVolate) / voltage > needdouble)
                {
                    double ClickTime = (voltage - (await _communicationProtocol.ReadStataThree(token)).AVolate) / 1;
                    if (ClickTime >= 1)
                    {
                        await _communicationProtocol.SetTestPra(TestKind.ControlsVolateUP, (byte)ClickTime, token);

                    }
                    if (ClickTime <= -1)
                    {
                        await _communicationProtocol.SetTestPra(TestKind.ControlsVolateDown, (byte)Math.Abs(ClickTime), token);

                    }
                }
                else
                    break;
            }

            if (Math.Abs(voltage - (await _communicationProtocol.ReadStataThree(token)).AVolate) / voltage > needdouble)
            {
                token.ThrowIfCancellationRequested();

                if (++i > TimeOver)
                    return false;
                goto herep;
            }
            return true;

        }
        public async Task<bool> SettindHighVolateByLow(double voltage, ICommunicationProtocol _communicationProtocol, Xmldata.IXmlconfig _xmlconfig, CancellationToken token, int TimeOver = 5)
        {
            double lowvolate = voltage * 1000 / Convert.ToDouble(_xmlconfig.GetAddNodeValue("Abs"));
            return await SettindVolate(lowvolate, _communicationProtocol, _xmlconfig, token);
        }
        public async Task<bool> SettingFre(double Fre, ICommunicationProtocol _communicationProtocol, CancellationToken token, int TimeOver = 5)
        {
            await ControlsPowerStata(true, _communicationProtocol, token);
            await Task.Delay(100); int i = 0;
            await _communicationProtocol.ThicknessAdjustable(true);
            while ((await _communicationProtocol.ReadStataThree(token)).AVolate < 10)
            {
                await _communicationProtocol.SetTestPra(TestKind.ControlsVolateUP, 1, token);
                await Task.Delay(30);
            }
        Heref: while (Math.Abs(Fre - (await _communicationProtocol.ReadStataThree(token)).Fre) >= 1)
            {

            p1: var tpd = await _communicationProtocol.ReadStataThree(token);
                token.ThrowIfCancellationRequested();

                if (!tpd.Checked)
                    goto p1;
                var range = Fre - tpd.Fre;
                var data = (byte)(Math.Abs((int)(Math.Round(range, 1))));
                if (range > 0)
                    await _communicationProtocol.SetTestPra(TestKind.ControlsFreUp, data, token);
                else
                    await _communicationProtocol.SetTestPra(TestKind.ControlsFreDown, data, token);
                await Task.Delay(100, token);
            }
            await _communicationProtocol.ThicknessAdjustable(false);
            while (Math.Abs(Fre - (await _communicationProtocol.ReadStataThree(token)).Fre) < 1)
            {
            p2: var tpd = await _communicationProtocol.ReadStataThree(token);
                token.ThrowIfCancellationRequested();

                if (!tpd.Checked)
                    goto p2;
                var range = Fre - tpd.Fre;
                var data = (byte)(Math.Abs((int)Math.Round((range * 10))));
                if (range > 0)
                    await _communicationProtocol.SetTestPra(TestKind.ControlsFreUp, data, token);
                else if (range < 0)
                    await _communicationProtocol.SetTestPra(TestKind.ControlsFreDown, data, token);
                else
                    return true;
                await Task.Delay(100, token);
            }
            if (Math.Abs(Fre - (await _communicationProtocol.ReadStataThree(token)).Fre) == 0)
            {
                return true;
            }
            else
            {
                token.ThrowIfCancellationRequested();

                if (++i > TimeOver)
                    return false;
                goto Heref;
            }
        }

        public async Task<bool> ControlsPowerStata(bool Open, ICommunicationProtocol _communicationProtocol, CancellationToken token)
        {
            if (Open)
            {
                if (await _communicationProtocol.GetPowerStata())
                {
                    return true;
                }
                else
                {
                    await _communicationProtocol.SetTestPra(TestKind.Start, 2, token);
                    await Task.Delay(7000, token);
                    return await _communicationProtocol.GetPowerStata();
                }
            }
            else
            {
                if (await _communicationProtocol.GetPowerStata())
                {
                    return await _communicationProtocol.SetTestPra(TestKind.Stop, 2, token);
                }
                else
                {
                    return true;
                }
            }
        }

        public async Task DownAndClosePower(ICommunicationProtocol _communicationProtocolk, Xmldata.IXmlconfig _xmlconfig, CancellationToken token)
        {
            if (await _communicationProtocolk.GetPowerStata() && (await _communicationProtocolk.ReadStataThree(token)).AVolate > 10)
            {
                await DownVolateZero(_communicationProtocolk, _xmlconfig, token);
                await ControlsPowerStata(false, _communicationProtocolk, token);
            }
            else
            {
                await ControlsPowerStata(false, _communicationProtocolk, token);
            }
            await ControlsPowerStata(false, _communicationProtocolk, token);

        }

    }
}
