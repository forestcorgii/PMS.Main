using Pms.Employees.Domain;
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
    public class TimesheetStore :IStore
    {
        private string _cutoffId { get; set; }


        #region TIMESHEET
        private TimesheetModel _model;
        private readonly List<Timesheet> _timesheets;
        public IEnumerable<Timesheet> Timesheets { get; private set; }

        private Lazy<Task> _initializeLazy;
        public Action? Reloaded { get; set; }
        #endregion

        public TimesheetStore(TimesheetModel cutoffTimesheet)
        {
            // TIMESHEET
            _cutoffId = string.Empty;
            Timesheets = new List<Timesheet>();
            _timesheets = new List<Timesheet>();
            _initializeLazy = new Lazy<Task>(Initialize);
            _model = cutoffTimesheet;
        }

        public async Task Load()
        {
            try
            {
                await _initializeLazy.Value;
            }
            catch (Exception)
            {
                _initializeLazy = new Lazy<Task>(Initialize);
                throw;
            }
        }

        public async Task Reload()
        {
            _initializeLazy = new Lazy<Task>(Initialize);
            await Load();
        }

        private async Task Initialize()
        {
            IEnumerable<Timesheet> timesheets = new List<Timesheet>();
            await Task.Run(() =>
            {
                timesheets = _model.GetTimesheets(_cutoffId);
            });

            _timesheets.Clear();
            _timesheets.AddRange(timesheets);
            Reloaded?.Invoke();
        }

        public async void SetCutoffId(string cutoffId)
        {
            _cutoffId = cutoffId;
            await Reload();
        }

        public void SetPayrollCode(PayrollCode payrollCode)
        {
            Timesheets = _timesheets.Where(ts => ts.PayrollCode == payrollCode.PayrollCodeId);
            Reloaded?.Invoke();
        }
    }
}
