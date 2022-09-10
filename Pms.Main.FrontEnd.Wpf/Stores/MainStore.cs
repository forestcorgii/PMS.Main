using Pms.Employees.Domain;
using Pms.Main.FrontEnd.Wpf.Models;
using Pms.Timesheets.Domain;
using Pms.Timesheets.Domain.SupportTypes;
using Pms.Timesheets.ServiceLayer.EfCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Main.FrontEnd.Wpf.Stores
{
    public class MainStore : IStore
    {
        #region MAIN
        public Cutoff Cutoff { get; private set; }
        public string PayrollCode { get; private set; } = "";
        public string Site
        {
            get
            {
                if ((PayrollCode != "" && PayrollCode[0] == 'L') || PayrollCode == "P4A")
                    return "LEYTE";
                else
                    return "MANILA";
            }
        }

        public string[] CutoffIds { get; private set; }
        public string[] PayrollCodes { get; private set; }
        public Action? Reloaded { get; set; }
        private Lazy<Task> _initializeLazy;
        #endregion

        private TimesheetModel _timesheet;
        private PayrollModel _payroll;

        private readonly TimesheetStore _timesheetStore;
        private readonly EmployeeStore _employeeStore;
        private readonly BillingStore _billingStore;
        private readonly PayrollStore _payrollStore;


        public MainStore(TimesheetModel timesheet, PayrollModel payroll, TimesheetStore timesheetStore, EmployeeStore employeeStore, BillingStore billingStore, PayrollStore payrollStore)
        {
            _timesheetStore = timesheetStore;
            _employeeStore = employeeStore;
            _billingStore = billingStore;
            _payrollStore = payrollStore;

            _timesheet = timesheet;
            _payroll = payroll;

            Cutoff = new Cutoff();
            CutoffIds = new string[] { };
            PayrollCodes = new string[] { };
            _initializeLazy = new Lazy<Task>(Initialize);
        }

        public async Task Load()
        {
            try
            {
                await _initializeLazy.Value;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                _initializeLazy = new Lazy<Task>(Initialize);
                throw;
            }
        }
        public async Task Reload()
        {
            _initializeLazy = new Lazy<Task>(Initialize);
            await Load();
        }


        public async Task Initialize()
        {
            string[] cutoffIds = new string[] { };
            string[] payrollCodes = new string[] { };
            await Task.Run(() =>
            {
                cutoffIds = _timesheet
                    .ListCutoffIds()
                    .Union(_payroll.ListCutoffIds())
                    .OrderByDescending(c => c)
                    .ToArray();
                payrollCodes = _timesheet
                    .ListPayrollCodes()
                    .Union(_payroll.ListPayrollCodes())
                    .OrderBy(c => c)
                    .ToArray();
            });

            CutoffIds = cutoffIds;
            PayrollCodes = payrollCodes;
            Reloaded?.Invoke();
        }


        public void SetCutoff(Cutoff cutoff)
        {
            Cutoff = cutoff;

            _timesheetStore.SetCutoffId(cutoff.CutoffId);
            _billingStore.SetCutoffId(cutoff.CutoffId);
            _payrollStore.SetCutoffId(cutoff.CutoffId);
        }

        public void SetPayrollCode(string payrollCode)
        {
            PayrollCode = payrollCode;
            Cutoff.SetSite(Site);

            _timesheetStore.SetPayrollCode(payrollCode);
            _employeeStore.SetPayrollCode(payrollCode);
            _billingStore.SetPayrollCode(payrollCode);
            _payrollStore.SetPayrollCode(payrollCode);
        }
    }
}
