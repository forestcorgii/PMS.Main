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
        private Lazy<Task> _initializeLoadFiltersLazy;
        #endregion

        private TimesheetModel _cutoffTimesheet;

        private readonly TimesheetStore _timesheetStore;
        private readonly EmployeeStore _employeeStore;
        private readonly BillingStore _billingStore;


        public MainStore(TimesheetModel cutoffTimesheet, TimesheetStore timesheetStore, EmployeeStore employeeStore, BillingStore billingStore)
        {
            _timesheetStore = timesheetStore;
            _employeeStore = employeeStore;
            _billingStore = billingStore;

            _cutoffTimesheet = cutoffTimesheet;

            Cutoff = new Cutoff();
            CutoffIds = new string[] { };
            PayrollCodes = new string[] { };
            _initializeLoadFiltersLazy = new Lazy<Task>(InitializeLoadFilters);
        }

        public async Task Load()
        {
            try
            {
                await _initializeLoadFiltersLazy.Value;
            }
            catch (Exception)
            {
                _initializeLoadFiltersLazy = new Lazy<Task>(InitializeLoadFilters);
                throw;
            }
        }

        public async Task InitializeLoadFilters()
        {
            string[] cutoffIds = new string[] { };
            string[] payrollCodes = new string[] { };
            await Task.Run(() =>
            {
                cutoffIds = _cutoffTimesheet.ListCutoffIds();
                payrollCodes = _cutoffTimesheet.ListPayrollCodes();
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
        }

        public void SetPayrollCode(string payrollCode)
        {
            PayrollCode = payrollCode;
            Cutoff.SetSite(Site);
            _timesheetStore.SetPayrollCode(payrollCode);
            _employeeStore.SetPayrollCode(payrollCode);
            _billingStore.SetPayrollCode(payrollCode);
        }

    }
}
