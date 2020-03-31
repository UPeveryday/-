using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortableEquipment.Servers.Json
{
    public class JsondeelServers : IJsondeel
    {
        public string GetJsonByclass<T>(T t)
        {
            if (t == null)
                return string.Empty;
            JsonSerializerSettings jsetting = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Include,
                StringEscapeHandling = StringEscapeHandling.EscapeNonAscii,
            };
            return JsonConvert.SerializeObject(t, Formatting.Indented, jsetting);
        }

        public T GetClassFromStr<T>(string jsonstr)
        {
            return JsonConvert.DeserializeObject<T>(jsonstr);
        }
    }
}
