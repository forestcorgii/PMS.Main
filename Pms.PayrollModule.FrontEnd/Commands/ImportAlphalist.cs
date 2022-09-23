using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Win32;
using Pms.Masterlists.Domain;
using Pms.PayrollModule.FrontEnd.Models;
using Pms.PayrollModule.FrontEnd.ViewModels;
using Pms.Payrolls.Domain;
using Pms.Payrolls.Domain.SupportTypes;
using Pms.Payrolls.ServiceLayer.Files;
using Pms.Payrolls.ServiceLayer.Files.Exports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static Pms.Payrolls.Domain.Enums;
using Pms.Main.FrontEnd.Common.Utils;

namespace Pms.PayrollModule.FrontEnd.Commands
{
    public class ImportAlphalist : IRelayCommand
    {
        public event EventHandler? CanExecuteChanged;

        private readonly AlphalistViewModel _viewModel;
        private readonly Models.Payrolls _model;

        private bool _canExecute { get; set; } = true;

        public ImportAlphalist(AlphalistViewModel viewModel, Models.Payrolls model)
        {
            _viewModel = viewModel;
            _model = model;
        }


        public bool CanExecute(object? parameter) => _canExecute;


        public async void Execute(object? parameter)
        {
            _canExecute = false;
            try
            {
                await Task.Run(() =>
                {
                    _viewModel.SetProgress("Select Alphalist files.", 0);
                    OpenFileDialog openFile = new() { Multiselect = true };

                    bool? isValid = openFile.ShowDialog();
                    if (isValid is not null && isValid == true)
                    {
                        _viewModel.SetProgress("Sending Alphalist to BIR Program DBF.", 1);
                        foreach (string payRegister in openFile.FileNames)
                        {
                            try
                            {
                                string payrollCode = _viewModel.PayrollCodeId;
                                Company company = _viewModel.Company;
                                if (company is not null)
                                {
                                    CompanyView _companyView = new(company.RegisteredName, company.TIN, company.BranchCode, company.Region);
                                    
                                    AlphalistImport importer = new();
                                    importer.ImportToBIRProgram(
                                        payRegister,
                                        _viewModel.BirDbfDirectory,
                                        _companyView,
                                        _viewModel.Cutoff.YearCovered
                                    );
                                }
                                else
                                    MessageBoxes.Error("Company is null.", "Alphalist Import Error");
                            }
                            catch (Exception ex)
                            {
                                MessageBoxes.Error(ex.Message, "Alphalist Import Error");
                            }

                        }
                    }

                    _viewModel.SetAsFinishProgress();
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            _canExecute = true;
            NotifyCanExecuteChanged();
        }

        public void NotifyCanExecuteChanged() =>
            CanExecuteChanged?.Invoke(this, new EventArgs());
    }
}
