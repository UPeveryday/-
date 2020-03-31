using PortableEquipment.TestParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortableEquipment.Servers.SqlDeel
{
    public interface ISqlHelp
    {
        bool SaveTransformerDataBase(Translator testmessage);
        bool SaveMutualTranslatorTransformerDataBase(MutualTranslator testmessage);
    }
}
