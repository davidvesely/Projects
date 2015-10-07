using System;
using System.Collections.Generic;
using System.Linq;
using CarRental.Admin.ViewModels;
using Core.Common.Core;
using MahApps.Metro.Controls;

namespace CarRental.Admin
{
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            main.DataContext = ObjectBase.Container.GetExportedValue<MainViewModel>();
        }
    }
}
