using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortableEquipment.Servers.CHangeVolate
{
    public interface ISetVolate
    {
        Task SettindVolate(double voltage, Servers.CommunicationProtocol.ICommunicationProtocol _communicationProtocol, Xmldata.IXmlconfig _xmlconfig);
    }
}
