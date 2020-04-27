using Stylet;
using StyletIoC;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;

namespace PortableEquipment.ViewModels
{
    public class TimeViewModel : Screen, IHandle<HideMessage>
    {
        public IEventAggregator _eventAggregator;
        public TimeViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);
        }
        public void ConfireClick()
        {
            
            this.RequestClose();
        }
        public void CancerClick()
        {
            if (Cancertext == "重试")
                _eventAggregator.Publish(Stata.Redo);
            this.RequestClose();
        }

        
        public void Handle(HideMessage message)
        {
            HeaderText = message.HeaderText;
            Messages = message.Messages;
            ConfireVisibility = message.ConfireVisibility;
            CancerVisibility = message.CancerVisibility;
            Cancertext = message.Cancertext;
        }

        public string HeaderText { get; set; } = "警告";
        public string Cancertext { get; set; } = "取消";
        public string Messages { get; set; }
        public System.Windows.Visibility ConfireVisibility { get; set; }
        public System.Windows.Visibility CancerVisibility { get; set; }

    }

    public class HideMessage
    {
        public string HeaderText { get; set; } = "警告";
        public string Messages { get; set; }
        public System.Windows.Visibility ConfireVisibility { get; set; }
        public System.Windows.Visibility CancerVisibility { get; set; }
        public string Cancertext { get; set; }
    }

    public enum Stata
    {
        Redo = 0,
        Cancer = 1
    }

}
