using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Win32;
using Pms.Main.FrontEnd.Common;
using Pms.Main.FrontEnd.Common.Utils;
using Pms.Masterlists.Domain;
using Pms.MasterlistModule.FrontEnd.Models;
using Pms.MasterlistModule.FrontEnd.ViewModels;
using Pms.Payrolls.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Pms.Masterlists.Domain.Entities.Employees;

namespace Pms.MasterlistModule.FrontEnd.Commands.Masterlists
{
    public class EEDataImport : IRelayCommand
    {
        private readonly Employees _model;
        private readonly EmployeeListingVm _viewModel;


        public EEDataImport(EmployeeListingVm viewModel, Employees model)
        {
            _model = model;
            _viewModel = viewModel;
        }

        public async void Execute(object? parameter)
        {
            executable = false;

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
                            MessageBoxes.Error(ex.Message,    "EE Data Import Error");
                        }
                    }
                    _viewModel.SetAsFinishProgress();
                }
            });

            executable = true;
        }

        protected bool executable;

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => executable;

        public void NotifyCanExecuteChanged() =>
            CanExecuteChanged?.Invoke(this, new EventArgs());

    }
}
