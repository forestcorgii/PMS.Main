using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Win32;
using Pms.Masterlists.Domain;
using Pms.Masterlists.Domain.Exceptions;
using Pms.Masterlists.FrontEnd.Models;
using Pms.Masterlists.FrontEnd.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
//using static Pms.Main.FrontEnd.Wpf.Utils.MessageBoxes;

using Pms.Main.FrontEnd.Common;

namespace Pms.Masterlists.FrontEnd.Commands
{
    public class BankImport : IRelayCommand
    {
        private readonly Models.Employees _model;
        private readonly EmployeeListingVm _viewModel;


        public BankImport(EmployeeListingVm viewModel, Models.Employees model)
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
                                catch (InvalidFieldValueException ex) { MessageBoxes.ShowError(ex.Message, Path.GetFileName(filename)); }
                                catch (DuplicateBankInformationException ex) { MessageBoxes.ShowError(ex.Message, Path.GetFileName(filename)); }
                                _viewModel.ProgressValue++;
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBoxes.ShowError(ex.Message, Path.GetFileName(filename));
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
