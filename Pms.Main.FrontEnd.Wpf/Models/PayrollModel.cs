using Pms.Employees.Domain;
using Pms.Payrolls.Domain;
using Pms.Payrolls.Domain.Services;
using Pms.Payrolls.Domain.SupportTypes;
using Pms.Payrolls.ServiceLayer.Files;
using Pms.Payrolls.ServiceLayer.Files.Exports;
using Pms.Payrolls.ServiceLayer.Files.Exports.Bank_Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Pms.Payrolls.Domain.Enums;
using static Pms.Payrolls.ServiceLayer.EfCore.PayrollProviderExtensions;

namespace Pms.Main.FrontEnd.Wpf.Models
{
    public class PayrollModel
    {
        private readonly IManagePayrollService _manager;
        private readonly IProvidePayrollService _provider;

        public PayrollModel(IManagePayrollService manager, IProvidePayrollService provider)
        {
            _manager = manager;
            _provider = provider;
        }

        public IEnumerable<Payroll> Get(string cutoffId) =>
            _provider.GetPayrolls(cutoffId);

        public IEnumerable<Payroll> Get(string cutoffId, string payrollCode) =>
                    _provider.GetPayrolls(cutoffId, payrollCode);

        public IEnumerable<Payroll> Get(int yearCovered, string companyId) =>
            _provider.GetPayrolls(yearCovered, companyId);


        public string[] ListCutoffIds() =>
            _provider.GetAllPayrolls().ExtractCutoffIds().ToArray();

        public string[] ListPayrollCodes() =>
            _provider.GetAllPayrolls().ExtractPayrollCodes().ToArray();

        public IEnumerable<Payroll> Import(string payregFilePath, ImportProcessChoices processType)
        {
            PayrollRegisterImportBase importer = new(processType);
            return importer.StartImport(payregFilePath);
        }

        public void ExportBankReport(IEnumerable<Payroll> payrolls, string cutoffId, string payrollCode)
        {
            BankReportBase exporter = new(cutoffId, payrollCode);
            exporter.StartExport(payrolls.Where(p => p.NetPay > 0.01));
        }

        public void ExportAlphalist(IEnumerable<AlphalistDetail> alphalists, int year, Company company)
        {
            AlphalistExporter exporter = new();
            exporter.StartExport(alphalists, year, company.CompanyId, company.MinimumRate);
        }

        public void ExportAlphalistVerifier(IEnumerable<IEnumerable<Payroll>> employeePayrolls, int year, Company company)
        {
            AlphalistVerifierExporter exporter = new();
            exporter.StartExport(employeePayrolls, year, company.CompanyId);
        }

        public IEnumerable<string> ListNoEEPayrolls() =>
            _provider.GetNoEEPayrolls().ExtractEEIds();

        internal void Save(Payroll payroll, string payrollCode, string companyId)
        {
            payroll.PayrollCode = payrollCode;
            payroll.CompanyId = companyId;
            _manager.SavePayroll(payroll);
        }
    }
}
