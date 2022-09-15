using Pms.Masterlists.Domain;
using Pms.Main.FrontEnd.Wpf.Models;
using Pms.Timesheets.Domain.SupportTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Main.FrontEnd.Wpf.Stores
{
    public class MasterlistStore : IStore
    {
        private PayrollCode _payrollCode { get; set; }

        public bool IncludeArchived;

        public string Filter = string.Empty;

        private MasterlistModel _employeeModel;
        private IEnumerable<Employee> _employees;
        public IEnumerable<Employee> Employees { get; private set; }
        private Lazy<Task> _initializeLazy;

        public Action? Reloaded { get; set; }

        public MasterlistStore(MasterlistModel employeeModel)
        {
            _employeeModel = employeeModel;
            _initializeLazy = new Lazy<Task>(Initialize);

            _employees = new List<Employee>();
            Employees = _employees;

            _payrollCode = new();
        }


        public async Task Load()
        {
            try
            {
                await _initializeLazy.Value;
            }
            catch (Exception)
            {
                _initializeLazy = new Lazy<Task>(Initialize);
                throw;
            }
        }

        public async Task Reload()
        {
            _initializeLazy = new Lazy<Task>(Initialize);
            await _initializeLazy.Value;
        }

        private async Task Initialize()
        {
            IEnumerable<Employee> employees = new List<Employee>();
            await Task.Run(() =>
            {
                employees = _employeeModel.GetEmployees();
            });


            _employees = employees;

            ReloadFilter();

            Reloaded?.Invoke();
        }


        public void ReloadFilter()
        {
            Employees = _employees
                .FilterPayrollCode(_payrollCode.PayrollCodeId)
                .IncludeArchived(IncludeArchived)
                .FilterSearchInput(Filter);

            Reloaded?.Invoke();
        }

        public void SetPayrollCode(PayrollCode payrollCode)
        {
            _payrollCode = payrollCode;
            ReloadFilter();
        }

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
