using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Win32;
using Pms.Main.FrontEnd.Common.Utils;
using Pms.PayrollModule.FrontEnd.Models;
using Pms.PayrollModule.FrontEnd.ViewModels;
using Pms.Payrolls.Domain;
using Pms.Payrolls.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static Pms.Payrolls.Domain.Enums;

namespace Pms.PayrollModule.FrontEnd.Commands
{
    public class ImportPayrollRegister : IRelayCommand
    {
        private readonly Models.Payrolls _model;
        private readonly PayrollViewModel _viewModel;


        public ImportPayrollRegister(PayrollViewModel viewModel, Models.Payrolls model)
        {
            _model = model;
            _viewModel = viewModel;

            _canExecute = true;
        }

        public async void Execute(object? parameter)
        {
            _canExecute = false;
            NotifyCanExecuteChanged();

            await Task.Run(() =>
            {
                _viewModel.SetProgress("Select Pay Register files.", 0);

                OpenFileDialog openFile = new() { Multiselect = true };

                bool? isValid = openFile.ShowDialog();
                if (isValid is not null && isValid == true)
                {
                    foreach (string payRegister in openFile.FileNames)
                    {
                        try
                        {
                            IEnumerable<Payroll> extractedPayrolls = _model.Import(payRegister, (ImportProcessChoices)_viewModel.PayrollCode.Process);
                            _viewModel.SetProgress($"Saving extracted Payrolls from {Path.GetFileName(payRegister)}.", extractedPayrolls.Count());
                            foreach (Payroll payroll in extractedPayrolls)
                            {
                                _model.Save(payroll, _viewModel.PayrollCode.PayrollCodeId, _viewModel.Company.CompanyId);
                                if (!_viewModel.IncrementProgress())
                                    break;
                            }
                        }
                        catch (PayrollRegisterHeaderNotFoundException ex)
                        {
                            MessageBoxes.Error($"{ex.Header} was not found in {ex.PayrollRegisterFilePath}.\nMake sure Your select the right Process type.",
                                "Payroll Import Error");
                            break;
                        }
                        catch (Exception ex)
                        {
                            MessageBoxes.Error(ex.Message,
                                "Payroll Import Error");
                            break;
                        }
                    }
                }
            });

            IEnumerable<string> noEEPayrolls = _model.ListNoEEPayrolls();
            //if (noEEPayrolls.Any())
            //    await _viewModel.EmployeeDownloadCommand.ExecuteAsync(noEEPayrolls.ToArray());
            _viewModel.SetAsFinishProgress();
            _viewModel.PayrollListing.Execute(true);

            _canExecute = true;
            NotifyCanExecuteChanged();
        }

        protected bool _canExecute;

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => _canExecute;

        public void NotifyCanExecuteChanged() =>
            CanExecuteChanged?.Invoke(this, new EventArgs());

    }
}
