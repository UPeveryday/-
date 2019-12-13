using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace PortableEquipment.Servers
{
    public interface IEntityServer
    {
        jsEntities entitiesmodel { get; set; }
    }
}
