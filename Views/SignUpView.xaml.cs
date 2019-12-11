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
    /// SignUpView.xaml 的交互逻辑
    /// </summary>
    public partial class SignupView :MetroWindow
    {
        public SignupView()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MetroWindow_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if(IsEnabled==false)
            {
                this.Close();
            }
        }
    }
}
