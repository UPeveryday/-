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
            Bind<IWriteFile>().To<WriteDataToFile>().InSingletonScope(); //.DisposeWithContainer(false);//单个注入,不释放
            Bind<ILogger>().To<LoggerRunning>().InSingletonScope(); //.DisposeWithContainer(false);//单个注入,不释放
            Bind<Servers.IEntityServer>().To<Servers.EntityModelServer>().InSingletonScope(); //.DisposeWithContainer(false);//单个注入,不释放
            // Bind<Servers.IEntityServer>().To<Servers.EntityModelServer>() ;//单个注入
            // Bind<IWriteFile>().To<WriteDataToFile>(); ;//单个注入
            Bind<Servers.CommunicationProtocol.ICommunicationProtocol>().To<Servers.CommunicationProtocol.CommunicationProtocol>().InSingletonScope();
            Bind<Servers.CommunicationProtocol.IParsingdata>().To<Servers.CommunicationProtocol.Parsingdata>().InSingletonScope();
            Bind<Servers.Xmldata.IXmlconfig>().To<Servers.Xmldata.Xmlconfig>();
            Bind<Servers.Json.IJsondeel>().To<Servers.Json.JsondeelServers>().InSingletonScope(); 
            Bind<Servers.CHangeVolate.ISetVolate>().To<Servers.CHangeVolate.SetVolate>().InSingletonScope();
            Bind<Servers.SqlDeel.ISqlHelp>().To<Servers.SqlDeel.SqlHelp>().InSingletonScope();
            Bind<Servers.SelfCheck.ISelfCheck>().To<Servers.SelfCheck.SelfCheck>().InSingletonScope();
            // Bind<Comport.ISerialCommunication>().To<Comport.Serial>();

            Bind<ViewModels.VoltageTestViewModel>().ToSelf().InSingletonScope();
            Bind<ViewModels.TransformerViewModel>().ToSelf().InSingletonScope();
            Bind<ViewModels.ManualVoltageViewModel>().ToSelf().InSingletonScope();
            Bind<ViewModels.MainViewModel>().ToSelf().InSingletonScope();
            Bind<ViewModels.JfViewModel>().ToSelf().InSingletonScope();
            Bind<IMessageBoxViewModel>().To<ViewModels.MessageBoxViewModel>();
        }
    }
}
