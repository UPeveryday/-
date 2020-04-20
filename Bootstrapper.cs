using System;
using Stylet;
using StyletIoC;
using PortableEquipment.Pages;
using System.Collections.Generic;
using Model;
using PortableEquipment.ViewModels;
using PortableEquipment.StyletLogger;

namespace PortableEquipment
{
    public class Bootstrapper : Bootstrapper<ViewModels.MainViewModel>
    {
        protected override void ConfigureIoC(IStyletIoCBuilder builder)
        {
            builder.AddModule(new StyletIocModules.LoginModules());//Login modules
            #region IOC Test
            //builder.Bind(typeof(Servers.ILogin<>)).ToAllImplementations();//注入此接口所有的服务
            //builder.Bind(typeof(Servers.ILogin<>)).To(typeof(Servers.LoginRespontory<>)).WithKey("Lr");
            //builder.Bind(typeof(Servers.ILogin<>)).To(typeof(Servers.LoginSingalRespontitory<>)).WithKey("Lrs");
            //builder.Bind<Servers.IEntityServer>().To<Servers.EntityModelServer>(); ;//单个注入
            // Configure the IoC container in here
            //  builder.Bind<Servers.ILogin>().To<Servers.LoginRespontory>();//注册login服务
            // builder.Bind(typeof(Servers.ILogin)).To(typeof(Servers.LoginRespontory));
            // builder.Bind<Servers.LoginRespontory>().ToSelf();//位置
            //  Container = builder.BuildContainer();
            //Container.GetAll<Servers.ILogin>();
            //var builder = new StyletIoCBuilder();
            //builder.Bind<Servers.ILogin>().To<Servers.LoginRespontory>();//注册login服务
            //IContainer Ioc = builder.BuildContainer();
            //var Login = Ioc.Get<Servers.ILogin>();

            #endregion
        }

        protected override void Configure()
        {
            // Perform any other configuration before the application starts
            //var vm = this.Container.Get<IWriteFile>();
            //var vm1 = this.Container.Get<ViewModels.LoginViewModel>();
        }

        protected override void OnStart()
        {
            Stylet.Logging.LogManager.LoggerFactory = name => new StyletLogger.MyLogger();
            Stylet.Logging.LogManager.Enabled = true;
        }
    }
}
