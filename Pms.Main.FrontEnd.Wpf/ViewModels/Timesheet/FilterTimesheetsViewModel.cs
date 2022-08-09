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

namespace Pms.Main.FrontEnd.Wpf.ViewModel
{
    public class FilterTimesheetsViewModel
    {
        public delegate void ProgressHandler(object sender, int value);
        public delegate void ProgressStartedHandler(object sender, int maximum);
        public delegate void ProgressFailedHandler(object sender, string errorMessage);

        public event ProgressStartedHandler? TimesheetFillStarted;
        public event EventHandler? TimesheetFilled;
        public event ProgressFailedHandler? TimesheetFillFailed;

        private readonly TimesheetDbContext Context;

        public Cutoff Cutoff { get; private set; }

        public string PayrollCode { get; private set; }

        public FilterTimesheetsViewModel(Cutoff cutoff, string payrollCode)
        {
            //Context = new TimesheetDbContext();

            Cutoff = cutoff;
            PayrollCode = payrollCode;
        }
         
        public List<Timesheet> GetTimesheets()
        {
            return new List<Timesheet>();
            //ListTimesheetsService service = new(Context);
            //return service.GetTimesheetsByCutoffId(Cutoff.CutoffId, PayrollCode).ToList();
        }

        public List<string> GetCutoffs()
        {
            return new List<string>();
            //ListTimesheetsService service = new(Context);
            //return service.GetTimesheets().ToList()
            //        .GroupBy(ts => ts.CutoffId)
            //        .Select(ts => ts.First())
            //        .OrderByDescending(ts => ts.CutoffId)
            //        .Select(ts => new Cutoff(ts.CutoffId).CutoffId)
            //        .ToList();
        }


        public Task FillEmployeeDetail()
        {
            return Task.Run(() =>
            {
                //ListTimesheetsService ListingService = new(Context);
                //List<Timesheet> timesheets = ListingService.GetTimesheetsByCutoffId(Cutoff.CutoffId).Where(ts => ts.EE.PayrollCode == PayrollCode).ToList();

                //TimesheetFillStarted?.Invoke(this, timesheets.Count);
                //try
                //{
                //    SaveTimesheetBizLogic writeBizLogic = new(Context);
                //    foreach (Timesheet timesheet in timesheets)
                //    {
                //        writeBizLogic.SaveTimesheetEmployeeData(timesheet);
                //        TimesheetFilled?.Invoke(this, new EventArgs());
                //    }
                //    _ = Context.SaveChanges();
                //}
                //catch (Exception ex)
                //{
                //    TimesheetFillFailed?.Invoke(this, ex.Message);
                //}
            });
        }
    }
}
