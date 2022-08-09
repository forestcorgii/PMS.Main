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
using static Pms.Timesheets.ServiceLayer.TimeSystem.Services.Enums;

namespace Pms.Main.FrontEnd.Wpf.ViewModel
{
    public class DownloadTimesheetsViewModel
    {
        #region Event Handler
        public delegate void PageDownloadHandler(object sender, int Page);
        public delegate void ProcessExceptionHandler(object sender, string errorMessage);

        public delegate void DownloadStartedHandler(object sender, int TotalPages);
        #endregion

        #region Event
        public event PageDownloadHandler? PageDownloadSucceeded;
        public event ProcessExceptionHandler? PageDownloadFailed;

        public event DownloadStartedHandler? DownloadStarted;
        public event EventHandler? DownloadEnded;
        #endregion

        public bool IsBusy { get; private set; }

        private readonly TimeDownloaderAdapter Adapter;
        private readonly TimesheetDbContext Context;

        public Cutoff Cutoff { get; private set; }

        public string PayrollCode { get; private set; }

        public DownloadTimesheetsViewModel(Cutoff cutoff, string payrollCode)
        {
            //Context = new TimesheetDbContext();
            Adapter = TimeDownloaderFactory.CreateAdapter(Shared.Configuration);

            Cutoff = cutoff;
            PayrollCode = payrollCode;
        }


        public async Task StartDownload(DownloadOptions options)
        {
            if (IsBusy == false)
            {
                IsBusy = true;

                //TimesheetPageService pageService = new(Context);

                //List<int> pages;
                //if (options == DownloadOptions.UnconfirmedOnly)
                //    pages = pageService.GetPageWithUnconfirmedTS(Cutoff.CutoffId, PayrollCode);
                //else
                //{
                //    DownloadSummaryService service = new(Adapter);
                //    DownloadSummary<Timesheet> summary = await service.GetTimesheetSummary(Cutoff.CutoffRange, PayrollCode);
                //    pages = Enumerable.Range(0, int.Parse(summary.TotalPage) + 1).ToList();
                //}

                //DownloadStarted?.Invoke(this, pages.Count);

                //foreach (int page in pages)
                //    await DownloadPageContentAsync(page);

            }
            IsBusy = false;
            DownloadEnded?.Invoke(this, new EventArgs());

            // Call Evaluate Method.
        }

        public async Task StartDownload(int page)
        {
            if (IsBusy == false)
            {
                IsBusy = true;
                DownloadStarted?.Invoke(this, 1);

                await DownloadPageContentAsync(page);

                IsBusy = false;
                DownloadEnded?.Invoke(this, new EventArgs());

            }
        }

        public async Task StartDownload(int[] pages)
        {
            if (IsBusy == false)
            {
                IsBusy = true;
                DownloadStarted?.Invoke(this, 1);

                foreach (int page in pages)
                {
                    await DownloadPageContentAsync(page);
                }

                IsBusy = false;
                DownloadEnded?.Invoke(this, new EventArgs());

            }
        }

        private async Task DownloadPageContentAsync(int page)
        {
            try
            {
                string site = "MANILA";
                if (PayrollCode[0] == 'L') site = "LEYTE";

                //DownloadTimesheetService service = new(Adapter);
                //DownloadContent<Timesheet> timesheets = await service.DownloadTimesheets(Cutoff.CutoffRange, PayrollCode, page, site);
                //if (timesheets is not null && timesheets.message is not null)
                //{
                //    SaveTimesheetBizLogic writeBizLogic = new(Context);
                //    foreach (Timesheet timesheet in timesheets.message)
                //        writeBizLogic.SaveTimesheet(timesheet, Cutoff.CutoffId, page);

                //    PageDownloadSucceeded?.Invoke(this, page);
                //}
            }
            catch (Exception ex)
            {
                PageDownloadFailed?.Invoke(this, ex.Message);
            }
        }



    }

}




