using Pms.Timesheets.BizLogic;
using Pms.Timesheets.Domain;
using Pms.Timesheets.Domain.SupportTypes;
using Pms.Timesheets.ServiceLayer.EfCore;
using Pms.Timesheets.ServiceLayer.EfCore.Queries;
using Pms.Timesheets.ServiceLayer.TimeSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Main.FrontEnd.Wpf.Models
{
    public class CutoffTimesheet
    {
        private ITimesheetProvider _timesheetProvider;
        private ITimesheetPageProvider _pageProvider;
        private IDownloadContentProvider _downloadProvider;
        private ITimesheetSaving _timesheetSaving;

        public CutoffTimesheet(ITimesheetProvider timesheetProvider, ITimesheetPageProvider pageProvider, IDownloadContentProvider downloadProvider, ITimesheetSaving timesheetSaving)
        {
            _timesheetProvider = timesheetProvider;
            _pageProvider = pageProvider;
            _downloadProvider = downloadProvider;
            _timesheetSaving = timesheetSaving;
        }


        public IEnumerable<Timesheet> GetTimesheets(string cutoffId, string payrollCode,string bankCategory) =>
            _timesheetProvider.GetTimesheetsByCutoffId(cutoffId, payrollCode,bankCategory);

        public IEnumerable<Timesheet> GetTimesheets(string cutoffId, string payrollCode) =>
            _timesheetProvider.GetTimesheetsByCutoffId(cutoffId, payrollCode);

        public IEnumerable<Timesheet> GetTimesheets(string cutoffId) =>
                    _timesheetProvider.GetTimesheetsByCutoffId(cutoffId);

        public List<string> ListTimesheetCutoffIds() =>
                    _timesheetProvider.ListTimesheetCutoffIds();

        public List<string> ListTimesheetPayrollCodes() =>
                    _timesheetProvider.ListTimesheetPayrollCodes();

        public List<string> ListTimesheetBankCategories(string cutoffId, string payrollCode) =>
                    _timesheetProvider.ListTimesheetBankCategories(cutoffId, payrollCode);



        public IEnumerable<Timesheet> ListExportableTimesheets(string cutoffId, string payrollCode) =>
            _timesheetProvider
                .GetTimesheetsByCutoffId(cutoffId, payrollCode)
                .ByExportable()
                .ToArray();

        public IEnumerable<Timesheet> ListUnconfirmedTimesheetsWithAttendance(string cutoffId, string payrollCode) =>
            _timesheetProvider
                .GetTimesheetsByCutoffId(cutoffId, payrollCode)
                .ByUnconfirmedWithAttendance()
                .ToArray();

        public IEnumerable<Timesheet> ListUnconfirmedTimesheetsWithoutAttendance(string cutoffId, string payrollCode) =>
            _timesheetProvider
                .GetTimesheetsByCutoffId(cutoffId, payrollCode)
                .ByUnconfirmedWithoutAttendance()
                .ToArray();


        public IEnumerable<string> ListTimesheetNoEETimesheet(string cutoffId) =>
            _timesheetProvider.GetTimesheetNoEETimesheet(cutoffId)
                .Select(ts => ts.EEId)
                .ToList();

        public void SaveEmployeeData(Timesheet timesheet) =>
            _timesheetSaving.SaveTimesheetEmployeeData(timesheet);



        /// <summary>
        /// 
        /// </summary>
        /// <param name="payrollCode"></param>
        /// <returns>returns an array of pages to download.</returns>
        public int[] GetPageWithUnconfirmedTS(Cutoff cutoff, string payrollCode) =>
            _pageProvider.GetPageWithUnconfirmedTS(cutoff.CutoffId, payrollCode).ToArray();

        public int[] GetMissingPages(string cutoffId, string payrollCode) =>
            _pageProvider.GetMissingPages(cutoffId, payrollCode).ToArray();




        /// <summary>
        /// 
        /// </summary>
        /// <param name="payrollCode"></param>
        /// <returns>returns an array of pages to download.</returns>
        public async Task<int[]> DownloadContentSummary(Cutoff cutoff, string payrollCode,string site)
        {
            DownloadSummary<Timesheet> summary = await _downloadProvider.GetTimesheetSummary(cutoff.CutoffRange, payrollCode,site);
            return Enumerable.Range(0, int.Parse(summary.TotalPage) + 1).ToArray();
        }

        public async Task<IEnumerable<Timesheet>> DownloadContent(Cutoff cutoff, string payrollCode,string site, int page)
        {
            DownloadContent<Timesheet> rawTimesheets = await _downloadProvider.DownloadTimesheets(cutoff.CutoffRange, payrollCode, page, site);
            if (rawTimesheets is not null && rawTimesheets.message is not null)
            {
                foreach (Timesheet timesheet in rawTimesheets.message)
                    _timesheetSaving.SaveTimesheet(timesheet, cutoff.CutoffId, page);

                return rawTimesheets.message;
            }
            return new List<Timesheet>();
        }

    }
}
