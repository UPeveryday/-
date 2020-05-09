using StyletIoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PortableEquipment.Servers.SelfCheck
{
    public class SelfCheck : ISelfCheck
    {
        [Inject]
        public Servers.CommunicationProtocol.ICommunicationProtocol _CommunicationProtocol;
        [Inject]
        public Servers.CHangeVolate.ISetVolate _setVolate;
        [Inject]
        public Servers.Xmldata.IXmlconfig _xmlconfig;
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
                ret += "变频电源:" + new Xmldata.Xmlconfig().GetAddNodeValue("upComport") + " 打开失败\t\n";
            }
            checkMesssage.hidemessage = ret;
            return checkMesssage;
        }

        public async Task<bool> SeleCheck(CancellationToken token)
        {
            bool Power = false;
            bool Set = false;
            if ((await ComPortCheck()).IsOk)
            {
                for (int i = 0; i < 10; i++)
                {
                    var p = await _CommunicationProtocol.ReadStataThree(token);
                    if (p.Checked)
                    {
                        Power = true;
                        break;
                    }
                }
                var rec = new byte[1];
                byte[] comman = new byte[3] { 0xa5, 0x0a, 0xaf };
                await Task.Run(() =>
                {
                    int recnum = Comport.Serial.upserialport.SendCommand(comman, ref rec, 100);
                });
                if (rec[0] == 0xAA)
                    Set = true;
                else if (rec[0] == 0xAB)
                    Set = true;
                else
                    Set = false;
            }
            if (Power == Set == true)
                return true;
            return false;
        }

    }

    public struct SelfCheckMesssage
    {
        public string hidemessage;
        public bool IsOk;
    }
}
