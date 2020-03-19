using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PortableEquipment.Servers.Xmldata.Xmlconfig;

namespace PortableEquipment.Servers.Xmldata
{
    public interface IXmlconfig
    {
        void UpdataAllAddNodeConfigXml(string ConnenctionString, string strKey);
        string GetAddNodeValue(string key);
        XmlConfigAdd GetAllXmlItem();
    }
}
