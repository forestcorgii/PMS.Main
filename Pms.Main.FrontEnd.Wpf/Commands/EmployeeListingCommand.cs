using Microsoft.Toolkit.Mvvm.Input;
using Pms.Main.FrontEnd.Wpf.Models;
using Pms.Main.FrontEnd.Wpf.Stores;
using Pms.Main.FrontEnd.Wpf.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Main.FrontEnd.Wpf.Commands
{
    public class EmployeeListingCommand : IRelayCommand
    {
        private EmployeeViewModel _viewModel;
        private EmployeeStore _employeeStore;

        public event EventHandler? CanExecuteChanged;
        private bool _canExecute;


        public EmployeeListingCommand(EmployeeViewModel viewModel, EmployeeStore employeeStore)
        {
            _viewModel = viewModel;
            _employeeStore = employeeStore;

        }
        public bool CanExecute(object? parameter) => _canExecute;

        public async void Execute(object? parameter)
        {
            _canExecute = false;

            try
            {
                await _employeeStore.Load();
            }
            catch
            {
                _viewModel.StatusMessage = "Failed to Load Timesheets.";
            }

            _canExecute = true;
            NotifyCanExecuteChanged();
        }

        public void NotifyCanExecuteChanged() =>
            CanExecuteChanged?.Invoke(this, new EventArgs());
    }
}
