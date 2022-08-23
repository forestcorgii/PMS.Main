using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Win32;
using Pms.Employees.Domain;
using Pms.Main.FrontEnd.Wpf.Models;
using Pms.Main.FrontEnd.Wpf.Stores;
using Pms.Main.FrontEnd.Wpf.ViewModels;
using Pms.Payrolls.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Main.FrontEnd.Wpf.Commands
{
    public class EmployeeSaveCommand : IRelayCommand
    {
        private readonly EmployeeModel _model;
        private readonly EmployeeViewModel _viewModel;
        private readonly MainStore _mainStore;


        public EmployeeSaveCommand(EmployeeViewModel viewModel, EmployeeModel model, MainStore mainStore)
        {
            _model = model;
            _viewModel = viewModel;
            _mainStore = mainStore;

            _canExecute = true;
        }

        public void Execute(object? parameter)
        {
            if (parameter is not null)
            {
                if (parameter is IGeneralInformation)
                    _model.Save((IGeneralInformation)parameter);
                else if (parameter is IBankInformation)
                    _model.Save((IBankInformation)parameter);

                _viewModel.SetProgress("Changes has been saved.", 0);
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
