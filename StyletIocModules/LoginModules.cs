using PortableEquipment.StyletLogger;
using StyletIoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PortableEquipment.Servers;
using Stylet;

namespace PortableEquipment.StyletIocModules
{
    public class LoginModules : StyletIoCModule
    {
        protected override void Load()
        {
            Bind(typeof(Servers.ILogin<>)).To(typeof(Servers.LoginRespontory<>)).WithKey("Lr");
            Bind(typeof(Servers.ILogin<>)).To(typeof(Servers.LoginSingalRespontitory<>)).WithKey("Lrs");
            //Bind<IWriteFile>().ToFactory(container => new WriteDataToFile());
            //Bind<IWriteFile>().ToInstance(new WriteDataToFile());//绑定新实例到容器
            Bind<IWriteFile>().To<WriteDataToFile>();//.DisposeWithContainer(false);//单个注入,不释放
            Bind<ILogger>().To<LoggerRunning>();//.DisposeWithContainer(false);//单个注入,不释放
            Bind<Servers.IEntityServer>().To<Servers.EntityModelServer>();//.DisposeWithContainer(false);//单个注入,不释放
            // Bind<Servers.IEntityServer>().To<Servers.EntityModelServer>() ;//单个注入
            // Bind<IWriteFile>().To<WriteDataToFile>(); ;//单个注入


            Bind<Servers.CommunicationProtocol.ICommunicationProtocol>().To<Servers.CommunicationProtocol.CommunicationProtocol>();
            Bind<Servers.CommunicationProtocol.IParsingdata>().To<Servers.CommunicationProtocol.Parsingdata>();
            Bind<Servers.Xmldata.IXmlconfig>().To<Servers.Xmldata.Xmlconfig>();
           // Bind<Comport.ISerialCommunication>().To<Comport.Serial>();
        }
    }
}
