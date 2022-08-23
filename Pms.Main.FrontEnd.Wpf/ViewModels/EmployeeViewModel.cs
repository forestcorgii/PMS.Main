using Microsoft.Toolkit.Mvvm.ComponentModel;
using Pms.Employees.Domain;
using Pms.Main.FrontEnd.Wpf.Commands;
using Pms.Main.FrontEnd.Wpf.Models;
using Pms.Main.FrontEnd.Wpf.Stores;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Pms.Main.FrontEnd.Wpf.ViewModels
{
    public class EmployeeViewModel : ViewModelBase
    {
        private EmployeeStore _employeeStore { get; set; }

        private string _filter = "";
        public string Filter
        {
            get => _filter;
            set
            {

                SetProperty(ref _filter, value);
            }
        }

        private Employee _selectedEmployee;
        public Employee SelectedEmployee
        {
            get => _selectedEmployee;
            set => SetProperty(ref _selectedEmployee, value);
        }
        public IGeneralInformation SelectedGeneralInformation { get => SelectedEmployee; }
        public IBankInformation SelectedBankInformation { get => SelectedEmployee; }

        private ObservableCollection<Employee> _employees;
        public ObservableCollection<Employee> Employees
        {
            get => _employees;
            set => SetProperty(ref _employees, value);
        }


        public ICommand LoadEmployeesCommand { get; }
        public ICommand DownloadCommand { get; }
        public ICommand ImportCommand { get; }
        public ICommand SaveCommand { get; }

        public EmployeeViewModel(MainStore mainStore, EmployeeStore employeeStore, EmployeeModel employeeModel)
        {
            DownloadCommand = new EmployeeDownloadCommand(this, mainStore, employeeStore, employeeModel);
            ImportCommand = new EmployeeImportCommand(this, employeeModel, mainStore);
            SaveCommand = new EmployeeSaveCommand(this, employeeModel, mainStore);

            LoadEmployeesCommand = new ListingCommand(employeeStore);
            LoadEmployeesCommand.Execute(null);

            _selectedEmployee = new();

            _employeeStore = employeeStore;
            _employeeStore.Reloaded += _cutoffStore_EmployeesReloaded;

            _employees = new ObservableCollection<Employee>();
            Employees = new ObservableCollection<Employee>(_employeeStore.Employees);
        }

        public override void Dispose()
        {
            _employeeStore.Reloaded -= _cutoffStore_EmployeesReloaded;
            base.Dispose();
        }

        private void _cutoffStore_EmployeesReloaded()
        {
            Employees = new ObservableCollection<Employee>(_employeeStore.Employees);
        }


    }
}
