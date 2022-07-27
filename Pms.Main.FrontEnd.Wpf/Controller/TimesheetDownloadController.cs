using Pms.Employees.Domain;
using Pms.Timesheets.BizLogic.Concrete;
using Pms.Timesheets.Domain;
using Pms.Timesheets.Domain.SupportTypes;
using Pms.Timesheets.Persistence;
using Pms.Timesheets.ServiceLayer.EfCore.Concrete;
using Pms.Timesheets.ServiceLayer.EfCore.Queries;
using Pms.Timesheets.ServiceLayer.TimeSystem.Adapter;
using Pms.Timesheets.ServiceLayer.TimeSystem.Services;
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

        public async Task StartDownload(Cutoff cutoff, string payrollCode)
        {
            DownloadSummaryService service = new(Adapter);
            DownloadSummary<Timesheet> summary = await service.GetTimesheetSummary(cutoff.CutoffRange, payrollCode);
            if (summary is not null && IsBusy == false)
            {
                IsBusy = true;
                DownloadStarted?.Invoke(this, int.Parse(summary.TotalPage));

                foreach (int page in Enumerable.Range(0, int.Parse(summary.TotalPage) + 1).ToList())
                {
                    await DownloadPageContentAsync(cutoff.CutoffDate, payrollCode, page);
                    if (CancelPending)
                    {
                        CancelPending = false;
                        DownloadCancelled?.Invoke(this, new EventArgs());
                        break;
                    }
                }
                EvaluateTimesheets(cutoff.CutoffDate, payrollCode);
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

        public async Task StartDownload(DateTime cutoffDate, string payrollCode, int[] pages)
        {
            if (IsBusy == false)
            {
                IsBusy = true;
                DownloadStarted?.Invoke(this, 1);

                foreach (int page in pages)
                {
                    await DownloadPageContentAsync(cutoffDate, payrollCode, page);
                    if (CancelPending)
                    {
                        CancelPending = false;
                        DownloadCancelled?.Invoke(this, new EventArgs());
                        break;
                    }
                }

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
                DownloadContent<Timesheet> timesheets = await service.DownloadTimesheets(cutoff.CutoffRange, payrollCode, page);
                if (timesheets is not null && timesheets.message is not null)
                {
                    SaveTimesheetBizLogic writeBizLogic = new(Context);
                    foreach (Timesheet timesheet in timesheets.message)
                        writeBizLogic.SaveTimesheet(timesheet, cutoff.CutoffId, page);

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
                    .GetTimesheetNoEETimesheet(cutoff.CutoffId)
                    .Select(ts => ts.EEId)
                    .ToList();

                args.MissingPages = PageService.GetMissingPages(cutoff.CutoffId, payrollCode);
                ListingService.GetTimesheets().ToList().Where(ts => ts.EE == null).ToList();

                args.Timesheets = ListingService.GetTimesheetsByCutoffId(cutoff.CutoffId, payrollCode)
                    .Where(ts => ts.IsConfirmed && ts.TotalHours > 0).ToList();

                args.UnconfirmedTimesheetsWithAttendance = ListingService.GetTimesheetsByCutoffId(cutoff.CutoffId, payrollCode)
                    .Where(ts => !ts.IsConfirmed && ts.TotalHours > 0).ToList();
                args.UnconfirmedTimesheetsWithoutAttendance = ListingService.GetTimesheetsByCutoffId(cutoff.CutoffId, payrollCode)
                    .Where(ts => !ts.IsConfirmed && ts.TotalHours == 0).ToList();

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




