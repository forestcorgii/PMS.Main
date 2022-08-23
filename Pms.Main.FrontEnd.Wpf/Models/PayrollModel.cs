using Pms.Payrolls.Domain;
using Pms.Payrolls.Domain.Services;
using Pms.Payrolls.Domain.SupportTypes;
using Pms.Payrolls.ServiceLayer.Files.Exports;
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
        private readonly IImportPayrollService _importer;
        private readonly LBPExport _lbpExporter;
        private readonly ThirteenthMonthExport _13thMonthExporter;

        public PayrollModel(IManagePayrollService manager, IProvidePayrollService provider, IImportPayrollService importer, LBPExport lbpExporter, ThirteenthMonthExport __13thMonthExporter)
        {
            _manager = manager;
            _provider = provider;
            _importer = importer;
            _lbpExporter = lbpExporter;
            _13thMonthExporter = __13thMonthExporter;
        }

        public IEnumerable<Payroll> Get(string cutoffId, BankType bankType) =>
            _provider.GetPayrolls(cutoffId, bankType);


        public IEnumerable<Payroll> Get(int yearCovered, BankType bankType) =>
                    _provider.GetPayrolls(yearCovered, bankType);


        public string[] ListCutoffIds() =>
                        _provider.GetAllPayrolls().ExtractCutoffIds().ToArray();

        public IEnumerable<Payroll> Import(string payregFilePath) =>
             _importer.StartImport(payregFilePath);

        public void ExportLBP(IEnumerable<Payroll> payrolls, string cutoffId, string companyName) =>
            _lbpExporter.StartExport(payrolls, cutoffId, companyName);

        public void Export13thMonth(IEnumerable<ThirteenthMonth> thirteenthMonths, int year, BankType bankType) =>
            _13thMonthExporter.StartExport(thirteenthMonths, year, bankType);

        public IEnumerable<string> ListNoEEPayrolls() =>
            _provider.GetNoEEPayrolls().ExtractEEIds();

        internal void Save(Payroll payroll, string payrollCode, BankType bankType)
        {
            payroll.PayrollCode = payrollCode;
            payroll.Bank = bankType;
            _manager.SavePayroll(payroll);
        }
    }
}
