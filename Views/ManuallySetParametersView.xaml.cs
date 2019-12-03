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
    /// ManuallySetParametersView.xaml 的交互逻辑
    /// </summary>
    public partial class ManuallySetParametersView : MetroWindow
    {
        public ManuallySetParametersView()
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void High_click(object sender, RoutedEventArgs e)
        {
            //BB.Visibility = Visibility.Collapsed;
            //RS.Visibility = Visibility.Collapsed;

            BB.Opacity = 0.3;
            RS.Opacity = 0.3;
            BB.IsEnabled = false;
            RS.IsEnabled = false;
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            BB.Opacity = 1;
            RS.Opacity = 1;
            BB.IsEnabled = true;
            RS.IsEnabled = true;
        }
    }
}
