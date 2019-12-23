using Stylet;
using StyletIoC;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortableEquipment.StyletLogger
{
    public class MyLogger :  Stylet.Logging.ILogger
    {
        [Inject]
        public IWriteFile _writeFile;

        private string _LoggerBasePath;

        public object Current => throw new NotImplementedException();

        public void Error(Exception exception, string message = null)
        {
            Loaded();
            _writeFile.WriteFile(_LoggerBasePath, "Error  :" + message + "\t\t\t" + DateTime.Now.ToString() + "\t\t\t" + "\r\n");
        }
        public MyLogger()
        {
            _writeFile = new WriteDataToFile();
        }
        private void Loaded()
        {
            _LoggerBasePath = System.IO.Directory.GetCurrentDirectory().Replace("bin\\Debug", "") + "StyletLogger\\Logger\\" + DateTime.Now.ToString("D");
            if (!File.Exists(_LoggerBasePath))
            {
                _writeFile.FileBaseDeel(_LoggerBasePath, MyFileMode.Create);
            }
        }
        public void Info(string format, params object[] args)
        {
            Loaded();
            _writeFile.WriteFile(_LoggerBasePath, "Info  :" + string.Join("-", args) + "\t\t\t" + DateTime.Now.ToString() + "\t\t\t" + "\r\n");
        }

        public void Warn(string format, params object[] args)
        {
            Loaded();
            _writeFile.WriteFile(_LoggerBasePath, "Warn  :" + string.Join("-", args) + "\t\t\t" + DateTime.Now.ToString() + "\r\n");
        }


    }
}
