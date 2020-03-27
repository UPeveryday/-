using PortableEquipment.Servers.CommunicationProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortableEquipment.Servers.CHangeVolate
{
    public class SetVolate : ISetVolate
    {
        public async Task SettindVolate(double voltage, ICommunicationProtocol _communicationProtocol, Xmldata.IXmlconfig _xmlconfig)
        {
            await Task.Delay(500);
            var CurrentVolate = _communicationProtocol.ReadStataThree().AVolate;//当前电压
            var ChnageRange = Math.Abs(voltage - CurrentVolate);//升降压值
            var UpVolateSpeed = Convert.ToDouble(_xmlconfig.GetAddNodeValue("UpVolateSpeed"));//升降压速度
            double tpd = ChnageRange / UpVolateSpeed;//升降压次数

            if (CurrentVolate <= voltage)
            {
                while ((int)tpd > 1)
                {
                    _communicationProtocol.SetTestPra(TestKind.ControlsVolateUP, (byte)(tpd * 0.2));
                    tpd -= tpd * 0.2;
                    await Task.Delay(500);
                }
                if (Math.Abs(voltage - _communicationProtocol.ReadStataThree().AVolate) / voltage > 0.05)
                {
                   await SettindVolate(voltage, _communicationProtocol, _xmlconfig);
                }

            }
            else
            {
                while ((int)tpd > 1)
                {
                    _communicationProtocol.SetTestPra(TestKind.ControlsVolateDown, (byte)(tpd * 0.2));
                    tpd -= tpd * 0.2;
                    await Task.Delay(500);
                }
                if (Math.Abs(voltage - _communicationProtocol.ReadStataThree().AVolate) / voltage > 0.01)
                {
                    await SettindVolate(voltage, _communicationProtocol, _xmlconfig);
                }

            }
        }
    }
}
