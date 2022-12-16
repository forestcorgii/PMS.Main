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
        private TimesheetProvider _timesheetProvider;
        private IDownloadContentProvider _downloadProvider;
        private TimesheetManager _timesheetManager;

        public Timesheets(TimesheetProvider timesheetProvider, IDownloadContentProvider downloadProvider, TimesheetManager timesheetManager)
        {
            _timesheetProvider = timesheetProvider;
            _downloadProvider = downloadProvider;
            _timesheetManager = timesheetManager;
        }

        public EmployeeView FindEmployeeView(string eeId) =>
            _timesheetProvider.FindEmployeeView(eeId);

        public IEnumerable<Timesheet> MapEmployeeView(Timesheet[] timesheets)
        {
            List<Timesheet> mappedTimesheets = new();

            foreach (Timesheet timesheet in timesheets)
            {
                timesheet.EE = _timesheetProvider.FindEmployeeView(timesheet.EEId);
                mappedTimesheets.Add(timesheet);
            }

            return mappedTimesheets;
        }


        public IEnumerable<Timesheet> GetTimesheets(string cutoffId) =>
            _timesheetProvider.GetTimesheets(cutoffId);

        public IEnumerable<Timesheet> GetTimesheets(string cutoffId, string payrollCodeId) =>
                    _timesheetProvider.GetTimesheets(cutoffId, payrollCodeId);

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





        public async Task<DownloadSummary<Timesheet>> DownloadContentSummary(Cutoff cutoff, string payrollCode, string site) =>
             await _downloadProvider.GetTimesheetSummary(cutoff.CutoffRange, payrollCode, site);

        public async Task<IEnumerable<Timesheet>> DownloadContent(Cutoff cutoff, string payrollCodeName, string site, int page)
        {
            DownloadContent<Timesheet> rawTimesheets = await _downloadProvider.DownloadTimesheets(cutoff.CutoffRange, payrollCodeName, page, site);
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
