using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortableEquipment.Models
{
    public class Publisher
    {
        private IEventAggregator eventAggregator;
        public Publisher(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
        }

        public void PublishEvent(string a)
        {
            this.eventAggregator.Publish(a);
        }

        public void PublishEvent(int a)
        {
            this.eventAggregator.Publish(a);
        }
    }


}
