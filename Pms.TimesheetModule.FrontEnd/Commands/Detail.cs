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
    public class Detail : IRelayCommand
    {
        private Models.Timesheets Timesheets;
        public Detail(Models.Timesheets timesheets)
        {
            Timesheets = timesheets;
        }

        public event EventHandler? CanExecuteChanged;

        private bool executable = true;
        public bool CanExecute(object? parameter) => executable;

        public void Execute(object? parameter)
        {
            executable = false;
            try
            {
                TimesheetDetailVm detailVm;
                if (parameter is Timesheet timesheet)
                    detailVm = new(Timesheets, timesheet);
                else detailVm = new(Timesheets, new());

                TimesheetDetailView detailView = new() { DataContext = detailVm };

                detailVm.OnRequestClose += (s, e) => detailView.Close();

                detailView.ShowDialog();
            }
            catch (Exception ex) { MessageBoxes.Error(ex.Message); }


            executable = true;
        }

        public void NotifyCanExecuteChanged() { }
    }
}
