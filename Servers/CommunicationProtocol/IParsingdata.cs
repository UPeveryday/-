using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortableEquipment.Servers.CommunicationProtocol
{
    public interface IParsingdata
    {
        StataTwo ParsingTwo(byte[] data);
        StataThree ParsingThree(byte[] data);
        Methonstata Getstata(byte stata);
    }
}
