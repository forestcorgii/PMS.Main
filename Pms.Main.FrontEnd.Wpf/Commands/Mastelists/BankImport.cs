using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Win32;
using Pms.Masterlists.Domain;
using Pms.Masterlists.Domain.Exceptions;
using Pms.Main.FrontEnd.Wpf.Models;
using Pms.Main.FrontEnd.Wpf.ViewModels;
using Pms.Payrolls.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Pms.Main.FrontEnd.Common.Utils;

namespace Pms.Main.FrontEnd.Wpf.Commands
{
    public class BankImport : IRelayCommand
    {
        private readonly MasterlistModel _model;
        private readonly MasterlistViewModel _viewModel;


        public BankImport(MasterlistViewModel viewModel, MasterlistModel model)
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
                                catch (InvalidFieldValueException ex) { MessageBoxes.Error(ex.Message, Path.GetFileName(filename)); }
                                catch (DuplicateBankInformationException ex) { MessageBoxes.Error(ex.Message, Path.GetFileName(filename)); }
                                _viewModel.ProgressValue++;
                            }
                        }
                        catch (Exception ex) { MessageBoxes.Error(ex.Message, Path.GetFileName(filename)); }
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
