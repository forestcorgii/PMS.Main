using Microsoft.Toolkit.Mvvm.Input;
using Pms.Main.FrontEnd.Wpf.Models;
using Pms.Main.FrontEnd.Wpf.Stores;
using Pms.Main.FrontEnd.Wpf.ViewModels;
using Pms.Timesheets.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Main.FrontEnd.Wpf.Commands
{
    public class TimesheetEvaluationCommand : IRelayCommand
    {
        private TimesheetViewModel _viewModel;
        private MainStore _mainStore;
        private TimesheetModel _model;

        public event EventHandler? CanExecuteChanged;

        public TimesheetEvaluationCommand(TimesheetViewModel viewModel, TimesheetModel model, MainStore mainStore)
        {
            _viewModel = viewModel;
            _mainStore = mainStore;
            _model = model;
        }

        public bool CanExecute(object? parameter) => true;

        public async void Execute(object? parameter)
        {
            string cutoffId = _mainStore.Cutoff.CutoffId;
            string payrollCode = _mainStore.PayrollCode;

            int[] missingPages = _model.GetMissingPages(cutoffId, payrollCode);
            if (missingPages is not null && missingPages.Length == 0)
            {
                try
                {
                    IEnumerable<string> noEETimesheets = _model.ListTimesheetNoEETimesheet(cutoffId);

                    if (noEETimesheets.Any())
                        await _viewModel.EmployeeDownloadCommand.ExecuteAsync(noEETimesheets.ToArray());

                    await FillEmployeeDetail();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            else
                _viewModel.DownloadCommand.Execute(missingPages);

            _viewModel.LoadTimesheetCommand.Execute(true);
            _viewModel.LoadFilterCommand.Execute(null);
        }


        public Task FillEmployeeDetail()
        {
            return Task.Run(() =>
           {
               if (_mainStore.Cutoff is not null)
               {
                   try
                   {
                       List<Timesheet> timesheets = _model
                           .GetTimesheets(_mainStore.Cutoff.CutoffId)
                           .Where(ts => ts.EE.PayrollCode == _mainStore.PayrollCode)
                           .ToList();

                       _viewModel.SetProgress("Filling Employee detail to Timesheets", timesheets.Count());

                       foreach (Timesheet timesheet in timesheets)
                       {
                           _model.SaveEmployeeData(timesheet);
                           _viewModel.ProgressValue++;
                       }
                   }
                   catch (Exception ex)
                   {
                       Console.WriteLine(ex.Message);
                   }
                   _viewModel.SetAsFinishProgress();
               }
           });
        }

        public void NotifyCanExecuteChanged()
        {

        }
    }
}
