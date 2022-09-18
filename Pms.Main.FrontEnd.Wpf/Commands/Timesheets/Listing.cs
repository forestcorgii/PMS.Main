using CommunityToolkit.Mvvm.Input;
using Pms.Main.FrontEnd.Wpf.Models;
using Pms.Main.FrontEnd.Wpf.ViewModels;
using Pms.Timesheets.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Main.FrontEnd.Wpf.Commands.Timesheets
{
    public class Listing : IRelayCommand
    {
        public event EventHandler? CanExecuteChanged;

        TimesheetViewModel _viewModel;
        TimesheetModel _model;
        private bool executable;

        public Listing(TimesheetViewModel viewModel, TimesheetModel model)
        {
            _viewModel = viewModel;
            _model = model;
        }


        public bool CanExecute(object? parameter) => executable;


        public async void Execute(object? parameter)
        {
            executable = false;
            try
            {
                if (_viewModel.Cutoff is not null && _viewModel.PayrollCode is not null)
                {
                    IEnumerable<Timesheet> timesheets = new List<Timesheet>();
                    await Task.Run(() =>
                    {
                        timesheets = _model.GetTimesheets(_viewModel.Cutoff.CutoffId).Where(ts => ts.PayrollCode == _viewModel.PayrollCode.PayrollCodeId);
                    });

                    _viewModel.Timesheets = timesheets;

                }
            }
            catch (Exception ex) { MessageBoxes.Error(ex.Message); }

            executable = true;
            NotifyCanExecuteChanged();
        }

        public void NotifyCanExecuteChanged() =>
            CanExecuteChanged?.Invoke(this, new EventArgs());
    }
}
