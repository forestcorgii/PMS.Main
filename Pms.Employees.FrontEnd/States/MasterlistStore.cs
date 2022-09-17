using Pms.Main.FrontEnd.Common;
using Pms.Masterlists.Domain;
using Pms.Masterlists.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Masterlists.FrontEnd.Stores
{
    public class MasterlistStore : IStore
    {
        public SiteChoices Site { get; set; }
        public Company Company { get; set; }
        public PayrollCode PayrollCode { get; set; }

        private Models.Employees _employeeModel;
        
        private IEnumerable<Employee> _employees;
        public IEnumerable<Employee> Employees { get; private set; }
        
        private Lazy<Task> _initializeLazy;

        public Action Reloaded { get; set; }

        public MasterlistStore(Models.Employees employeeModel)
        {
            _employeeModel = employeeModel;
            _initializeLazy = new Lazy<Task>(Initialize);

            _employees = new List<Employee>();
            Employees = _employees;

            PayrollCode = new();
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
             

            Reloaded?.Invoke();
        }




    }

   
}
