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
        private CutoffStore _cutoffStore;

        public event EventHandler? CanExecuteChanged;
        private bool _canExecute;


        public EmployeeListingCommand(EmployeeViewModel viewModel, CutoffStore cutoffStore)
        {
            _viewModel = viewModel;
            _cutoffStore = cutoffStore;

        }
        public bool CanExecute(object? parameter) => _canExecute;

        public async void Execute(object? parameter)
        {
            _canExecute = false;

            try
            {
                await _cutoffStore.LoadEmployees();
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
