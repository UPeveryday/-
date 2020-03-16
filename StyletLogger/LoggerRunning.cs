using StyletIoC;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortableEquipment.StyletLogger
{
    public class LoggerRunning : ILogger
    {
        [Inject]
        public IWriteFile _iwrite;
        public void Writer(string conternt)
        {
            string filename = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "Logger\\" + System.DateTime.Now.ToString(" yyyy-MM-dd");
            if (File.Exists(filename))
                _iwrite.WriteFile(filename, DateTime.Now.ToString() + ":" + conternt + "\n");
            else
            {
                _iwrite.DeelDirectoryInfo(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "Logger\\", Mode.Create);
                _iwrite.FileBaseDeel(filename, MyFileMode.Create);
                _iwrite.WriteFile(filename, conternt);
            }
        }
    }
}
