using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
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
    /// ParameterSettingView.xaml 的交互逻辑
    /// </summary>
    public partial class ParameterSettingView : MetroWindow
    {
        public ParameterSettingView()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void MetroWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }

            // 双击放大
            if (e.ClickCount == 2 && e.ChangedButton == MouseButton.Left)
            {
                this.WindowState = this.WindowState == WindowState.Maximized ?
               WindowState.Normal : WindowState.Maximized;
            }
        }

        private void Sample1_DialogHost_OnDialogClosing(object sender, MaterialDesignThemes.Wpf.DialogClosingEventArgs eventArgs)
        {

        }
    }
}
