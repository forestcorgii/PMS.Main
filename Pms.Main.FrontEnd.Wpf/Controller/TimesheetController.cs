using Pms.Timesheets.BizLogic.Concrete;
using Pms.Timesheets.Domain;
using Pms.Timesheets.Domain.SupportTypes;
using Pms.Timesheets.Persistence;
using Pms.Timesheets.ServiceLayer.EfCore.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Main.FrontEnd.Wpf.Controller
{
    public class TimesheetController
    {
        public delegate void ProgressHandler(object sender, int value);
        public delegate void ProgressStartedHandler(object sender, int maximum);
        public delegate void ProgressFailedHandler(object sender, string errorMessage);

        public event ProgressStartedHandler? TimesheetFillStarted;
        public event EventHandler? TimesheetFilled;
        public event ProgressFailedHandler? TimesheetFillFailed;

        private readonly TimesheetDbContext Context;

        public TimesheetController()
        {
            Context = new TimesheetDbContext();
        }

        public List<Timesheet> GetTimesheets()
        {
            ListTimesheetsService service = new(Context);
            return service.GetTimesheets().ToList();
        }

        public List<Timesheet> GetTimesheets(string cutoffId, string payrollCode)
        {
            ListTimesheetsService service = new(Context);
            return service.GetTimesheetsByCutoffId(cutoffId, payrollCode).ToList();
        }

        public List<string> GetCutoffs()
        {
            ListTimesheetsService service = new(Context);
            return service.GetTimesheets().ToList()
                    .GroupBy(ts => ts.CutoffId)
                    .Select(ts => ts.First())
                    .OrderByDescending(ts=>ts.CutoffId)
                    .Select(ts => new Cutoff(ts.CutoffId).CutoffId)
                    .ToList();
        }


        public Task FillEmployeeDetail(string cutoffId, string payrollCode)
        {
            return Task.Run(() =>
            {
                ListTimesheetsService ListingService = new(Context);
                List<Timesheet> timesheets = ListingService.GetTimesheetsByCutoffId(cutoffId).Where(ts => ts.EE.PayrollCode == payrollCode).ToList();
   
                TimesheetFillStarted?.Invoke(this, timesheets.Count);
                try
                {
                    SaveTimesheetBizLogic writeBizLogic = new(Context);
                    foreach (Timesheet timesheet in timesheets)
                    {
                        writeBizLogic.SaveTimesheetEmployeeData(timesheet);
                        TimesheetFilled?.Invoke(this, new EventArgs());
                    }
                }
                catch (Exception ex)
                {
                    TimesheetFillFailed?.Invoke(this, ex.Message);
                }
            });
        }
    }
}
