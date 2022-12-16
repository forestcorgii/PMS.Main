using Microsoft.Toolkit.Mvvm.Input;
using Pms.Main.FrontEnd.Common.Utils;
using Pms.TimesheetModule.FrontEnd.Models;
using Pms.TimesheetModule.FrontEnd.ViewModels;
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

namespace Pms.TimesheetModule.FrontEnd.Commands
{
    public class Export : IRelayCommand
    {
        private readonly TimesheetListingVm ListingVm;
        private Models.Timesheets _model;

        public event EventHandler? CanExecuteChanged;

        public Export(TimesheetListingVm viewModel, Models.Timesheets model)
        {
            _model = model;
            ListingVm = viewModel;
            ListingVm.CanExecuteChanged += ListingVm_CanExecuteChanged;
        }

        public async void Execute(object? parameter)
        {

            Cutoff cutoff = ListingVm.Cutoff;
            string cutoffId = cutoff.CutoffId;
            string payrollCode = ListingVm.PayrollCode.PayrollCodeId;
            cutoff.SetSite(ListingVm.PayrollCode.Site);

            IEnumerable<Timesheet> timesheets = _model.GetTimesheets(cutoffId);
            timesheets = timesheets.FilterByPayrollCode(payrollCode);
            if (timesheets.Any(ts => !ts.IsValid))
                if (!MessageBoxes.Inquire("There are Timesheets that are invalid, do you want to proceed?"))
                    return;

            IEnumerable<Timesheet> twoPeriodTimesheets = _model.GetTwoPeriodTimesheets(cutoffId).FilterByPayrollCode(payrollCode);

            List<TimesheetBankChoices> bankCategories = timesheets.ExtractBanks();

            ListingVm.SetProgress("Exporting Timesheets", bankCategories.Count);

            await Task.Run(() =>
            {
                try
                {
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
                        ListingVm.ProgressValue++;
                    }
                }
                catch (Exception ex) { MessageBoxes.Error(ex.Message); }

            });
            ListingVm.SetAsFinishProgress();
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
            catch (Exception ex) { Console.WriteLine(ex.Message); }
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


        public bool CanExecute(object? parameter) => ListingVm.Executable;
        private void ListingVm_CanExecuteChanged(object? sender, bool e) => NotifyCanExecuteChanged();
        public void NotifyCanExecuteChanged() =>
            CanExecuteChanged?.Invoke(this, new EventArgs());
    }
}
