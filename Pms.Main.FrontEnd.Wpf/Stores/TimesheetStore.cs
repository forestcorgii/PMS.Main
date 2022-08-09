using Pms.Main.FrontEnd.Wpf.Models;
using Pms.Timesheets.Domain;
using Pms.Timesheets.Domain.SupportTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Main.FrontEnd.Wpf.Stores
{
    public class TimesheetStore
    {
        private string _cutoffId { get; set; }
        private string _payrollCode { get; set; } = "";


        #region TIMESHEET
        private CutoffTimesheet _cutoffTimesheet;
        private readonly List<Timesheet> _timesheets;
        public IEnumerable<Timesheet> Timesheets { get; private set; }
        private Lazy<Task> _initializeLoadTimesheetsLazy;
        public event Action? TimesheetsReloaded;
        #endregion

        public TimesheetStore(CutoffTimesheet cutoffTimesheet)
        {
            // TIMESHEET
            _timesheets = new List<Timesheet>();
            _initializeLoadTimesheetsLazy = new Lazy<Task>(Initialize);
            _cutoffTimesheet = cutoffTimesheet;
        }

        public async Task Load()
        {
            try
            {
                await _initializeLoadTimesheetsLazy.Value;
            }
            catch (Exception)
            {
                _initializeLoadTimesheetsLazy = new Lazy<Task>(Initialize);
                throw;
            }
        }

        public async Task Reload()
        {
            _initializeLoadTimesheetsLazy = new Lazy<Task>(Initialize);
            await _initializeLoadTimesheetsLazy.Value;
        }

        private async Task Initialize()
        {
            IEnumerable<Timesheet> timesheets = new List<Timesheet>();
            await Task.Run(() =>
            {
                timesheets = _cutoffTimesheet.GetTimesheets(_cutoffId);
            });

            _timesheets.Clear();
            _timesheets.AddRange(timesheets);
            TimesheetsReloaded?.Invoke();
        }

        public async void SetCutoffId(string cutoffId)
        {
            _cutoffId = cutoffId;
            await Reload();
        }

        public void SetPayrollCode(string payrollCode)
        {
            _payrollCode = payrollCode;
            Timesheets = _timesheets.Where(ts => ts.PayrollCode == payrollCode);
            TimesheetsReloaded?.Invoke();
        }


    }
}
