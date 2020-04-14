﻿using PortableEquipment.Servers.CommunicationProtocol;
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
            await Task.Delay(100); int i = 0;
            double needdouble = Convert.ToDouble(_xmlconfig.GetAddNodeValue("UpvolateNeeddouble"));
        here: while (Math.Abs(voltage - (await _communicationProtocol.ReadStataThree()).AVolate) >= 10)
            {
                double tpd = Math.Abs(voltage - (await _communicationProtocol.ReadStataThree()).AVolate) / 10;
                //if (await _communicationProtocol.ThicknessAdjustable(true))
                //{
                while ((int)tpd >= 1)
                {
                    if (await _communicationProtocol.SetTestPra(await GetUpOrdownAsync(voltage, _communicationProtocol), GetTpd(tpd)))
                    {
                        tpd -= tpd * 0.5;
                        await Task.Delay(Convert.ToInt32(_xmlconfig.GetAddNodeValue("SetPraJg")));
                    }
                }
                //}
            }

            while (Math.Abs(voltage - (await _communicationProtocol.ReadStataThree()).AVolate) < 10)
            {
                if (Math.Abs(voltage - (await _communicationProtocol.ReadStataThree()).AVolate) / voltage > needdouble)
                {
                    double tpd = Math.Abs(voltage - (await _communicationProtocol.ReadStataThree()).AVolate) / 1;
                    //if (await _communicationProtocol.ThicknessAdjustable(false))
                    //{
                    while ((int)tpd >= 1)
                    {
                        if (await _communicationProtocol.SetTestPra(await GetUpOrdownAsync(voltage, _communicationProtocol), GetTpd(tpd)))
                        {
                            tpd -= tpd * 0.5;
                            await Task.Delay(Convert.ToInt32(_xmlconfig.GetAddNodeValue("SetPraJg")));
                        }
                    }
                    //}
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


        //public async Task<bool> SettindVolateConfire(double voltage, ICommunicationProtocol _communicationProtocol, Xmldata.IXmlconfig _xmlconfig, int TimeOver = 5)
        //{
        //    double needdouble = Convert.ToDouble(_xmlconfig.GetAddNodeValue("UpvolateNeeddouble"));
        //    while (Math.Abs(voltage - _communicationProtocol.ReadStataThree().AVolate) / voltage > needdouble)
        //    {
        //        while ((voltage - _communicationProtocol.ReadStataThree().AVolate) > 10)
        //        {
        //            double tpd = Math.Abs(voltage - _communicationProtocol.ReadStataThree().AVolate) / 10;
        //            if (_communicationProtocol.ThicknessAdjustable(true))
        //            {

        //            }
        //        }
        //    }
        //}
    }
}
