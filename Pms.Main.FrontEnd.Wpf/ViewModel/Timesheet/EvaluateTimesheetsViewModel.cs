using Pms.Timesheets.Domain;
using Pms.Timesheets.Domain.SupportTypes;
using Pms.Timesheets.Persistence;
using Pms.Timesheets.ServiceLayer.EfCore.Concrete;
using Pms.Timesheets.ServiceLayer.EfCore.Queries;
using Pms.Timesheets.ServiceLayer.TimeSystem.Adapter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Main.FrontEnd.Wpf.ViewModel
{
    public class EvaluateTimesheetsViewModel
    {
        #region Event Handler
        public delegate void ProcessExceptionHandler(object sender, string errorMessage);
        public delegate void EvaluationSucceededHandler(object sender, EvaluationResultArgs e);
        #endregion

        #region Event
        public event EventHandler? EvaluationStarted; // Checks if the downloaded timesheets are complete and if all employee data are present in the database.
        public event EvaluationSucceededHandler? EvaluationSucceeded;
        public event ProcessExceptionHandler? EvaluationFailed;// Please try again 
        #endregion

        public bool IsBusy { get; private set; }

        private readonly TimesheetDbContext Context;

        public Cutoff Cutoff { get; private set; }

        public string PayrollCode { get; private set; }

        public EvaluateTimesheetsViewModel(Cutoff cutoff, string payrollCode)
        {
            Context = new TimesheetDbContext();

            Cutoff = cutoff;
            PayrollCode = payrollCode;
        }



        public void EvaluateTimesheets()
        {
            EvaluationStarted?.Invoke(this, new EventArgs());
            EvaluationResultArgs args = new();
            try
            {
                TimesheetPageService PageService = new(Context);
                ListTimesheetsService ListingService = new(Context);
                args.MissingPages = PageService.GetMissingPages(Cutoff.CutoffId, PayrollCode);
                if (args.MissingPages is not null && args.MissingPages.Count == 0)
                {
                    args.NoEETimesheets = ListingService
                        .GetTimesheetNoEETimesheet(Cutoff.CutoffId)
                        .Select(ts => ts.EEId)
                        .ToList();

                    args.Timesheets = ListingService.GetTimesheetsByCutoffId(Cutoff.CutoffId, PayrollCode)
                        .ByExportable().ToList();

                    args.UnconfirmedTimesheetsWithAttendance = ListingService.GetTimesheetsByCutoffId(Cutoff.CutoffId, PayrollCode)
                        .ByUnconfirmedWithAttendance()
                        .ToList();

                    args.UnconfirmedTimesheetsWithoutAttendance = ListingService.GetTimesheetsByCutoffId(Cutoff.CutoffId, PayrollCode)
                        .ByUnconfirmedWithoutAttendance()
                        .ToList();
                }
                EvaluationSucceeded?.Invoke(this, args);
            }
            catch (Exception ex)
            {
                EvaluationFailed?.Invoke(this, ex.Message);
            }
        }
    }

    public class EvaluationResultArgs
    {
        public List<string>? NoEETimesheets { get; set; }// Find on Server
        public List<int>? MissingPages { get; set; }// Re Download
        public List<Timesheet>? Timesheets { get; set; }// Include in Report
        public List<Timesheet>? UnconfirmedTimesheetsWithAttendance { get; set; }// Include in Report
        public List<Timesheet>? UnconfirmedTimesheetsWithoutAttendance { get; set; }// Include in Report
    }
}
