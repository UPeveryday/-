using MahApps.Metro.Controls;
using PortableEquipment.Servers.EmbeddedApp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PortableEquipment.Views
{
    /// <summary>
    /// JfView.xaml 的交互逻辑
    /// </summary>
    public partial class JfView : Window
    {
        public JfView()
        {
            InitializeComponent();
            this.Topmost = true;

        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var item in Process.GetProcessesByName("数字式局部放电检测系统7.0"))
            {
                item.Kill();
                await Task.Delay(10);
            }
            if (Process.GetProcessesByName("数字式局部放电检测系统7.0").Length == 0)
            {
                string path = System.Environment.CurrentDirectory + "\\Jf\\数字式局部放电检测系统7.0.exe";
                EmbeddedApp ea = new EmbeddedApp(WndHost, 100, 100, path, "数字式局部放电检测系统");
                WndHost.Child = ea;
                //EmbeddedApp eb = new EmbeddedApp(WndHost, 100, 100, path, "数字式局部放电检测系统");
                //eb.IsOpen = true;
                //WndHost.Child = eb;
            }
        }


    }
}
