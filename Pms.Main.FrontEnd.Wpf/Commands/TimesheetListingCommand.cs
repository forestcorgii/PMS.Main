using Microsoft.Toolkit.Mvvm.Input;
using Pms.Main.FrontEnd.Wpf.Stores;
using Pms.Main.FrontEnd.Wpf.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Main.FrontEnd.Wpf.Commands
{
    public class TimesheetListingCommand : IRelayCommand
    {
        public event EventHandler? CanExecuteChanged;
        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly TimesheetViewModel _viewModel;
        private TimesheetStore _timesheetStore;
        private bool _canExecute;

        public TimesheetListingCommand(TimesheetViewModel viewModel, TimesheetStore timesheetStore)
        {
            _timesheetStore = timesheetStore;
            _viewModel = viewModel;
        }


        public bool CanExecute(object? parameter) => _canExecute;

        public async void Execute(object? parameter)
        {
            _canExecute = false;
            try
            {
                await _timesheetStore.Load();
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
