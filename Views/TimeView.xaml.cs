using PortableEquipment.ViewModels;
using System.Collections.Generic;
using System.Windows;

namespace PortableEquipment.Views
{
    /// <summary>
    /// TimeView.xaml 的交互逻辑
    /// </summary>
    public partial class TimeView : Window
    {
        public TimeView()
        {
            InitializeComponent();
        }
        List<string> VolataGroup = new List<string>();
        private void Sample1_DialogHost_OnDialogClosing(object sender, MaterialDesignThemes.Wpf.DialogClosingEventArgs eventArgs)
        {
            if ((bool)eventArgs.Parameter)
            {
                //VolataGroup.Add(FruitTextBox.Text);
                //FruitListBox.ItemsSource = VolataGroup;
                (this.DataContext as TimeViewModel).VolataGroup.Add((this.DataContext as TimeViewModel).Conetnt);
            }

        }
    }
}
