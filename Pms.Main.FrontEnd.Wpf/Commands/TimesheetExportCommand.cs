using Microsoft.Toolkit.Mvvm.Input;
using Pms.Main.FrontEnd.Wpf.Models;
using Pms.Main.FrontEnd.Wpf.Stores;
using Pms.Main.FrontEnd.Wpf.ViewModels;
using Pms.Timesheets.Domain;
using Pms.Timesheets.Domain.SupportTypes;
using Pms.Timesheets.ServiceLayer.EfCore;
using Pms.Timesheets.ServiceLayer.Outputs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Main.FrontEnd.Wpf.Commands
{
    public class TimesheetExportCommand : IRelayCommand
    {
        private readonly TimesheetViewModel _viewModel;
        private MainStore _cutoffStore;
        private CutoffTimesheet _cutoffTimesheet;

        public event EventHandler? CanExecuteChanged;

        public TimesheetExportCommand(TimesheetViewModel viewModel, CutoffTimesheet cutoffTimesheet, MainStore cutoffStore)
        {
            _viewModel = viewModel;
            _cutoffStore = cutoffStore;
            _cutoffTimesheet = cutoffTimesheet;
        }


        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public async void Execute(object? parameter)
        {
            await Task.Run(() =>
            {
                Cutoff cutoff = _cutoffStore.Cutoff;
                string cutoffId = cutoff.CutoffId;
                string payrollCode = _cutoffStore.PayrollCode;

                IEnumerable<Timesheet> timesheets = _cutoffTimesheet.GetTimesheets(cutoffId).FilterByPayrollCode(payrollCode);
                List<string> bankCategories = timesheets.ExtractBankCategories();

                _viewModel.SetProgress("Exporting Timesheets", bankCategories.Count);

                foreach (string bankCategory in bankCategories)
                {
                    var timesheetsByBankCategory = timesheets.FilterByBankCategory(bankCategory);
                    if (timesheets.Any())
                    {
                        List<Timesheet> exportable = timesheets.ByExportable().ToList();
                        List<Timesheet> unconfirmedTimesheetsWithAttendance = timesheets.ByUnconfirmedWithAttendance().ToList();
                        List<Timesheet> unconfirmedTimesheetsWithoutAttendance = timesheets.ByUnconfirmedWithoutAttendance().ToList();

                        ExportEfile(cutoff, payrollCode, bankCategory, exportable, unconfirmedTimesheetsWithAttendance, unconfirmedTimesheetsWithoutAttendance);
                        ExportDBF(cutoff, payrollCode, bankCategory, exportable);
                    }
                    _viewModel.ProgressValue++;
                }
                _viewModel.SetAsFinishProgress();
            });
        }

        public void ExportEfile(Cutoff cutoff, string payrollCode, string bankCategory, List<Timesheet> exportable, List<Timesheet> unconfirmedTimesheetsWithAttendance, List<Timesheet> unconfirmedTimesheetsWithoutAttendance)
        {
            try
            {
                ExportTimesheetsEfileService service = new(cutoff, payrollCode, bankCategory, exportable, unconfirmedTimesheetsWithAttendance, unconfirmedTimesheetsWithoutAttendance);

                string efiledir = $@"{AppDomain.CurrentDomain.BaseDirectory}\EXPORT\{cutoff.CutoffId}\{payrollCode}";
                string efilepath = $@"{efiledir}\{payrollCode}_{bankCategory}_{cutoff.CutoffId}.XLS";
                System.IO.Directory.CreateDirectory(efiledir);
                service.ExportEFile(efilepath);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void ExportDBF(Cutoff cutoff, string payrollCode, string bankCategory, List<Timesheet> exportable)
        {
            try
            {
                ExportTimesheetsDbfService service = new();
                string dbfdir = $@"{AppDomain.CurrentDomain.BaseDirectory}\EXPORT\{cutoff.CutoffId}\{payrollCode}";
                string dbfpath = $@"{dbfdir}\{payrollCode}_{bankCategory}_{cutoff.CutoffId}.DBF";
                System.IO.Directory.CreateDirectory(dbfdir);

                service.ExportDBF(dbfpath, cutoff.CutoffDate, exportable);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void NotifyCanExecuteChanged()
        {

        }
    }
}
