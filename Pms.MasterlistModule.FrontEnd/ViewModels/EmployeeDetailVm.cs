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
using CommunityToolkit.Mvvm.Messaging;
using Pms.Main.FrontEnd.Common.Messages;
using Pms.MasterlistModule.FrontEnd.Commands.Employees_;

namespace Pms.MasterlistModule.FrontEnd.ViewModels
{
    public class EmployeeDetailVm : ObservableObject
    {
        private Employee employee;

        public Employee Employee { get => employee; set => SetProperty(ref employee, value); }

        public ObservableCollection<BankChoices> BankTypes =>
            new ObservableCollection<BankChoices>(Enum.GetValues(typeof(BankChoices)).Cast<BankChoices>());

        public ObservableCollection<string> Sites
        {
            get
            {
                List<string> sites = new();
                foreach (SiteChoices site in Enum.GetValues(typeof(SiteChoices)))
                    sites.Add(site.ToString());

                return new ObservableCollection<string>(sites);
            }
        }

        public string[] PayrollCodes { get; }

        public ICommand Save { get; set; }
        public ICommand Sync { get; set; }

        public event EventHandler OnRequestClose;

        public EmployeeDetailVm(Employee employee, Models.Employees employees)
        {
            Employee = employee;
            Save = new Save(this, employees);
            Sync = new SyncOne(this, employees);

            PayrollCodes = WeakReferenceMessenger.Default.Send<CurrentPayrollCodesRequestMessage>().Response;
        }

        public void Close() => OnRequestClose?.Invoke(this, new EventArgs());

        public void RefreshProperties()
        {
            OnPropertyChanged(nameof(Employee));
        }
    }
}
