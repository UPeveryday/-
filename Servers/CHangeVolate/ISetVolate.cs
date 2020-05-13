using PortableEquipment.Servers.CommunicationProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PortableEquipment.Servers.CHangeVolate
{
    public  interface ISetVolate
    {
        Task<bool> SettindVolate(double voltage, ICommunicationProtocol _communicationProtocol, Xmldata.IXmlconfig _xmlconfig, CancellationToken token, int TimeOver = 5);
        Task<bool> SettingFre(double Fre, ICommunicationProtocol _communicationProtocol, CancellationToken token, int TimeOver = 5);
        Task<bool> ControlsPowerStata(bool Open, ICommunicationProtocol _communicationProtocol, CancellationToken token);
        Task<bool> SettindHighVolate(double voltage, ICommunicationProtocol _communicationProtocol, Xmldata.IXmlconfig _xmlconfig, CancellationToken token, int TimeOver = 5);
        Task<bool> SettindHighVolateByLow(double voltage, ICommunicationProtocol _communicationProtocol, Xmldata.IXmlconfig _xmlconfig, CancellationToken token, int TimeOver = 5);
        Task<bool> SetVolatedata(double voltage, ICommunicationProtocol _communicationProtocol, Xmldata.IXmlconfig _xmlconfig, CancellationToken token, int TimeOver = 5);
        Task DownVolateZero(ICommunicationProtocol _communicationProtocol, Xmldata.IXmlconfig _xmlconfig, CancellationToken token);
        Task DownAndClosePower(ICommunicationProtocol _communicationProtocolk, Xmldata.IXmlconfig _xmlconfig, CancellationToken token);
    }
}
