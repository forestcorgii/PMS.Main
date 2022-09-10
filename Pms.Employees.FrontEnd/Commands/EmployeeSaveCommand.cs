﻿using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Win32;
using Pms.Employees.Domain;
using Pms.Employees.Domain.Exceptions;
using Pms.Employees.FrontEnd.Models;
using Pms.Employees.FrontEnd.Stores;
using Pms.Employees.FrontEnd.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Pms.Employees.FrontEnd.Commands
{
    public class EmployeeSaveCommand : IRelayCommand
    {
        private readonly EmployeeModel _model;
        private readonly EmployeeViewModel _viewModel;


        public EmployeeSaveCommand(EmployeeViewModel viewModel, EmployeeModel model)
        {
            _model = model;
            _viewModel = viewModel;

            _canExecute = true;
        }

        public void Execute(object? parameter)
        {
            if (parameter is not null)
            {
                try
                {
                    if ((string)parameter == "PERSONAL")
                        _model.Save((IPersonalInformation)_viewModel.SelectedEmployee);
                    else if ((string)parameter == "BANK")
                        _model.Save((IBankInformation)_viewModel.SelectedEmployee);
                    else if ((string)parameter == "GOVERNMENT")
                        _model.Save((IGovernmentInformation)_viewModel.SelectedEmployee);

                    _viewModel.SetProgress("Changes has been saved.", 0);
                }
                catch (InvalidEmployeeFieldValueException ex)
                {
                    MessageBox.Show(ex.Message,
                        "Employee Save Import Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error
                    );
                }
            }
            else
                _viewModel.SetProgress("No Changes has been saved.", 0);
        }

        protected bool _canExecute;

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => _canExecute;

        public void NotifyCanExecuteChanged() =>
            CanExecuteChanged?.Invoke(this, new EventArgs());

    }
}
