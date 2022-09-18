using Microsoft.Toolkit.Mvvm.Input;
using Pms.Adjustments.Domain;
using Pms.Main.FrontEnd.Wpf.Models;
using Pms.Main.FrontEnd.Wpf.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pms.Main.FrontEnd.Wpf.Commands.Billings
{
    public class Export : IRelayCommand
    {
        public event EventHandler? CanExecuteChanged;

        private readonly BillingViewModel _viewModel;
        private readonly BillingModel _model;

        private bool _canExecute { get; set; } = true;

        public Export(BillingViewModel viewModel, BillingModel model)
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
                    _viewModel.SetProgress("Exporting Payrolls for Land Bank.",1);
                    string cutoffId = _viewModel.CutoffId.CutoffId;
                    string payrollCode= _viewModel.PayrollCodeId.PayrollCodeId;
                    string adjustmentName= _viewModel.AdjustmentName;
                    IEnumerable<Billing> billings = _viewModel.Billings;

                    _model.Export(billings, cutoffId, $"{cutoffId}_{payrollCode}_{adjustmentName}.xls");
                    _viewModel.SetAsFinishProgress();
                });
            }
            catch (Exception ex)
            {
                _viewModel.StatusMessage= ex.Message;
            }
            _canExecute = true;
            NotifyCanExecuteChanged();
        }

        public void NotifyCanExecuteChanged() =>
            CanExecuteChanged?.Invoke(this, new EventArgs());
    }
}
