using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Win32;
using Pms.Main.FrontEnd.Wpf.Models;
using Pms.Main.FrontEnd.Wpf.Stores;
using Pms.Main.FrontEnd.Wpf.ViewModels;
using Pms.Payrolls.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

                OpenFileDialog openFile = new()
                {
                    Multiselect = true
                };

                bool? isValid = openFile.ShowDialog();
                if (isValid is not null && isValid == true)
                {
                    foreach (string payRegister in openFile.FileNames)
                    {
                        try
                        {
                            IEnumerable<Payroll> extractedPayrolls = _model.Import(payRegister);

                            _viewModel.SetProgress($"Saving extracted Payrolls from {payRegister}.", extractedPayrolls.Count());
                            foreach (Payroll payroll in extractedPayrolls)
                            {
                                payroll.PayrollCode = _mainStore.PayrollCode;
                                _model.Save(payroll, _mainStore.PayrollCode, Enums.BankType.LBP);
                                _viewModel.ProgressValue++;
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                }
            });

            IEnumerable<string> noEEPayrolls = _model.ListNoEEPayrolls();
            if (noEEPayrolls.Any())
                await _viewModel.EmployeeDownloadCommand.ExecuteAsync(noEEPayrolls.ToArray());
            _viewModel.SetAsFinishProgress();
        }

        protected bool _canExecute;

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => _canExecute;

        public void NotifyCanExecuteChanged() =>
            CanExecuteChanged?.Invoke(this, new EventArgs());

    }
}
