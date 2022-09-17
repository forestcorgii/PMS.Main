using Pms.Adjustments.Domain.Models;
using Pms.Masterlists.Domain;
using Pms.Main.FrontEnd.Common;
using Pms.Main.FrontEnd.Government.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Main.FrontEnd.Government.Stores
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

        private MonthlyPayrollModel _payrollModel;
        private EmployeeModel _employeeModel;

        public MainStore(
            MonthlyPayrollModel monthlyPayrollModel,
            EmployeeModel employeeModel
        )
        {
            _payrollModel = monthlyPayrollModel;
            _employeeModel = employeeModel;

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
                cutoffIds =_payrollModel
                    .ListCutoffIds()
                    .OrderByDescending(c => c)
                    .ToArray();

                payrollCodes = _employeeModel.ListPayrollCodes();
                companies = _employeeModel.ListCompanies();
            });

            Companies = companies;
            CompanyIds = companies.Select(c => c.CompanyId);


            CutoffIds = cutoffIds;
            PayrollCodes = payrollCodes;
            Reloaded?.Invoke();
        }


        public void SetCutoff(Cutoff cutoff)
        {
            Cutoff = cutoff;

            //_billingStore.SetCutoffId(cutoff.CutoffId);
            //_payrollStore.SetCutoffId(cutoff.CutoffId);
        }

        public void SetPayrollCode(string payrollCodeId)
        {
            PayrollCode = PayrollCodes.Where(pc => pc.PayrollCodeId == payrollCodeId).First();

            //_employeeStore.SetPayrollCode(PayrollCode);
            //_billingStore.SetPayrollCode(PayrollCode);
            //_payrollStore.SetPayrollCode(PayrollCode);
        }
    }
}
