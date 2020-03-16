using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace PortableEquipment.ViewModels
{
    public class TimeViewModel : Screen
    {
        System.Timers.Timer timer = new System.Timers.Timer();

        public TimeViewModel()
        {
            timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
            timer.Enabled = true;
        }

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Hour = DateTime.Now.Hour;
            Mins = DateTime.Now.Minute;
            Seconed = DateTime.Now.Second;

        }

        public int Hour
        {
            get; set;
        }
        public int Mins
        {
            get; set;
        }
        public int Seconed
        {
            get; set;
        }


    }
}
