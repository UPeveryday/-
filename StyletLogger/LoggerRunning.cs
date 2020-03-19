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
            string filename = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "Logger\\" + System.DateTime.Now.ToString("yyyy-MM-dd");
            if (File.Exists(filename))
                _iwrite.WriteFile(filename, DateTime.Now.ToString() + ":" + conternt + "\n");
            else
            {
                _iwrite.DeelDirectoryInfo(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "Logger\\", Mode.Create);
                _iwrite.FileBaseDeel(filename, MyFileMode.Create);
                _iwrite.WriteFile(filename, conternt);
            }
        }


        public static void WriterByStatic(string conternt)
        {
            string filename = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "Logger\\" + System.DateTime.Now.ToString("yyyy-MM-dd");
            if (File.Exists(filename))
                new WriteDataToFile().WriteFile(filename, DateTime.Now.ToString() + ":" + conternt + "\n");
            else
            {
                new WriteDataToFile().DeelDirectoryInfo(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "Logger\\", Mode.Create);
                new WriteDataToFile().FileBaseDeel(filename, MyFileMode.Create);
                new WriteDataToFile().WriteFile(filename, conternt);
            }
        }
        public static void WriterByStatic(byte[] conternt, bool SendOrRec)
        {
            string flag = string.Empty;
            if (SendOrRec)
                flag = "发送数据  ：";
            else
                flag = "接受数据  ：";
            string data = flag + string.Join(" ", conternt);
            string filename = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "Logger\\" + System.DateTime.Now.ToString("yyyy-MM-dd");
            if (File.Exists(filename))
                new WriteDataToFile().WriteFile(filename, DateTime.Now.ToString() + ":" + data + "\n");
            else
            {
                new WriteDataToFile().DeelDirectoryInfo(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "Logger\\", Mode.Create);
                new WriteDataToFile().FileBaseDeel(filename, MyFileMode.Create);
                new WriteDataToFile().WriteFile(filename, data);
            }
        }

        public void Writer(byte[] conternt)
        {
            string data = string.Join(" ", conternt);
            string filename = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "Logger\\" + System.DateTime.Now.ToString("yyyy-MM-dd");
            if (File.Exists(filename))
                _iwrite.WriteFile(filename, DateTime.Now.ToString() + ":" + data + "\n");
            else
            {
                _iwrite.DeelDirectoryInfo(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "Logger\\", Mode.Create);
                _iwrite.FileBaseDeel(filename, MyFileMode.Create);
                _iwrite.WriteFile(filename, data);
            }
        }
    }
}
