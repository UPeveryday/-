using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stylet;
using StyletIoC;

namespace PortableEquipment.Pages.Home
{
    public class HomeViewModel : Screen, IHandle<string>
    {
        [Inject]//特性注入
        public IEventAggregator _eventAggregator;

        [Inject]
        public HomeViewModel()
        {
            DisplayName = "Home";
        }

        protected override void OnInitialActivate()
        {
            //base.OnInitialActivate();
            //_eventAggregator.Subscribe(this);
        }

        public string title { get; set; } = "图书管理系统  Hello World";
        public void Handle(string message)
        {
            title = message;
        }
    }
}
