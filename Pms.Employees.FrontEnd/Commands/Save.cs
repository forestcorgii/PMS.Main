using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Win32;
using Pms.Main.FrontEnd.Common;
using Pms.Main.FrontEnd.Common.Utils;
using Pms.Masterlists.Domain;
using Pms.Masterlists.Domain.Exceptions;
using Pms.Masterlists.FrontEnd.Models;
using Pms.Masterlists.FrontEnd.ViewModels;
using Pms.Payrolls.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Pms.Masterlists.FrontEnd.Commands.Masterlists
{
    public class Save : IRelayCommand
    {
        private readonly Employees _model;
        private readonly EmployeeListingVm _viewModel;


        public Save(EmployeeListingVm viewModel, Employees model)
        {
            _model = model;
            _viewModel = viewModel;
        }

        public void Execute(object? parameter)
        {
            _canExecute = false;
            if (parameter is not null)
            {
                try
                {
                    if ((string)parameter == "PERSONAL")
                        _model.Save((IPersonalInformation)_viewModel.SelectedEmployee);
                    else if ((string)parameter == "BANK")
                        _model.Save((IBankInformation)_viewModel.SelectedEmployee);
                    else if ((string)parameter == "GOVERNMENT")
                        _model.Save((IGovernmentInformation)_viewModel.SelectedEmployee);

                    _viewModel.SetProgress("Changes has been saved.", 0);
                }
                catch (InvalidFieldValueException ex) { MessageBoxes.Error(ex.Message, ""); }
                catch (DuplicateBankInformationException ex) { MessageBoxes.Error(ex.Message, ""); }
            }
            else
                _viewModel.SetProgress("No Changes has been saved.", 0);
            _canExecute = true;
        }

        protected bool _canExecute = true;

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => _canExecute;

        public void NotifyCanExecuteChanged() =>
            CanExecuteChanged?.Invoke(this, new EventArgs());

    }
}
