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

namespace Pms.MasterlistModule.FrontEnd.Commands.Masterlists
{
    public class Save : IRelayCommand
    {
        private readonly Employees _model;
        private readonly EmployeeDetailVm _viewModel;


        public Save(EmployeeDetailVm viewModel, Employees model)
        {
            _model = model;
            _viewModel = viewModel;
        }

        public void Execute(object? parameter)
        {
            executable = false;
            if (parameter is not null)
            {
                try
                {
                    _model.Save(_viewModel.Employee);

                    MessageBoxes.Prompt("Changes has been successfully saved.", "");
                    
                    _viewModel.Close();
                }
                catch (InvalidFieldValueException ex) { MessageBoxes.Error(ex.Message, ""); }
                catch (DuplicateBankInformationException ex) { MessageBoxes.Error(ex.Message, ""); }
            }

            executable = true;
        }

        protected bool executable = true;

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => executable;

        public void NotifyCanExecuteChanged() =>
            CanExecuteChanged?.Invoke(this, new EventArgs());

    }
}
