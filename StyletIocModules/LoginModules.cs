using StyletIoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortableEquipment.StyletIocModules
{
    public class LoginModules : StyletIoCModule
    {
        protected override void Load()
        {
            Bind(typeof(Servers.ILogin<>)).To(typeof(Servers.LoginRespontory<>)).WithKey("Lr");
            Bind(typeof(Servers.ILogin<>)).To(typeof(Servers.LoginSingalRespontitory<>)).WithKey("Lrs");
            Bind<StyletLogger.IWriteFile>().To<StyletLogger.WriteDataToFile>();
            Bind<Servers.IEntityServer>().To<Servers.EntityModelServer>(); ;//单个注入
        }
    }
}
