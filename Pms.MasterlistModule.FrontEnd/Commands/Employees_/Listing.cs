using Microsoft.Toolkit.Mvvm.Input;
using Pms.MasterlistModule.FrontEnd.Models;
using Pms.MasterlistModule.FrontEnd.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pms.Main.FrontEnd.Common.Utils;
using Pms.Masterlists.Domain.Entities.Employees;

namespace Pms.MasterlistModule.FrontEnd.Commands.Employees_
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
            NotifyCanExecuteChanged();
            try
            {
                IEnumerable<Employee> employees = new List<Employee>();
                await Task.Run(() =>
                {
                    employees = _model.GetEmployees()
                        .HideArchived(_viewModel.HideArchived)
                        .FilterSearchInput(_viewModel.SearchInput)
                        .FilterPayrollCode(_viewModel.PayrollCodeId);
                });

                _viewModel.Employees = employees.ToList();

                _viewModel.ActiveEECount = employees.Count(e => e.Active);
                _viewModel.NonActiveEECount = employees.Count(e => !e.Active);
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
            if (!string.IsNullOrEmpty(payrollCode))
                return employees.Where(p => p.PayrollCode == payrollCode);
            return employees;
        }

        public static IEnumerable<Employee> FilterSearchInput(this IEnumerable<Employee> employees, string filter)
        {
            if (!string.IsNullOrEmpty(filter))
                employees = employees
                   .Where(ts =>
                       ts.EEId.Contains(filter) ||
                       ts.Fullname.Contains(filter) ||
                       ts.CardNumber.Contains(filter) ||
                       ts.AccountNumber.Contains(filter)
                   );

            return employees;
        }

        public static IEnumerable<Employee> HideArchived(this IEnumerable<Employee> employees, bool hideArchived)
        {
            if (hideArchived)
                return employees.Where(ee => ee.Active);
            else
                return employees;
        }
    }
}
