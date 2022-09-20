using Pms.Payrolls.Domain;
using Pms.Payrolls.ServiceLayer.EfCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.PayrollModule.FrontEnd.Models
{
    public class MonthlyPayrolls
    {
        readonly PayrollProvider _provider;
        public MonthlyPayrolls(PayrollProvider provider)
        {
            _provider = provider;
        }


        public IEnumerable<Payroll[]> GetMonthlyPayrolls(int month, string payrollCode)
        {
            IEnumerable<Payroll> payrolls = _provider.GetMonthlyPayrolls(month, payrollCode)
                .OrderBy(p => p.CutoffId);

            return payrolls
                .GroupBy(p => p.EEId)
                .Select(ps => ps.ToArray());
        }



        public string[] ListCutoffIds() =>
            _provider.GetAllPayrolls().ExtractCutoffIds().ToArray();

        public string[] ListPayrollCodes() =>
            _provider.GetAllPayrolls().ExtractPayrollCodes().ToArray();

    }
}
