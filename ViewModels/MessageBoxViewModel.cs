using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PortableEquipment.ViewModels
{
    public class MessageBoxViewModel : Screen, IMessageBoxViewModel
    {
        public MessageBoxResult ClickedButton { get => boxResult; }

        public void Setup(string messageBoxText, string caption = null, MessageBoxButton buttons = MessageBoxButton.OK, MessageBoxImage icon = MessageBoxImage.None, MessageBoxResult defaultResult = MessageBoxResult.None, MessageBoxResult cancelResult = MessageBoxResult.None, IDictionary<MessageBoxResult, string> buttonLabels = null, FlowDirection? flowDirection = null, TextAlignment? textAlignment = null)
        {
            MessageBoxText = messageBoxText;
            Caption = caption;
            Buttons = buttons;
        }

        public string MessageBoxText { get; set; }
        public string Caption { get; set; } = "提示";

        private MessageBoxButton messageBoxButton;

        public MessageBoxButton Buttons
        {
            get
            {
                return messageBoxButton;
            }
            set
            {
                messageBoxButton = value;
                if (messageBoxButton == MessageBoxButton.OK)
                {
                    ConfireVisibility = Visibility.Visible;
                    CancerVisibility = Visibility.Hidden;

                }
                if (messageBoxButton == MessageBoxButton.OKCancel)
                {
                    ConfireVisibility = Visibility.Visible;
                    CancerVisibility = Visibility.Visible;
                }
                if (messageBoxButton == MessageBoxButton.YesNo)
                {
                    ConfireVisibility = Visibility.Visible;
                    CancerVisibility = Visibility.Visible;
                }
            }
        }

        public Visibility CancerVisibility { get; set; } = Visibility.Hidden;
        public Visibility ConfireVisibility { get; set; }
        public MessageBoxResult boxResult { get; set; }

        public string Cancertext { get; set; } = "取消";
        public string Confiretext { get; set; } = "确认";


        public void ConfireClick()
        {
            if (Buttons == MessageBoxButton.OK || Buttons == MessageBoxButton.OKCancel)
                boxResult = MessageBoxResult.OK;
            else if (Buttons == MessageBoxButton.YesNo)
                boxResult = MessageBoxResult.Yes;
            else
                boxResult = MessageBoxResult.OK;
            this.RequestClose();
        }
        public void CancerClick()
        {
            if (Buttons == MessageBoxButton.OK || Buttons == MessageBoxButton.OKCancel)
                boxResult = MessageBoxResult.Cancel;
            else if (Buttons == MessageBoxButton.YesNo)
                boxResult = MessageBoxResult.No;
            else
                boxResult = MessageBoxResult.Cancel;
            this.RequestClose();
        }
    }
}
