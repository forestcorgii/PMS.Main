using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Win32;
using Pms.Employees.Domain;
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
    public class EmployeeBankImportCommand : IRelayCommand
    {
        private readonly EmployeeModel _model;
        private readonly EmployeeViewModel _viewModel;


        public EmployeeBankImportCommand(EmployeeViewModel viewModel, EmployeeModel model)
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
                    _viewModel.SetProgress("Saving Employees bank information.", openFile.FileNames.Length);
                    foreach (string filename in openFile.FileNames)
                    {
                        try
                        {
                            //IEnumerable<IBankInformation> extractedEmployee = _model.Import(filename);
                            //foreach (IBankInformation employee in extractedEmployee)
                            //    _model.Save(employee);

                            //_viewModel.ProgressValue++;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message,
                                  "Bank Import Error",
                                  MessageBoxButton.OK,
                                  MessageBoxImage.Error
                              );
                        }
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
