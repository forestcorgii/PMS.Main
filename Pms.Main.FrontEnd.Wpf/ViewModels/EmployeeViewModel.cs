﻿using Microsoft.Toolkit.Mvvm.ComponentModel;
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
        private EmployeeModel _employeeModel { get; set; }
        private CutoffStore _cutoffStore { get; set; }

        private string _filter = "";
        public string Filter
        {
            get => _filter;
            set => SetProperty(ref _filter, value);
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


        public ICommand LoadEmployeesCommand { get; }
        public ICommand DownloadCommand { get; }

        public EmployeeViewModel(CutoffStore cutoffStore, EmployeeModel employeeModel)
        {
            _employeeModel = employeeModel;

            _cutoffStore = cutoffStore;
            _cutoffStore.EmployeesReloaded += _cutoffStore_EmployeesReloaded;
            DownloadCommand = new EmployeeDownloadCommand(this, cutoffStore, employeeModel);
            LoadEmployeesCommand = new EmployeeListingCommand(this, cutoffStore);

            Employees = new ObservableCollection<Employee>(_cutoffStore.Employees);
        }

        private void _cutoffStore_EmployeesReloaded()
        {
            Employees = new ObservableCollection<Employee>(_cutoffStore.Employees);
        }
    }
}