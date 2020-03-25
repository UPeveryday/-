using Stylet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Controls;

namespace PortableEquipment.ViewModels
{
    public class TimeViewModel : Screen
    {
        public TimeViewModel()
        {
          
        }
        public ObservableCollection<string> VolataGroup { get; set; } = new ObservableCollection<string>();
        public string Conetnt { get; set; }
    }
}
