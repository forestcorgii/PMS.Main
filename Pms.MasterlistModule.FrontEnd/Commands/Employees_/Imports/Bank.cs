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
        private readonly EmployeeListingVm ListingVm;


        public BankImport(EmployeeListingVm viewModel, Employees model)
        {
            _model = model;
            ListingVm = viewModel;
            ListingVm.CanExecuteChanged += ListingVm_CanExecuteChanged;
        }

        public async void Execute(object? parameter)
        {
              await Task.Run(() =>
            {
                ListingVm.SetProgress("Select EE Import file.", 0);

                OpenFileDialog openFile = new() { Multiselect = true };
                bool? isValid = openFile.ShowDialog();
                if (isValid is not null && isValid == true)
                {
                    foreach (string filename in openFile.FileNames)
                    {
                        try
                        {
                            IEnumerable<IBankInformation> extractedEmployee = _model.ImportBankInformation(filename);
                            
                            ListingVm.SetProgress($"Saving Extracted employees bank information from {Path.GetFileName(filename)}.", extractedEmployee.Count());
                            foreach (IBankInformation employee in extractedEmployee)
                            {
                                try { _model.Save(employee); }
                                catch (InvalidFieldValueException ex) { MessageBoxes. Error(ex.Message, Path.GetFileName(filename)); }
                                catch (DuplicateBankInformationException ex) { MessageBoxes.Error(ex.Message, Path.GetFileName(filename)); }
                                ListingVm.ProgressValue++;
                            }
                        }
                        catch (Exception ex) { MessageBoxes.Error(ex.Message, Path.GetFileName(filename)); }
                    }
                    ListingVm.SetAsFinishProgress();
                }
            });
          }
         
        public event EventHandler? CanExecuteChanged;


        public bool CanExecute(object? parameter) => ListingVm.Executable;
        private void ListingVm_CanExecuteChanged(object? sender, bool e) => NotifyCanExecuteChanged();
        public void NotifyCanExecuteChanged() =>
            CanExecuteChanged?.Invoke(this, new EventArgs());

    }
}
