using System;
using Stylet;
using StyletIoC;
using PortableEquipment.Pages;
using System.Collections.Generic;

namespace PortableEquipment
{
    public class Bootstrapper : Bootstrapper<ViewModels.LoginViewModel>
    {
        protected override void ConfigureIoC(IStyletIoCBuilder builder)
        {
            builder.Bind(typeof(Servers.ILogin<>)).ToAllImplementations();//注入此接口所有的服务
            builder.Bind<Servers.IEntityServer>().To<Servers.EntityModelServer>(); ;//单个注入

            // Configure the IoC container in here
            //  builder.Bind<Servers.ILogin>().To<Servers.LoginRespontory>();//注册login服务
            // builder.Bind(typeof(Servers.ILogin)).To(typeof(Servers.LoginRespontory));
            // builder.Bind<Servers.LoginRespontory>().ToSelf();//位置
            //builder.Bind(typeof(Servers.ILogin<>)).To(typeof(Servers.LoginRespontory<>));
            //builder.Bind(typeof(Servers.ILogin<>)).To(typeof(Servers.LoginSingalRespontitory<>));
            //  Container = builder.BuildContainer();
            //Container.GetAll<Servers.ILogin>();
            //var builder = new StyletIoCBuilder();
            //builder.Bind<Servers.ILogin>().To<Servers.LoginRespontory>();//注册login服务
            //IContainer Ioc = builder.BuildContainer();
            //var Login = Ioc.Get<Servers.ILogin>();
        }

        protected override void Configure()
        {
            // Perform any other configuration before the application starts
        }
    }
}
