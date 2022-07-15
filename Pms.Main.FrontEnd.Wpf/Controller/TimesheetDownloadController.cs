using Payroll.Employees.Domain;
using Payroll.Timesheets.BizLogic.Concrete;
using Payroll.Timesheets.Domain;
using Payroll.Timesheets.Domain.SupportTypes;
using Payroll.Timesheets.Persistence;
using Payroll.Timesheets.ServiceLayer.EfCore.Concrete;
using Payroll.Timesheets.ServiceLayer.EfCore.Queries;
using Payroll.Timesheets.ServiceLayer.TimeSystem.Adapter;
using Payroll.Timesheets.ServiceLayer.TimeSystem.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Main.FrontEnd.Wpf.Controller
{
    public class TimesheetDownloadController
    {
        #region Event Handler
        public delegate void PageDownloadHandler(object sender, int Page);
        public delegate void ProcessExceptionHandler(object sender, string errorMessage);

        public delegate void DownloadStartedHandler(object sender, int TotalPages);
        public delegate void EvaluationSucceededHandler(object sender, EvaluationResultArgs e);
        #endregion

        #region Event
        public event PageDownloadHandler? PageDownloadSucceeded;
        public event ProcessExceptionHandler? PageDownloadFailed;

        public event DownloadStartedHandler? DownloadStarted;
        public event EventHandler? DownloadCancelled;
        public event EventHandler? DownloadEnded;

        public event EventHandler? EvaluationStarted; // Checks if the downloaded timesheets are complete and if all employee data are present in the database.
        public event EvaluationSucceededHandler? EvaluationSucceeded;
        public event ProcessExceptionHandler? EvaluationFailed;// Please try again 
        #endregion

        private bool CancelPending { get; set; }
        public bool IsBusy { get; private set; }


        private readonly TimeDownloaderAdapter Adapter;
        private readonly TimesheetDbContext Context;

        public TimesheetDownloadController()
        {
            Context = new TimesheetDbContext();
            Adapter = TimeDownloaderFactory.CreateAdapter(Shared.Configuration);
        }

        public void Cancel() => CancelPending = true;

        public async Task StartDownload(DateTime cutoffDate, string payrollCode)
        {
            Cutoff cutoff = new Cutoff(cutoffDate);

            DownloadSummaryService service = new(Adapter);
            DownloadSummary<Timesheet> summary = await service.GetTimesheetSummary(cutoff.CutoffRange, payrollCode);
            if (summary is not null && IsBusy == false)
            {
                IsBusy = true;
                DownloadStarted?.Invoke(this, int.Parse(summary.TotalPage));

                foreach (int page in Enumerable.Range(0, int.Parse(summary.TotalPage) + 1).ToList())
                {
                    await DownloadPageContentAsync(cutoffDate, payrollCode, page);
                    if (CancelPending)
                    {
                        CancelPending = false;
                        DownloadCancelled?.Invoke(this, new EventArgs());
                        break;
                    }
                }
                EvaluateTimesheets(cutoffDate, payrollCode);
            }
            IsBusy = false;
            DownloadEnded?.Invoke(this, new EventArgs());

            // Call Evaluate Method.
        }

        public async Task StartDownload(DateTime cutoffDate, string payrollCode, int page)
        {
            if (IsBusy == false)
            {
                IsBusy = true;
                DownloadStarted?.Invoke(this, 1);

                await DownloadPageContentAsync(cutoffDate, payrollCode, page);

                IsBusy = false;
                DownloadEnded?.Invoke(this, new EventArgs());

            }
        }

        private async Task DownloadPageContentAsync(DateTime cutoffDate, string payrollCode, int page)
        {
            Cutoff cutoff = new Cutoff(cutoffDate);
            try
            {
                DownloadTimesheetService service = new(Adapter);
                DownloadContent<Timesheet>? timesheets = await service.DownloadTimesheets(cutoff.CutoffRange, payrollCode, page);
                if (timesheets is not null && timesheets.message is not null)
                {
                    SaveTimesheetBizLogic writeBizLogic = new(Context);
                    foreach (Timesheet timesheet in timesheets.message)
                        writeBizLogic.SaveTimesheet(timesheet, cutoff.CutoffId, payrollCode, page);

                    PageDownloadSucceeded?.Invoke(this, page);
                }
            }
            catch (Exception ex)
            {
                PageDownloadFailed?.Invoke(this, ex.Message);
            }
        }


        public void EvaluateTimesheets(DateTime cutoffDate, string payrollCode)
        {
            Cutoff cutoff = new(cutoffDate);
            EvaluationStarted?.Invoke(this, new EventArgs());
            EvaluationResultArgs args = new();
            try
            {
                TimesheetPageService PageService = new(Context);
                ListTimesheetsService ListingService = new(Context);
                args.NoEETimesheets = ListingService
                    .GetTimesheetNoEETimesheet(cutoff.CutoffId, payrollCode)
                    .Select(ts => new Employee() { EEId = ts.EEId })
                    .ToList();
                args.MissingPages = PageService.GetMissingPages(cutoff.CutoffId, payrollCode);
                
                args.Timesheets = ListingService.GetTimesheetsByCutoffId(cutoff.CutoffId, payrollCode).ToList();
                args.UnconfirmedTimesheetsWithAttendance = ListingService.GetTimesheetsByCutoffId(cutoff.CutoffId, payrollCode)
                    .Where(ts => !ts.IsConfirmed).ToList();
                args.UnconfirmedTimesheetsWithoutAttendance = ListingService.GetTimesheetsByCutoffId(cutoff.CutoffId, payrollCode)
                    .Where(ts => !ts.IsConfirmed && ts.TotalHours > 0).ToList();

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
        public List<Employee>? NoEETimesheets { get; set; }// Find on Server
        public List<int>? MissingPages { get; set; }// Re Download
        public List<Timesheet>? Timesheets { get; set; }// Include in Report
        public List<Timesheet>? UnconfirmedTimesheetsWithAttendance { get; set; }// Include in Report
        public List<Timesheet>? UnconfirmedTimesheetsWithoutAttendance { get; set; }// Include in Report
    }
}




