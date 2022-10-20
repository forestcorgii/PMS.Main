using Pms.Adjustments.Domain;
using Pms.Adjustments.ServiceLayer.EfCore.Table_Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.AdjustmentModule.FrontEnd.Models
{
    public class Employees
    {
        private EmployeeViewProvider Provider;

        public Employees(EmployeeViewProvider provider) =>
            Provider = provider;


        public EmployeeView Find(string eeId) => Provider.FindEmployee(eeId);
    }
}
