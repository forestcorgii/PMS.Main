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
    public class EmployeeImportCommand : IRelayCommand
    {
        private readonly EmployeeModel _model;
        private readonly ViewModelBase _viewModel;
        private readonly MainStore _mainStore;


        public EmployeeImportCommand(ViewModelBase viewModel, EmployeeModel model, MainStore mainStore)
        {
            _model = model;
            _viewModel = viewModel;
            _mainStore = mainStore;

            _canExecute = true;
        }

        public void Execute(object? parameter)
        {
            _viewModel.SetProgress("Select EE Import file.", 0);

            OpenFileDialog openFile = new();
            bool? isValid = openFile.ShowDialog();
            if (isValid is not null && isValid == true)
            {
                IEnumerable<IBankInformation> extractedEmployee = _model.Import(openFile.FileName);

                _viewModel.SetProgress("Saving Employees bank information.", extractedEmployee.Count());
                foreach (IBankInformation employee in extractedEmployee)
                {
                    _model.Save(employee);
                    _viewModel.ProgressValue++;
                }
                _viewModel.SetAsFinishProgress();
            }
        }

        protected bool _canExecute;

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => _canExecute;

        public void NotifyCanExecuteChanged() =>
            CanExecuteChanged?.Invoke(this, new EventArgs());

    }
}
