using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PortableEquipment.StyletLogger;
using Stylet;
using StyletIoC;

namespace PortableEquipment.Servers
{
    public class test: Screen
    {
        [Inject]
        public IWriteFile _WriteFile;
        public test()
        {

        }
        public string read()
        {
            return _WriteFile.ReadFile(@"C:\Users\My\Desktop\1.txt");
        }
    }
}
