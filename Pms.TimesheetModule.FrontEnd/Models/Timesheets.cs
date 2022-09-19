using Pms.Timesheets.Domain;
using Pms.Timesheets.Domain.SupportTypes;
using Pms.Timesheets.ServiceLayer.EfCore;
using Pms.Timesheets.ServiceLayer.TimeSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.TimesheetModule.FrontEnd.Models
{
    public class Timesheets
    {
        private IProvideTimesheetService _timesheetProvider;
        private IDownloadContentProvider _downloadProvider;
        private TimesheetManager _timesheetManager;

        public Timesheets(IProvideTimesheetService timesheetProvider, IDownloadContentProvider downloadProvider, TimesheetManager timesheetManager)
        {
            _timesheetProvider = timesheetProvider;
            _downloadProvider = downloadProvider;
            _timesheetManager= timesheetManager;
        }

        public IEnumerable<Timesheet> GetTimesheets(string cutoffId) =>
            _timesheetProvider.GetTimesheets(cutoffId);

        public IEnumerable<Timesheet> GetTwoPeriodTimesheets(string cutoffId) =>
            _timesheetProvider.GetTwoPeriodTimesheets(cutoffId);


        public IEnumerable<string> ListTimesheetNoEETimesheet(string cutoffId)
        {
            try
            {
                var da = _timesheetProvider.GetTimesheetNoEETimesheet(cutoffId)
                      .Select(ts => ts.EEId)
                      .ToList();
                return da;
            }
            catch (Exception ex)
            {
                Console.WriteLine();
            }
            return Enumerable.Empty<string>();
        }

        public void SaveEmployeeData(Timesheet timesheet) =>
            _timesheetManager.SaveTimesheetEmployeeData(timesheet);

        public void SaveTimesheet(Timesheet timesheet) =>
            _timesheetManager.SaveTimesheet(timesheet, timesheet.CutoffId, 0);



        public int[] GetPageWithUnconfirmedTS(Cutoff cutoff, string payrollCode) =>
            _timesheetProvider.GetPageWithUnconfirmedTS(cutoff.CutoffId, payrollCode).ToArray();

        public int[] GetMissingPages(string cutoffId, string payrollCode) =>
            _timesheetProvider.GetMissingPages(cutoffId, payrollCode).ToArray();



        public string[] ListPayrollCodes() =>
                _timesheetProvider.GetTimesheets().ExtractPayrollCodes().ToArray();

        public string[] ListCutoffIds() =>
                        _timesheetProvider.GetTimesheets().ExtractCutoffIds().ToArray();





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
                    _timesheetManager.SaveTimesheet(timesheet, cutoff.CutoffId, page);

                return rawTimesheets.message;
            }
            return new List<Timesheet>();
        }

    }
 
}
