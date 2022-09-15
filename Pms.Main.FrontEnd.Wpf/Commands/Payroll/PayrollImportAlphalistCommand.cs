using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Win32;
using Pms.Masterlists.Domain;
using Pms.Main.FrontEnd.Wpf.Models;
using Pms.Main.FrontEnd.Wpf.Stores;
using Pms.Main.FrontEnd.Wpf.ViewModels;
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

namespace Pms.Main.FrontEnd.Wpf.Commands
{
    public class PayrollImportAlphalistCommand : IRelayCommand
    {
        public event EventHandler? CanExecuteChanged;

        private readonly PayrollStore _store;
        private readonly MainStore _mainStore;
        private readonly AlphalistViewModel _viewModel;
        private readonly PayrollModel _model;

        private bool _canExecute { get; set; } = true;

        public PayrollImportAlphalistCommand(AlphalistViewModel viewModel, PayrollModel model, PayrollStore store, MainStore mainStore)
        {
            _store = store;
            _viewModel = viewModel;
            _model = model;
            _mainStore = mainStore;
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
                                Cutoff cutoff = new(_mainStore.Cutoff.CutoffId);
                                string companyId = _viewModel.CompanyId;
                                CompanyView company = new();//_store.Companies.Where(c => c.CompanyId == companyId).First();

                                AlphalistImport importer = new();
                                importer.ImportToBIRProgram(payRegister, _viewModel.BirDbfDirectory, company);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message,
                                    "Alphalist Import Error",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error
                                );
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
