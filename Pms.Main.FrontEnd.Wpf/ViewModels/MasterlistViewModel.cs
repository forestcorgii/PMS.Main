using Microsoft.Toolkit.Mvvm.ComponentModel;
using Pms.Masterlists.Domain;
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
using static Pms.Masterlists.Domain.Enums;

namespace Pms.Main.FrontEnd.Wpf.ViewModels
{
    public class MasterlistViewModel : ViewModelBase
    {
        private MasterlistStore _store { get; set; }

        public string Filter
        {
            get => _store.Filter;
            set
            {
                SetProperty(ref _store.Filter, value);
                _store.ReloadFilter();
            }
        }
        
        public bool IncludeArchived
        {
            get => _store.IncludeArchived;
            set
            {
                SetProperty(ref _store.IncludeArchived, value);
                _store.ReloadFilter();
            }
        }

        private Employee _selectedEmployee;
        public Employee SelectedEmployee
        {
            get => _selectedEmployee;
            set => SetProperty(ref _selectedEmployee, value);
        }

        private ObservableCollection<Employee> _employees;
        public ObservableCollection<Employee> Employees
        {
            get => _employees;
            set => SetProperty(ref _employees, value);
        }

        public ObservableCollection<BankChoices> BankTypes => new ObservableCollection<BankChoices>(Enum.GetValues(typeof(BankChoices)).Cast<BankChoices>());

        public ICommand LoadEmployeesCommand { get; }
        public ICommand DownloadCommand { get; }
        public ICommand BankImportCommand { get; }
        public ICommand EEDataImportCommand { get; }
        public ICommand SaveCommand { get; }

        public MasterlistViewModel(MainStore mainStore, MasterlistStore employeeStore, MasterlistModel employeeModel)
        {
            DownloadCommand = new Download(this, mainStore, employeeStore, employeeModel);
            BankImportCommand = new BankImport(this, employeeModel);
            EEDataImportCommand = new EEDataImport(this, employeeModel);
            SaveCommand = new Save(this, employeeModel, mainStore);

            LoadEmployeesCommand = new ListingCommand(employeeStore);
            LoadEmployeesCommand.Execute(null);

            _selectedEmployee = new();

            _store = employeeStore;
            _store.Reloaded += _cutoffStore_EmployeesReloaded;

            _employees = new ObservableCollection<Employee>();
            Employees = new ObservableCollection<Employee>(_store.Employees);
        }

        public override void Dispose()
        {
            _store.Reloaded -= _cutoffStore_EmployeesReloaded;
            base.Dispose();
        }

        private void _cutoffStore_EmployeesReloaded()
        {
            Employees = new ObservableCollection<Employee>(_store.Employees);
            if (Employees.Count == 1)
                SelectedEmployee = Employees.First();
        }


    }
}
