using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using StyletIoC;

namespace PortableEquipment.Servers
{
    public class EntityModelServer : IEntityServer
    {
        public jsEntities entitiesmodel
        {
            get => new jsEntities(); set { }
        }
    }
}
