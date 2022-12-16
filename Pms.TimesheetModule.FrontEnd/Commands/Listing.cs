using CommunityToolkit.Mvvm.Input;
using Pms.Main.FrontEnd.Common.Utils;
using Pms.TimesheetModule.FrontEnd.Models;
using Pms.TimesheetModule.FrontEnd.ViewModels;
using Pms.Timesheets.Domain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.TimesheetModule.FrontEnd.Commands
{
    public class Listing : IRelayCommand
    {
        public event EventHandler? CanExecuteChanged;

        TimesheetListingVm _viewModel;
        Models.Timesheets _model;
        private bool executable;

        public Listing(TimesheetListingVm viewModel, Models.Timesheets model)
        {
            _viewModel = viewModel;
            _model = model;
        }


        public bool CanExecute(object? parameter) => executable;


        public async void Execute(object? parameter)
        {
            executable = false;
            NotifyCanExecuteChanged();
            try
            {
                if (_viewModel.Cutoff is not null && _viewModel.PayrollCode is not null)
                {
                    IEnumerable<Timesheet> timesheets = new List<Timesheet>();
                    await Task.Run(() =>
                    {
                        timesheets = _model
                        .GetTimesheets(_viewModel.Cutoff.CutoffId)
                        .FilterPayrollCode(_viewModel.PayrollCode.PayrollCodeId)
                        .FilterSearchInput(_viewModel.SearchInput);
                    });

                    _viewModel.Timesheets = new ObservableCollection<Timesheet>(timesheets);
                    _viewModel.Confirmed = timesheets.Count(p => p.TotalHours > 0 && p.IsConfirmed);
                    _viewModel.CWithoutAttendance = timesheets.Count(p => p.TotalHours == 0 && p.IsConfirmed);
                    _viewModel.NotConfirmed = timesheets.Count(p => !p.IsConfirmed);
                    _viewModel.NCWithAttendance = timesheets.Count(p => p.TotalHours > 0 && !p.IsConfirmed);
                    _viewModel.TotalTimesheets = timesheets.Count();
                }
            }
            catch (Exception ex) { MessageBoxes.Error(ex.Message); }

            executable = true;
            NotifyCanExecuteChanged();
        }

        public void NotifyCanExecuteChanged() =>
            CanExecuteChanged?.Invoke(this, new EventArgs());

    }



    static class EmployeeFilterExtension
    {
        public static IEnumerable<Timesheet> FilterPayrollCode(this IEnumerable<Timesheet> timesheets, string payrollCode)
        {
            if (!string.IsNullOrEmpty(payrollCode))
                return timesheets
                    .Where(p => p.EE is not null)
                    .Where(p => p.EE.PayrollCode == payrollCode);
            else
                return timesheets.Where(p => p.EE is null);
        }

        public static IEnumerable<Timesheet> FilterSearchInput(this IEnumerable<Timesheet> timesheets, string filter)
        {
            if (!string.IsNullOrEmpty(filter))
                timesheets = timesheets
                   .Where(ts =>
                       ts.EEId.Contains(filter) ||
                       (
                            ts.EE is not null &&
                            ts.EE.Fullname.Contains(filter)
                       )
                   );

            return timesheets;
        }
    }
}
