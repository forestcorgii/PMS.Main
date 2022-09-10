using Pms.Main.FrontEnd.Wpf.Models;
using Pms.Payrolls.Domain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Pms.Payrolls.Domain.Enums;

namespace Pms.Main.FrontEnd.Wpf.Stores
{
    public class PayrollStore : IStore
    {
        private string _cutoffId { get; set; } = string.Empty;
        private string _payrollCode = string.Empty;

        public BankChoices Bank = BankChoices.LBP;
        public string CompanyId = "";

        public IEnumerable<string> CompanyIds { get; set; }
        public IEnumerable<Company> Companies { get; set; }
        public IEnumerable<Payroll> _payrolls { get; set; }

        public ObservableCollection<Payroll> Payrolls;
        public ImportProcessChoices Process = ImportProcessChoices.PD;

        public IEnumerable<Payroll> PayrollsSetter { set => Payrolls = new ObservableCollection<Payroll>(value); }


        private readonly PayrollModel _model;

        public Lazy<Task> _initializeLazy { get; set; }
        public Action? Reloaded { get; set; }

        public PayrollStore(PayrollModel model)
        {
            _initializeLazy = new Lazy<Task>(Initialize);

            _payrolls = new List<Payroll>();
            Payrolls = new ObservableCollection<Payroll>();
            _model = model;

            CompanyIds = new List<string>();
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
            }
        }

        public async Task Reload()
        {
            _initializeLazy = new Lazy<Task>(Initialize);
            await _initializeLazy.Value;
        }


        private async Task Initialize()
        {
            IEnumerable<Payroll> payrolls = new List<Payroll>();
            IEnumerable<Company> companies = new List<Company>();
            await Task.Run(() =>
            {
                payrolls = _model.Get(_cutoffId);
                companies = _model.ListCompanies();
            });

            _payrolls = payrolls;
            PayrollsSetter = payrolls
                .SetPayrollCode(_payrollCode)
                .SetCompanyId(CompanyId);

            Companies = companies;
            CompanyIds = companies.Select(c => c.CompanyId);

            Reloaded?.Invoke();
        }


        public void ReloadFilter()
        {
            PayrollsSetter = _payrolls
                .SetPayrollCode(_payrollCode)
                .SetCompanyId(CompanyId);

            Reloaded?.Invoke();
        }


        public async void SetCutoffId(string cutoffId)
        {
            _cutoffId = cutoffId;
            await Reload();
        }

        public void SetPayrollCode(string payrollCode)
        {
            _payrollCode = payrollCode;
            ReloadFilter();
        }
    }

    static class PayrollFilterExtension
    {
        //public static IEnumerable<Payroll> SetBankType(this IEnumerable<Payroll> payrolls, BankChoices bank) =>
        //    payrolls.Where(p => p.Bank == bank);

        public static IEnumerable<Payroll> SetCompanyId(this IEnumerable<Payroll> payrolls, string companyId)
        {
            if (companyId != string.Empty)
                return payrolls.Where(p => p.CompanyId == companyId);
            return payrolls;
        }
        public static IEnumerable<Payroll> SetPayrollCode(this IEnumerable<Payroll> payrolls, string payrollCode)
        {
            if (payrollCode != string.Empty)
                return payrolls.Where(p => p.PayrollCode == payrollCode);
            return payrolls;
        }
    }
}
