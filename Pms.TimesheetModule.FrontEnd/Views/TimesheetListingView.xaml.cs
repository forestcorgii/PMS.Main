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

namespace Pms.TimesheetModule.FrontEnd.Views
{

    public partial class TimesheetListingView : UserControl
    {
        public TimesheetListingView()
        {
            InitializeComponent();
        }
    }
}