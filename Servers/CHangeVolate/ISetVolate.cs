using PortableEquipment.Servers.CommunicationProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortableEquipment.Servers.CHangeVolate
{
    public interface ISetVolate
    {
        Task<bool> SettindVolate(double voltage, ICommunicationProtocol _communicationProtocol, Xmldata.IXmlconfig _xmlconfig, int TimeOver = 5);
        Task<bool> SettingFre(double Fre, ICommunicationProtocol _communicationProtocol, int TimeOver=5);
    }
}
