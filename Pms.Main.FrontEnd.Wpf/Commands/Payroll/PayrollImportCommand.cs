using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Win32;
using Pms.Main.FrontEnd.Wpf.Models;
using Pms.Main.FrontEnd.Wpf.Stores;
using Pms.Main.FrontEnd.Wpf.ViewModels;
using Pms.Payrolls.Domain;
using Pms.Payrolls.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Pms.Main.FrontEnd.Wpf.Commands
{
    public class PayrollImportCommand : IRelayCommand
    {
        private readonly PayrollModel _model;
        private readonly PayrollViewModel _viewModel;
        private readonly MainStore _mainStore;


        public PayrollImportCommand(PayrollViewModel viewModel, PayrollModel model, MainStore mainStore)
        {
            _model = model;
            _viewModel = viewModel;
            _mainStore = mainStore;

            _canExecute = true;
        }

        public async void Execute(object? parameter)
        {
            await Task.Run(() =>
            {
                _viewModel.SetProgress("Select Pay Register files.", 0);

                OpenFileDialog openFile = new() { Multiselect = true };

                bool? isValid = openFile.ShowDialog();
                if (isValid is not null && isValid == true)
                {
                    _viewModel.SetProgress($"Saving extracted Payrolls.", openFile.FileNames.Length);
                    foreach (string payRegister in openFile.FileNames)
                    {
                        try
                        {
                            IEnumerable<Payroll> extractedPayrolls = _model.Import(payRegister, _viewModel.Process);
                            foreach (Payroll payroll in extractedPayrolls)
                            {
                                payroll.PayrollCode = _mainStore.PayrollCode;
                                _model.Save(payroll, _mainStore.PayrollCode, _viewModel.Bank, _viewModel.CompanyId);
                            }
                        }
                        catch (PayrollRegisterHeaderNotFoundException ex)
                        {
                            MessageBox.Show($"{ex.Header} was not found in {ex.PayrollRegisterFilePath}.\nMake sure Your select the right Process type.",
                                "Payroll Import Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error
                            );
                            break;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message,
                                "Payroll Import Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error
                            );
                            break;
                        }
                        _viewModel.ProgressValue++;
                    }
                }
            });

            IEnumerable<string> noEEPayrolls = _model.ListNoEEPayrolls();
            if (noEEPayrolls.Any())
                await _viewModel.EmployeeDownloadCommand.ExecuteAsync(noEEPayrolls.ToArray());
            _viewModel.SetAsFinishProgress();
            _viewModel.PayrollListing.Execute(true);
        }

        protected bool _canExecute;

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => _canExecute;

        public void NotifyCanExecuteChanged() =>
            CanExecuteChanged?.Invoke(this, new EventArgs());

    }
}
