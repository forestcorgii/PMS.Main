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

namespace Pms.MasterlistModule.FrontEnd.Commands.Payroll_Codes
{
    public class Save : IRelayCommand
    {
        private readonly PayrollCodes _model;
        private readonly PayrollCodeDetailVm _viewModel;


        public Save(PayrollCodeDetailVm viewModel, PayrollCodes model)
        {
            _model = model;
            _viewModel = viewModel;
        }

        public void Execute(object? parameter)
        {
            Executable = false;
            NotifyCanExecuteChanged();

            try
            {
                _viewModel.SelectedPayrollCode.PayrollCodeId = PayrollCode.GenerateId(_viewModel.SelectedPayrollCode);
                _model.Save(_viewModel.SelectedPayrollCode);

                MessageBoxes.Prompt("Changes has been successfully saved.", "");
            }
            catch (Exception ex) { MessageBoxes.Error(ex.Message, ""); }

            Executable = true;
            NotifyCanExecuteChanged();
        }

        protected bool Executable = true;

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => Executable;

        public void NotifyCanExecuteChanged() => CanExecuteChanged?.Invoke(this, new EventArgs());

    }
}
