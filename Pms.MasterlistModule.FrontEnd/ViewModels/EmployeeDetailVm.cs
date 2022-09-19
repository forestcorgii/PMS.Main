using CommunityToolkit.Mvvm.ComponentModel;
using Pms.Masterlists.Domain;
using Pms.MasterlistModule.FrontEnd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Pms.MasterlistModule.FrontEnd.ViewModels
{
    public class EmployeeDetailVm : ObservableObject
    {
        public Employee Employee { get; set; }

        public ICommand Save { get; set; }

        public EmployeeDetailVm(Employee employee, Models.Employees masterlistModel)
        {
            Employee = employee;
        }
    }
}
