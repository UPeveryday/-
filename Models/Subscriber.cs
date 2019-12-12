using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortableEquipment.Models
{
    public class Subscriber : IHandle<string>, IHandle<int>
    {
        public Subscriber(IEventAggregator eventAggregator)
        {
            eventAggregator.Subscribe(this);//订阅string
        }
        public void Handle(string data)
        {
        }

        public void Handle(int message)
        {
        }
    }
   
}
