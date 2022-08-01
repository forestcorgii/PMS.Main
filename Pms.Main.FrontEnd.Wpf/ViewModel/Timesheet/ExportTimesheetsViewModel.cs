using Pms.Timesheets.Domain;
using Pms.Timesheets.Domain.SupportTypes;
using Pms.Timesheets.Persistence;
using Pms.Timesheets.ServiceLayer.EfCore.Concrete;
using Pms.Timesheets.ServiceLayer.EfCore.Queries;
using Pms.Timesheets.ServiceLayer.Outputs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Main.FrontEnd.Wpf.ViewModel
{
    public class ExportTimesheetsViewModel
    {
        public delegate void ExportFailedHandler(object sender, string failedReason);
        public event EventHandler? ExportStarted;
        public event EventHandler? ExportEnded;
        public event ExportFailedHandler? ExportFailed;

        private readonly TimesheetDbContext Context;

        public Cutoff Cutoff { get; private set; }

        public string PayrollCode { get; private set; }

        public ExportTimesheetsViewModel(Cutoff cutoff, string payrollCode)
        {
            Context = new TimesheetDbContext();

            Cutoff = cutoff;
            PayrollCode = payrollCode;
        }



        public void Export()
        {
            ListTimesheetsService service = new(Context);
            List<string> bankCategories = service.ListTimesheetBankCategory(PayrollCode);

            foreach (string bankCategory in bankCategories)
            {
                var timesheets = service.GetTimesheetsByCutoffId(Cutoff.CutoffId, PayrollCode, bankCategory);
                if (timesheets.Any())
                {
                    List<Timesheet> exportable = timesheets.ByExportable().ToList();
                    List<Timesheet> unconfirmedTimesheetsWithAttendance = timesheets.ByUnconfirmedWithAttendance().ToList();
                    List<Timesheet> unconfirmedTimesheetsWithoutAttendance = timesheets.ByUnconfirmedWithoutAttendance().ToList();

                    ExportEfile(bankCategory, exportable, unconfirmedTimesheetsWithAttendance, unconfirmedTimesheetsWithoutAttendance);
                    ExportDBF(bankCategory, exportable);
                }
            }
        }

        public void ExportEfile(string bankCategory, List<Timesheet> exportable, List<Timesheet> unconfirmedTimesheetsWithAttendance, List<Timesheet> unconfirmedTimesheetsWithoutAttendance)
        {
            try
            {
                ExportStarted?.Invoke(this, new EventArgs());
                ExportTimesheetsEfileService service = new(Cutoff, PayrollCode, bankCategory, exportable, unconfirmedTimesheetsWithAttendance, unconfirmedTimesheetsWithoutAttendance);

                string efiledir = $@"{AppDomain.CurrentDomain.BaseDirectory}\EXPORT\EFILE\{Cutoff.CutoffId}";
                string efilepath = $@"{efiledir}\{PayrollCode}_{bankCategory}_{Cutoff.CutoffId}_{DateTime.Now:HHmmss}.xls";
                System.IO.Directory.CreateDirectory(efiledir);
                service.ExportEFile(efilepath);
                ExportEnded?.Invoke(this, new EventArgs());
            }
            catch (Exception ex)
            {
                ExportFailed?.Invoke(this, ex.Message);
            }
        }

        public void ExportDBF(string bankCategory, List<Timesheet> exportable)
        {
            try
            {
                ExportTimesheetsDbfService service = new();
                string dbfdir = $@"{AppDomain.CurrentDomain.BaseDirectory}\EXPORT\DBF\{Cutoff.CutoffId}";
                string dbfpath = $@"{dbfdir}\{PayrollCode}_{bankCategory}_{Cutoff.CutoffId}_{DateTime.Now:HHmmss}.dbf";
                System.IO.Directory.CreateDirectory(dbfdir);

                service.ExportDBF(dbfpath, Cutoff.CutoffDate, exportable);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
