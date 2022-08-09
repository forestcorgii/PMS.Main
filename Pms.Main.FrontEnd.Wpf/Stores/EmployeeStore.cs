using Pms.Employees.Domain;
using Pms.Main.FrontEnd.Wpf.Models;
using Pms.Timesheets.Domain.SupportTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Main.FrontEnd.Wpf.Stores
{
    public class EmployeeStore
    {
        #region EMPLOYEE
        private EmployeeModel _employeeModel;
        private readonly List<Employee> _employees;
        public IEnumerable<Employee> Employees { get; private set; }
        private Lazy<Task> _initializeLazy;
        public event Action? EmployeesReloaded;
        #endregion

        public EmployeeStore(EmployeeModel employeeModel)
        {
            _employeeModel = employeeModel;
            _initializeLazy = new Lazy<Task>(Initialize);

            _employees = new List<Employee>();
            Employees = _employees;
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

            _employees.Clear();
            _employees.AddRange(employees);
            EmployeesReloaded?.Invoke();
        }



        public void SetPayrollCode(string payrollCode)
        {
            Employees = _employees.Where(ts => ts.PayrollCode == payrollCode);
            EmployeesReloaded?.Invoke();
        }
    }
}
