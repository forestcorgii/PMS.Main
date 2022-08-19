using Pms.Payrolls.Domain;
using Pms.Payrolls.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Pms.Payrolls.Domain.Enums;

namespace Pms.Main.FrontEnd.Wpf.Models
{
    public class PayrollModel
    {
        private IManagePayrollService _manager;
        private IProvidePayrollService _provider;
        private IImportPayrollService _importer;

        public PayrollModel(IManagePayrollService manager, IProvidePayrollService provider, IImportPayrollService importer)
        {
            _manager = manager;
            _provider = provider;
            _importer = importer;
        }

        public IEnumerable<Payroll> Get(string cutoffId, BankType bankType)
        {
            return _provider.GetPayrolls(cutoffId, bankType);
        }


        public IEnumerable<Payroll> ImportPayroll(string payregFilePath) =>
             _importer.StartImport(payregFilePath);

        internal void Save(Payroll payroll)
        {
            _manager.SavePayroll(payroll);
        }
    }
}
