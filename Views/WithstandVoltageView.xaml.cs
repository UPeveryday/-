﻿using MahApps.Metro.Controls;
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
    /// WithstandVoltage.xaml 的交互逻辑
    /// </summary>
    public partial class WithstandVoltageView : MetroWindow
    {
        public WithstandVoltageView()
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
    }
}
