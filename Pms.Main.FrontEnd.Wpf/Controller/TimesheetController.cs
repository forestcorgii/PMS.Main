using Payroll.Employees.ServiceLayer.Concrete;
using Payroll.Timesheets.Domain;
using Payroll.Timesheets.Domain.SupportTypes;
using Payroll.Timesheets.Persistence;
using Payroll.Timesheets.ServiceLayer.EfCore.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Main.FrontEnd.Wpf.Controller
{
    public class TimesheetController
    {
        private readonly TimesheetDbContext Context;

        public TimesheetController()
        {
            Context = new TimesheetDbContext();
        }

        public List<Timesheet> GetTimesheets()
        {
            ListTimesheetsService service = new(Context);
            return service.GetTimesheets().ToList();
        }

        public List<string> GetCutoffs()
        {
            ListTimesheetsService service = new(Context);
            return service.GetTimesheets().ToList()
                    .GroupBy(ts => ts.CutoffId)
                    .Select(ts => ts.First())
                    .Select(ts => new Cutoff(ts.CutoffId).CutoffId)
                    .ToList();
        }

    }
}
