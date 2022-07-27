using Pms.Timesheets.Domain;
using Pms.Timesheets.Domain.SupportTypes;
using Pms.Timesheets.Persistence;
using Pms.Timesheets.ServiceLayer.Outputs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Main.FrontEnd.Wpf.Controller
{
    public class TimesheetOutputController
    {
        public delegate void ExportFailedHandler(object sender, string failedReason);
        public event EventHandler? ExportStarted;
        public event EventHandler? ExportEnded;
        public event ExportFailedHandler? ExportFailed;

        private readonly TimesheetDbContext Context;

        public TimesheetOutputController()
        {
            Context = new TimesheetDbContext();
        }



        public void ExportEfile(Cutoff cutoff, string payrollCode, string bankCategory, List<Timesheet> timesheets, List<Timesheet> unconfirmedTimesheetsWithAttendance, List<Timesheet> unconfirmedTimesheetsWithoutAttendance)
        {
            try
            {
                ExportStarted?.Invoke(this, new EventArgs());
                ExportTimesheetsEfileService service = new(cutoff, payrollCode, bankCategory, timesheets, unconfirmedTimesheetsWithAttendance, unconfirmedTimesheetsWithoutAttendance);

                string efiledir = $@"{AppDomain.CurrentDomain.BaseDirectory}\EXPORT";
                string efilepath = $@"{efiledir}\{payrollCode}_{bankCategory}_{cutoff.CutoffId}_{DateTime.Now:HHmmss}.xls";
                System.IO.Directory.CreateDirectory(efiledir);
                service.ExportEFile(efilepath);
                ExportEnded?.Invoke(this, new EventArgs());
            }
            catch (Exception ex)
            {
                ExportFailed?.Invoke(this, ex.Message);
            }
        }
    }
}
