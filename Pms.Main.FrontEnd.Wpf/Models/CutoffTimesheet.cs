using Pms.Timesheets.BizLogic;
using Pms.Timesheets.Domain;
using Pms.Timesheets.Domain.SupportTypes;
using Pms.Timesheets.ServiceLayer.EfCore;
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
        private IProvideTimesheetService _timesheetProvider;
        private IDownloadContentProvider _downloadProvider;
        private ITimesheetSaving _timesheetSaving;

        public CutoffTimesheet(IProvideTimesheetService timesheetProvider,  IDownloadContentProvider downloadProvider, ITimesheetSaving timesheetSaving)
        {
            _timesheetProvider = timesheetProvider;
            _downloadProvider = downloadProvider;
            _timesheetSaving = timesheetSaving;
        } 

        public IEnumerable<Timesheet> GetTimesheets(string cutoffId) =>
                    _timesheetProvider.GetTimesheets().FilterByCutoffId(cutoffId);
         

        public IEnumerable<string> ListTimesheetNoEETimesheet(string cutoffId) =>
            _timesheetProvider.GetTimesheetNoEETimesheet(cutoffId)
                .Select(ts => ts.EEId)
                .ToList();

        public void SaveEmployeeData(Timesheet timesheet) =>
            _timesheetSaving.SaveTimesheetEmployeeData(timesheet);



        public int[] GetPageWithUnconfirmedTS(Cutoff cutoff, string payrollCode) =>
            _timesheetProvider.GetPageWithUnconfirmedTS(cutoff.CutoffId, payrollCode).ToArray();

        public int[] GetMissingPages(string cutoffId, string payrollCode) =>
            _timesheetProvider.GetMissingPages(cutoffId, payrollCode).ToArray();

         

        public async Task<int[]> DownloadContentSummary(Cutoff cutoff, string payrollCode, string site)
        {
            DownloadSummary<Timesheet> summary = await _downloadProvider.GetTimesheetSummary(cutoff.CutoffRange, payrollCode, site);
            return Enumerable.Range(0, int.Parse(summary.TotalPage) + 1).ToArray();
        }

        public async Task<IEnumerable<Timesheet>> DownloadContent(Cutoff cutoff, string payrollCode, string site, int page)
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
