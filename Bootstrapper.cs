using System;
using Stylet;
using StyletIoC;
using PortableEquipment.Pages;

namespace PortableEquipment
{
    public class Bootstrapper : Bootstrapper<ViewModels.MainViewModel>
    {
        protected override void ConfigureIoC(IStyletIoCBuilder builder)
        {
            // Configure the IoC container in here
        }

        protected override void Configure()
        {
            // Perform any other configuration before the application starts
        }
    }
}
