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
        public static Publisher _publisher;
        public static Subscriber _subscriber;

        public static bool Checkdata(byte[] data)
        {
            byte sum = 0;
            for (int i = 0; i < data.Length - 1; i++)
            {
                sum += data[i];
            }
            return sum == data[data.Length - 1];
        }

        public static string GetPhame(int num)
        {
            if (num == 1)
                return "U端";
            if (num == 2)
                return "V端";
            if (num == 3)
                return "W端";
            return string.Empty;
        }


    }
}
