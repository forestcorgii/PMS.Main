using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Main.FrontEnd.Wpf.ViewModels
{
    public static class PayrollCodeValidator
    {
        public static bool Validate(string payrollCode)
        {
            if (payrollCode.Contains("PAY")) { return false; }
            
            return true;
        }
    }
}
