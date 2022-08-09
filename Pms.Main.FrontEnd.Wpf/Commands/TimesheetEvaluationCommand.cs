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
        private MainStore _cutoffStore;
        private CutoffTimesheet _cutoffTimesheet;

        public event EventHandler? CanExecuteChanged;

        public TimesheetEvaluationCommand(TimesheetViewModel viewModel, CutoffTimesheet cutoffTimesheet, MainStore cutoffStore)
        {
            _viewModel = viewModel;
            _cutoffStore = cutoffStore;
            _cutoffTimesheet = cutoffTimesheet;
        }

        public bool CanExecute(object? parameter) => true;

        public async void Execute(object? parameter)
        {
            string cutoffId = _cutoffStore.Cutoff.CutoffId;
            string payrollCode = _cutoffStore.PayrollCode;

            int[] missingPages = _cutoffTimesheet.GetMissingPages(cutoffId, payrollCode);
            if (missingPages is not null && missingPages.Length == 0)
            {
                try
                {
                    IEnumerable<string> noEETimesheets = _cutoffTimesheet.ListTimesheetNoEETimesheet(cutoffId);

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

            _viewModel.LoadFilterCommand.Execute(null);
        }


        public Task FillEmployeeDetail()
        {
            return Task.Run(() =>
           {
               if (_cutoffStore.Cutoff is not null)
               {
                   try
                   {
                       List<Timesheet> timesheets = _cutoffTimesheet
                           .GetTimesheets(_cutoffStore.Cutoff.CutoffId)
                           .Where(ts => ts.EE.PayrollCode == _cutoffStore.PayrollCode)
                           .ToList();

                       _viewModel.SetProgress("Filling Employee detail to Timesheets", timesheets.Count());

                       foreach (Timesheet timesheet in timesheets)
                       {
                           _cutoffTimesheet.SaveEmployeeData(timesheet);
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
