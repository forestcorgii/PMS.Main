using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.Extensions.Configuration;
using Pms.Timesheets.Domain;
using static Pms.Timesheets.ServiceLayer.TimeSystem.Services.Enums;

namespace Pms.Main.FrontEnd.Wpf.Views
{

    public partial class TimesheetPage:UserControl
    {
        public TimesheetPage()
        {
            InitializeComponent();
        }
    }
}