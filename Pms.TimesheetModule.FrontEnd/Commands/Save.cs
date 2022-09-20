using Microsoft.Toolkit.Mvvm.Input;
using Pms.Main.FrontEnd.Common.Utils;
using Pms.TimesheetModule.FrontEnd.Models;
using Pms.TimesheetModule.FrontEnd.ViewModels;
using Pms.TimesheetModule.FrontEnd.Views;
using Pms.Timesheets.Domain;
using Pms.Timesheets.Domain.SupportTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Pms.TimesheetModule.FrontEnd.Commands
{
    public class Save : IRelayCommand
    {
        private TimesheetDetailVm DetailVm;
        private Models.Timesheets Timesheets;
        public Save(TimesheetDetailVm detailVm, Models.Timesheets timesheets)
        {
            Timesheets = timesheets;
            DetailVm = detailVm;
        }

        public event EventHandler? CanExecuteChanged;

        private bool executable = true;
        public bool CanExecute(object? parameter) => executable;

        public void Execute(object? parameter)
        {
            executable = false;
            
            Timesheets.SaveTimesheet(DetailVm.Timesheet);

            DetailVm.Close();

            executable = true;
        }



        public void NotifyCanExecuteChanged() { }
    }
}
