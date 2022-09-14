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
        public PayrollCode PayrollCode { get; set; }

        public string Site => PayrollCode.Site;

        public IEnumerable<string> CompanyIds { get; set; }
        public IEnumerable<Company> Companies { get; set; }
        public IEnumerable<PayrollCode> PayrollCodes { get; set; }

        public string[] CutoffIds { get; private set; }

        public Action? Reloaded { get; set; }
        private Lazy<Task> _initializeLazy;
        #endregion

        private EmployeeModel _employeeModel;
        private TimesheetModel _timesheetModel;
        private PayrollModel _payrollModel;

        private readonly TimesheetStore _timesheetStore;
        private readonly EmployeeStore _employeeStore;
        private readonly BillingStore _billingStore;
        private readonly PayrollStore _payrollStore;


        public MainStore(
            TimesheetModel timesheetModel,
            PayrollModel payrollModel,
            EmployeeModel employeeModel,
            TimesheetStore timesheetStore,
            EmployeeStore employeeStore,
            BillingStore billingStore,
            PayrollStore payrollStore
        )
        {
            _timesheetStore = timesheetStore;
            _employeeStore = employeeStore;
            _billingStore = billingStore;
            _payrollStore = payrollStore;

            _employeeModel = employeeModel;
            _payrollModel = payrollModel;
            _timesheetModel = timesheetModel;

            Cutoff = new Cutoff();
            CutoffIds = new string[] { };
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
            IEnumerable<PayrollCode> payrollCodes = new List<PayrollCode>();
            IEnumerable<Company> companies = new List<Company>();
            await Task.Run(() =>
            {
                cutoffIds = _timesheetModel
                    .ListCutoffIds()
                    .Union(_payrollModel.ListCutoffIds())
                    .OrderByDescending(c => c)
                    .ToArray();

                payrollCodes = _employeeModel.ListPayrollCodes();
                companies = _employeeModel.ListCompanies();
            });

            Companies = companies;
            CompanyIds = companies.Select(c => c.CompanyId);


            CutoffIds = cutoffIds;
            PayrollCodes = payrollCodes;
            //PayrollCodes = payrollCodes;
            Reloaded?.Invoke();
        }


        public void SetCutoff(Cutoff cutoff)
        {
            Cutoff = cutoff;

            _timesheetStore.SetCutoffId(cutoff.CutoffId);
            _billingStore.SetCutoffId(cutoff.CutoffId);
            _payrollStore.SetCutoffId(cutoff.CutoffId);
        }

        public void SetPayrollCode(string payrollCodeId)
        {
            PayrollCode = PayrollCodes.Where(pc => pc.PayrollCodeId == payrollCodeId).First();
            Cutoff.SetSite(Site);

            _timesheetStore.SetPayrollCode(PayrollCode);
            _employeeStore.SetPayrollCode(PayrollCode);
            _billingStore.SetPayrollCode(PayrollCode);
            _payrollStore.SetPayrollCode(PayrollCode);
        }
    }
}
