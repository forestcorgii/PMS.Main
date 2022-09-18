using Microsoft.Toolkit.Mvvm.Input;
using Pms.Main.FrontEnd.Wpf.Models;
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
using static Pms.Payrolls.Domain.TimesheetEnums;

namespace Pms.Main.FrontEnd.Wpf.Commands.Timesheets
{
    public class Export : IRelayCommand
    {
        private readonly TimesheetViewModel _viewModel;
        private TimesheetModel _model;

        public event EventHandler? CanExecuteChanged;

        public Export(TimesheetViewModel viewModel, TimesheetModel model)
        {
            _viewModel = viewModel;
            _model = model;
        }

        private bool executable = true;
        public bool CanExecute(object? parameter) => executable;

        public async void Execute(object? parameter)
        {
            executable = false;

            await Task.Run(() =>
            {
                Cutoff cutoff = _viewModel.Cutoff;
                string cutoffId = cutoff.CutoffId;
                string payrollCode = _viewModel.PayrollCode.PayrollCodeId;

                IEnumerable<Timesheet> timesheets = _model.GetTimesheets(cutoffId).FilterByPayrollCode(payrollCode);
                IEnumerable<Timesheet> twoPeriodTimesheets = _model.GetTwoPeriodTimesheets(cutoffId).FilterByPayrollCode(payrollCode);
                
                List<TimesheetBankChoices> bankCategories = timesheets.ExtractBanks();

                _viewModel.SetProgress("Exporting Timesheets", bankCategories.Count);
                foreach (TimesheetBankChoices bankCategory in bankCategories)
                {
                    var timesheetsByBankCategory = timesheets.FilterByBank(bankCategory);
                    var twoPeriodTimesheetsByBankCategory = twoPeriodTimesheets.FilterByBank(bankCategory);
                    if (timesheetsByBankCategory.Any())
                    {
                        List<Timesheet> exportable = timesheetsByBankCategory.ByExportable().ToList();
                        ExportDBF(cutoff, payrollCode, bankCategory, exportable);
                        
                        List<Timesheet> unconfirmedTimesheetsWithAttendance = timesheetsByBankCategory.ByUnconfirmedWithAttendance().ToList();
                        List<Timesheet> unconfirmedTimesheetsWithoutAttendance = timesheetsByBankCategory.ByUnconfirmedWithoutAttendance().ToList();
                        ExportFeedback(cutoff, payrollCode, bankCategory, exportable, unconfirmedTimesheetsWithAttendance, unconfirmedTimesheetsWithoutAttendance);


                        IEnumerable<Timesheet> monthlyExportable = twoPeriodTimesheetsByBankCategory.ByExportable();
                        ExportEFile(cutoff, payrollCode, bankCategory, monthlyExportable.GroupTimesheetsByEEId().ToList());
                    }
                    _viewModel.ProgressValue++;
                }
                _viewModel.SetAsFinishProgress();
            });

            executable = true;
        }

        public void ExportFeedback(Cutoff cutoff, string payrollCode, TimesheetBankChoices bank, List<Timesheet> exportable, List<Timesheet> unconfirmedTimesheetsWithAttendance, List<Timesheet> unconfirmedTimesheetsWithoutAttendance)
        {
            try
            {
                TimesheetFeedbackExporter service = new(cutoff, payrollCode, bank, exportable, unconfirmedTimesheetsWithAttendance, unconfirmedTimesheetsWithoutAttendance);

                string efiledir = $@"{AppDomain.CurrentDomain.BaseDirectory}\EXPORT\{cutoff.CutoffId}\{payrollCode}";
                string efilepath = $@"{efiledir}\{payrollCode}_{bank}_{cutoff.CutoffId}-FEEDBACK.XLS";
                System.IO.Directory.CreateDirectory(efiledir);
                service.StartExport(efilepath);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void ExportEFile(Cutoff cutoff, string payrollCode, TimesheetBankChoices bank, List<Timesheet[]> exportable)
        {
            try
            {
                TimesheetEfileExporter service = new(cutoff, payrollCode, bank, exportable);

                string efiledir = $@"{AppDomain.CurrentDomain.BaseDirectory}\EXPORT\{cutoff.CutoffId}\{payrollCode}";
                string efilepath = $@"{efiledir}\{payrollCode}_{bank}_{cutoff.CutoffId}.XLS";
                System.IO.Directory.CreateDirectory(efiledir);
                service.ExportEFile(efilepath);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void ExportDBF(Cutoff cutoff, string payrollCode, TimesheetBankChoices bank, List<Timesheet> exportable)
        {
            try
            {
                ExportTimesheetsDbfService service = new();
                string dbfdir = $@"{AppDomain.CurrentDomain.BaseDirectory}\EXPORT\{cutoff.CutoffId}\{payrollCode}";
                string dbfpath = $@"{dbfdir}\{payrollCode}_{bank}_{cutoff.CutoffId}.DBF";
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
