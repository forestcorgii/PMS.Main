using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Main.FrontEnd.Wpf.ViewModels
{
    public static class PayrollCodeParser
    {
        public static string Parse(string payroll_code)
        {
            string pCode = payroll_code.Split('-')[0].Replace("PAY", "P").Trim();

            if (pCode.Contains("K12AA")) { return "K12A"; }
            if (pCode.Contains("K12AT")) { return "K12"; }

            if (pCode.Contains("K12A")) { return "K12A"; }
            if (pCode.Contains("K12")) { return "K12"; }

            if (pCode.Contains("K13")) { return "K13"; }
            if (pCode.Contains("P1A")) { return "P1A"; }
            if (pCode.Contains("LP4A")) { return "LP4A"; }
            if (pCode.Contains("P4A")) { return "P4A"; }
            if (pCode.Contains("P5A")) { return "P5A"; }
            if (pCode.Contains("P7A")) { return "P7A"; }
            if (pCode.Contains("P10A")) { return "P10A"; }
            if (pCode.Contains("P11A")) { return "P11A"; }

            return "";
        }
    }
}
