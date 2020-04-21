using Stylet;
using StyletIoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PortableEquipment.ViewModels
{
    public class JfViewModel : Screen, IHandle<WindowState>
    {
        [Inject]
        public IWindowManager _windowManager;
        public IEventAggregator _eventAggregator;

        public JfViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);
        }
        public void JfConfire()
        {
            windowslocation = false;
            _eventAggregator.Publish(new JfTestResult { Jffinish = true, Jfdata = Jfdata == null ? "0.00" : Jfdata });
            WindowState = WindowState.Minimized;
        }

        public void Handle(WindowState message)
        {
            WindowState = message;
        }

        public string Jfdata { get; set; }
        public bool windowslocation { get; set; } = true;

        public WindowState WindowState { get; set; } = WindowState.Normal;
    }
    public struct JfTestResult
    {
        public bool Jffinish;
        public string Jfdata;
    }
}
