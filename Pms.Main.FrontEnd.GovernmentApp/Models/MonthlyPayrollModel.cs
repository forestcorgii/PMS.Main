using Pms.Main.FrontEnd.Government.ViewModels;
using Pms.Payrolls.Domain;
using Pms.Payrolls.ServiceLayer.EfCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Main.FrontEnd.Government.Models
{
    public class MonthlyPayrollModel
    {
        readonly PayrollProvider _provider;
        public MonthlyPayrollModel(PayrollProvider provider)
        {
            _provider = provider;
        }


        public IEnumerable<PayrollDetailViewModel> GetMonthlyPayrolls(int month, string payrollCode)
        {
            IEnumerable<Payroll> payrolls = _provider.GetMonthlyPayrolls(month, payrollCode)
                .OrderBy(p => p.CutoffId);

            return payrolls
                .GroupBy(p => p.EEId)
                .Select(ps => ps.ToArray())
                .Select(p => new PayrollDetailViewModel(p));
        }



        public string[] ListCutoffIds() =>
            _provider.GetAllPayrolls().ExtractCutoffIds().ToArray();

        public string[] ListPayrollCodes() =>
            _provider.GetAllPayrolls().ExtractPayrollCodes().ToArray();

    }
}
