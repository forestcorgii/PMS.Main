using Pms.Masterlists.Domain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Pms.Main.FrontEnd.Common;
using Pms.Masterlists.FrontEnd.Stores;
using Pms.Masterlists.FrontEnd;
using Pms.Masterlists.FrontEnd.Commands;

namespace Pms.Masterlists.FrontEnd.ViewModels
{
    public class EmployeeListingVm : ViewModelBase
    {
        private MasterlistStore _store { get; set; }

        private string _filter;
        public string Filter
        {
            get => _filter;
            set => SetProperty(ref _filter, value);
        }

        private bool _includeArchived;
        public bool IncludeArchived
        {
            get => _includeArchived;
            set => SetProperty(ref _includeArchived, value);
        }

        private Models.Employees _employeeModel;

        private IEnumerable<Employee> _employees;
        private ObservableCollection<EmployeeDetailVm> _employeeDetailVms;
        public ObservableCollection<EmployeeDetailVm> EmployeeDetailVms
        {
            get => _employeeDetailVms;
            set => SetProperty(ref _employeeDetailVms, value);
        }

        public ICommand GotoDetail { get; }

        public EmployeeListingVm(MasterlistStore masterlistStore, Models.Employees employeeModel,
            NavigationStore navigationStore,
            NavigationService<EmployeeDetailVm> employeeDetailNavigation
        )
        {
            _employeeModel = employeeModel;

            GotoDetail = new NavigateCommand<EmployeeDetailVm>(employeeDetailNavigation);

            _store = masterlistStore;
            _store.Reloaded += _cutoffStore_EmployeesReloaded;

        }

        public override void Dispose()
        {
            _store.Reloaded -= _cutoffStore_EmployeesReloaded;
            base.Dispose();
        }

        private void _cutoffStore_EmployeesReloaded()
        {
            _employees = _store.Employees;
            EmployeeDetailVms = new ObservableCollection<EmployeeDetailVm>(_employees
                .Select(ee =>
                    new EmployeeDetailVm(ee, _employeeModel)
                )
                .ToList());
        }



        //public void ReloadFilter()
        //{
        //    EmployeeDetailVms = _employees
        //        .FilterPayrollCode(PayrollCode.PayrollCodeId)
        //        .IncludeArchived(IncludeArchived);
        //}

        //public void SetPayrollCode(PayrollCode payrollCode)
        //{
        //    PayrollCode = payrollCode;
        //    ReloadFilter();
        //}
    }


    static class EmployeeFilterExtension
    {

        public static IEnumerable<Employee> FilterPayrollCode(this IEnumerable<Employee> employees, string payrollCode)
        {
            if (payrollCode != string.Empty)
                return employees.Where(p => p.PayrollCode == payrollCode);
            return employees;
        }

        public static IEnumerable<Employee> FilterSearchInput(this IEnumerable<Employee> employees, string filter)
        {
            if (filter != string.Empty)
                employees = employees
                   .Where(ts =>
                       ts.EEId.Contains(filter) ||
                       ts.Fullname.Contains(filter) ||
                       ts.CardNumber.Contains(filter) ||
                       ts.AccountNumber.Contains(filter)
                   );

            return employees;
        }

        public static IEnumerable<Employee> IncludeArchived(this IEnumerable<Employee> employees, bool includeArchived)
        {
            if (includeArchived)
                return employees;
            else
                return employees.Where(ee => ee.Active == true);
        }
    }
}
