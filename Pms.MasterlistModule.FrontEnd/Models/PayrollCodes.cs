using Pms.Masterlists.Domain;
using Pms.Masterlists.ServiceLayer;
using Pms.Masterlists.ServiceLayer.EfCore;
using Pms.Masterlists.ServiceLayer.Files;
using Pms.Masterlists.ServiceLayer.HRMS;
using Pms.Masterlists.ServiceLayer.HRMS.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.MasterlistModule.FrontEnd.Models
{
    public class PayrollCodes
    {
        PayrollCodeManager _payrollCodeManager;

        public PayrollCodes(PayrollCodeManager payrollCodeManager)=>
            _payrollCodeManager = payrollCodeManager;


        public IEnumerable<PayrollCode> ListPayrollCodes() =>
            _payrollCodeManager.GetPayrollCodes().ToList();

        public void Save(PayrollCode payrollCode) =>
            _payrollCodeManager.SavePayrollCode(payrollCode);
    }
}
