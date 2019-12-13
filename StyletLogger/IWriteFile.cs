using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortableEquipment.StyletLogger
{
    public interface IWriteFile
    {
        void DeelDirectoryInfo(string Path, Mode mode);
        void FileBaseDeel(string path, MyFileMode fileMode);
        void WriteFile(string path, string Content);
        void WriteFile(string path, string Content, int position);
        string ReadFile(string path);
    }
}
