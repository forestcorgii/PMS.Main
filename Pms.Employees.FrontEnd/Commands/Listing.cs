using Microsoft.Toolkit.Mvvm.Input;
using Pms.Masterlists.FrontEnd.Models;
using Pms.Masterlists.FrontEnd.ViewModels;
using Pms.Masterlists.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pms.Main.FrontEnd.Common;
using Pms.Main.FrontEnd.Common.Utils;

namespace Pms.Masterlists.FrontEnd.Commands.Masterlists
{
    public class Listing : IRelayCommand
    {
        public event EventHandler? CanExecuteChanged;
        private bool executable = true;

        private readonly Employees _model;
        private readonly EmployeeListingVm _viewModel;


        public Listing(EmployeeListingVm viewModel, Employees model)
        {
            _model = model;
            _viewModel = viewModel;
        }


        public bool CanExecute(object? parameter) => executable;


        public async void Execute(object? parameter)
        {
            executable = false;
            try
            {
                IEnumerable<Employee> employees = new List<Employee>();
                await Task.Run(() =>
                {
                    employees = _model.GetEmployees()
                        .IncludeArchived(_viewModel.IncludeArchived)
                        .FilterPayrollCode(_viewModel.PayrollCodeId)
                        .FilterSearchInput(_viewModel.SearchInput);
                });

                _viewModel.Employees = employees.ToList();
            }
            catch (Exception ex) { MessageBoxes.Error(ex.Message); }

            executable = true;
            NotifyCanExecuteChanged();
        }

        public void NotifyCanExecuteChanged() =>
            CanExecuteChanged?.Invoke(this, new EventArgs());
    }



    static class EmployeeFilterExtension
    {

        public static IEnumerable<Employee> FilterPayrollCode(this IEnumerable<Employee> employees, string payrollCode)
        {
            if (!string.IsNullOrEmpty( payrollCode ))
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
