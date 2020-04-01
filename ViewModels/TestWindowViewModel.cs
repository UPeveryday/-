using PortableEquipment.TestParameters;
using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortableEquipment.ViewModels
{
    public class TestWindowViewModel : Screen, IHandle<TestParameters.MutualTranslator>
    {
        public void Handle(MutualTranslator message)
        {
            int a = 0;
        }
    }
}
