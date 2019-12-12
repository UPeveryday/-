using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stylet;

namespace PortableEquipment.Models
{
    public static class StaticClass
    {
        public static IEventAggregator _eventAggregator;
        public static Publisher _publisher ;
        public static Subscriber _subscriber ;
    }
}
