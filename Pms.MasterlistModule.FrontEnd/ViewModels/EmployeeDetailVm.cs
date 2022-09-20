using CommunityToolkit.Mvvm.ComponentModel;
using Pms.Masterlists.Domain;
using Pms.MasterlistModule.FrontEnd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Pms.Masterlists.Domain.Entities.Employees;
using System.Collections.ObjectModel;
using Pms.Masterlists.Domain.Enums;
using Pms.MasterlistModule.FrontEnd.Commands;
using Pms.MasterlistModule.FrontEnd.Commands.Masterlists;

namespace Pms.MasterlistModule.FrontEnd.ViewModels
{
    public class EmployeeDetailVm : ObservableObject
    {
        private Employee employee;

        public Employee Employee { get => employee; set => SetProperty(ref employee, value); }

        public ObservableCollection<BankChoices> BankTypes =>
            new ObservableCollection<BankChoices>(Enum.GetValues(typeof(BankChoices)).Cast<BankChoices>());

        public ICommand Save { get; set; }

        public event EventHandler OnRequestClose;

        public EmployeeDetailVm(Employee employee, Models.Employees employees)
        {
            Employee = employee;
            Save = new Save(this, employees);
        }

        public void Close() => OnRequestClose?.Invoke(this, new EventArgs());

    }
}
