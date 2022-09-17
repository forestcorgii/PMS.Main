using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Win32;
using Pms.Masterlists.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Pms.Masterlists.FrontEnd.Models;
using Pms.Masterlists.FrontEnd.ViewModels;
using Pms.Main.FrontEnd.Common;

namespace Pms.Masterlists.FrontEnd.Commands
{
    public class EEDataImport : IRelayCommand
    {
        private readonly Models.Employees _model;
        private readonly EmployeeListingVm _viewModel;


        public EEDataImport(EmployeeListingVm viewModel, Models.Employees model)
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
                            IEnumerable<IEEDataInformation> extractedEmployee = _model.ImportEEData(filename);
                            _viewModel.SetProgress("Saving Employees EE Data information.", extractedEmployee.Count());
                            foreach (IEEDataInformation employee in extractedEmployee)
                            {
                                _model.Save(employee);
                                _viewModel.ProgressValue++;
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBoxes.ShowError(ex.Message,
                                  "EE Data Import Error"
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
