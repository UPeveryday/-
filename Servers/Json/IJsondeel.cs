using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortableEquipment.Servers.Json
{
    public interface IJsondeel
    {
        string GetJsonByclass<T>(T t);
        T GetClassFromStr<T>(string jsonstr);
    }
}
