using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortableEquipment.Servers.SelfCheck
{
    public class SelfCheck : ISelfCheck
    {
        public async Task<SelfCheckMesssage> ComPortCheck()
        {
            string ret = string.Empty;
            SelfCheckMesssage checkMesssage = new SelfCheckMesssage();
            checkMesssage.IsOk = true;
            if (!await Task.Factory.StartNew(Comport.Serial.Cgfserialport.openPort))
            {
                checkMesssage.IsOk = false;
                ret += "Cgf端口:" + new Xmldata.Xmlconfig().GetAddNodeValue("CgfComport") + " 打开失败\t\n";
            }
            if (!await Task.Factory.StartNew(Comport.Serial.serialport.openPort))
            {
                checkMesssage.IsOk = false;
                ret += "功率表:" + new Xmldata.Xmlconfig().GetAddNodeValue("PowerComport") + " 打开失败\t\n";
            }
            if (!await Task.Factory.StartNew(Comport.Serial.upserialport.openPort))
            {
                checkMesssage.IsOk = false;
                ret += "变配电源:" + new Xmldata.Xmlconfig().GetAddNodeValue("upComport") + " 打开失败\t\n";
            }
            checkMesssage.hidemessage = ret;
            return checkMesssage;
        }
    }

    public struct SelfCheckMesssage
    {
        public string hidemessage;
        public bool IsOk;
    }
}
