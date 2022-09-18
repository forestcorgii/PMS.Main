using Microsoft.Toolkit.Mvvm.Input;
using Pms.Adjustments.Domain;
using Pms.Main.FrontEnd.Wpf.Models;
using Pms.Main.FrontEnd.Wpf.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Main.FrontEnd.Wpf.Commands.Billings
{
    public class Generate : IRelayCommand
    {
        public event EventHandler? CanExecuteChanged;

        private BillingViewModel _viewModel;
        private BillingModel _model;
        private MasterlistModel _employeeModel;


        public Generate(BillingViewModel viewModel, BillingModel model, MasterlistModel employeeModel)
        {
            _model = model;
            _viewModel = viewModel;
            _employeeModel = employeeModel;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public async void Execute(object? parameter)
        {
            await Task.Run(() =>
            {
                string[] eeIds = _employeeModel.GetEmployees().Where(ee => ee.PayrollCode == _viewModel.PayrollCodeId).Select(ee => ee.EEId).ToArray();
                List<Billing> billings = new();

                _viewModel.SetProgress("Billings Generation on going.", eeIds.Length);
                foreach (string eeId in eeIds)
                {
                    billings.AddRange(_model.GenerateBillings(_viewModel.CutoffId, eeId));
                    _viewModel.ProgressValue++;
                }

                _viewModel.SetProgress("Saving Generated billings.", billings.Count);
                foreach (Billing billing in billings)
                {
                    billing.PayrollCode = _viewModel.PayrollCodeId;
                    _model.AddBilling(billing);
                    _viewModel.ProgressValue++;
                }

                _viewModel.SetAsFinishProgress();
                _viewModel.ListBillings.Execute(true);
            });
        }

        public void NotifyCanExecuteChanged() { }
    }
}
