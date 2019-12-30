using MahApps.Metro.Controls;
using System.Windows;
using System.Windows.Input;

namespace PortableEquipment.Views
{
    /// <summary>
    /// ManualVoltageView.xaml 的交互逻辑
    /// </summary>
    public partial class ManualVoltageView : MetroWindow
    {
        public ManualVoltageView()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Grid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

       
    }

    
}
