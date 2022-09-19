using Microsoft.Toolkit.Mvvm.Input;
using Pms.Main.FrontEnd.Common.Utils;
using Pms.Main.FrontEnd.Wpf.Models;
using Pms.Main.FrontEnd.Wpf.ViewModels;
using Pms.Timesheets.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Pms.Main.FrontEnd.Wpf.Commands.Timesheets
{
    public class Evaluation : IRelayCommand
    {
        private TimesheetViewModel _viewModel;
        private TimesheetModel _model;

        public event EventHandler? CanExecuteChanged;

        public Evaluation(TimesheetViewModel viewModel, TimesheetModel model)
        {
            _viewModel = viewModel;
            _model = model;
        }

        public bool CanExecute(object? parameter) => true;

        public async void Execute(object? parameter)
        {
            string cutoffId = _viewModel.Cutoff.CutoffId;
            string payrollCode = _viewModel.PayrollCode.PayrollCodeId;

            int[] missingPages = _model.GetMissingPages(cutoffId, payrollCode);
            if (missingPages is not null && missingPages.Length == 0)
            {
                try
                {
                    IEnumerable<string> noEETimesheets = _model.ListTimesheetNoEETimesheet(cutoffId);

                    await FillEmployeeDetail();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            else
                _viewModel.DownloadCommand.Execute(missingPages);
            _viewModel.LoadTimesheets.Execute(null);
        }


        public Task FillEmployeeDetail()
        {
            return Task.Run(() =>
           {
               if (_viewModel.Cutoff is not null)
               {
                   try
                   {
                       List<Timesheet> timesheets = _model
                           .GetTimesheets(_viewModel.Cutoff.CutoffId)
                           .Where(ts => ts.EE.PayrollCode == _viewModel.PayrollCode.PayrollCodeId)
                           .ToList();

                       _viewModel.SetProgress("Filling Employee detail to Timesheets", timesheets.Count());

                       foreach (Timesheet timesheet in timesheets)
                       {
                           _model.SaveEmployeeData(timesheet);
                           _viewModel.ProgressValue++;
                       }
                   }
                   catch (Exception ex) { MessageBoxes.Error(ex.Message, "Timesheet Evaluation Error"); }

                   _viewModel.SetAsFinishProgress();
               }
           });
        }

        public void NotifyCanExecuteChanged()
        {

        }
    }
}
