using Payroll.Timesheets.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Main.FrontEnd.Wpf.Controller
{
    public class TimesheetOutputController
    {
        private readonly TimesheetDbContext Context;

        public TimesheetOutputController()
        {
            Context = new TimesheetDbContext();
        }




    }
}
