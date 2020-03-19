using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortableEquipment.Comport
{
    public interface ISerialCommunication
    {
        SerialClass SerialServers { get; set; }

    }
}
