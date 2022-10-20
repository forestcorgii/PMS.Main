using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Win32;
using Pms.Main.FrontEnd.Common;
using Pms.Main.FrontEnd.Common.Utils;
using Pms.Masterlists.Domain;
using Pms.Masterlists.Domain.Exceptions;
using Pms.MasterlistModule.FrontEnd.Models;
using Pms.MasterlistModule.FrontEnd.ViewModels;
using Pms.Payrolls.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Pms.Masterlists.Domain.Entities.Employees;

namespace Pms.MasterlistModule.FrontEnd.Commands.Employees_
{
    public class BankImport : IRelayCommand
    {
        private readonly Employees _model;
        private readonly EmployeeListingVm _viewModel;


        public BankImport(EmployeeListingVm viewModel, Employees model)
        {
            _model = model;
            _viewModel = viewModel;

        }

        public async void Execute(object? parameter)
        {
            executable = false;
            NotifyCanExecuteChanged();
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
                                catch (InvalidFieldValueException ex) { MessageBoxes. Error(ex.Message, Path.GetFileName(filename)); }
                                catch (DuplicateBankInformationException ex) { MessageBoxes.Error(ex.Message, Path.GetFileName(filename)); }
                                _viewModel.ProgressValue++;
                            }
                        }
                        catch (Exception ex) { MessageBoxes.Error(ex.Message, Path.GetFileName(filename)); }
                    }
                    _viewModel.SetAsFinishProgress();
                }
            });
            executable = true;
            NotifyCanExecuteChanged();
        }

        protected bool executable=true;

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => executable;

        public void NotifyCanExecuteChanged() =>
            CanExecuteChanged?.Invoke(this, new EventArgs());

    }
}
