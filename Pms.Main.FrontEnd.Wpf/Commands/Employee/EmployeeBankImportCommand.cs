using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Win32;
using Pms.Employees.Domain;
using Pms.Employees.Domain.Exceptions;
using Pms.Main.FrontEnd.Wpf.Models;
using Pms.Main.FrontEnd.Wpf.Stores;
using Pms.Main.FrontEnd.Wpf.ViewModels;
using Pms.Payrolls.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static Pms.Main.FrontEnd.Wpf.Utils.MessageBoxes;

namespace Pms.Main.FrontEnd.Wpf.Commands
{
    public class EmployeeBankImportCommand : IRelayCommand
    {
        private readonly EmployeeModel _model;
        private readonly ViewModelBase _viewModel;


        public EmployeeBankImportCommand(ViewModelBase viewModel, EmployeeModel model)
        {
            _model = model;
            _viewModel = viewModel;

            _canExecute = true;
        }

        public async void Execute(object? parameter)
        {
            await Task.Run(() =>
            {
                _viewModel.SetProgress("Select EE Import file.", 0);

                OpenFileDialog openFile = new() { Multiselect = true };
                bool? isValid = openFile.ShowDialog();
                if (isValid is not null && isValid == true)
                {
                    foreach (string filename in openFile.FileNames)
                    {
                        try
                        {
                            IEnumerable<IBankInformation> extractedEmployee = _model.ImportBankInformation(filename);
                            
                            _viewModel.SetProgress($"Saving Extracted employees bank information from {Path.GetFileName(filename)}.", extractedEmployee.Count());
                            foreach (IBankInformation employee in extractedEmployee)
                            {
                                try { _model.Save(employee); }
                                catch (InvalidEmployeeFieldValueException ex) { ShowError(ex.Message, Path.GetFileName(filename)); }
                                catch (DuplicateBankInformationException ex) { ShowError(ex.Message, Path.GetFileName(filename)); }
                                _viewModel.ProgressValue++;
                            }
                        }
                        catch (Exception ex) { ShowError(ex.Message, Path.GetFileName(filename)); }
                    }
                    _viewModel.SetAsFinishProgress();
                }
            });
        }

        protected bool _canExecute;

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => _canExecute;

        public void NotifyCanExecuteChanged() =>
            CanExecuteChanged?.Invoke(this, new EventArgs());

    }
}
