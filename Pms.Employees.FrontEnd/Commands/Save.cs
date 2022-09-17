using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Win32;
using Pms.Main.FrontEnd.Common;
using Pms.Masterlists.Domain;
using Pms.Masterlists.Domain.Exceptions;
using Pms.Masterlists.FrontEnd.Models;
using Pms.Masterlists.FrontEnd.Stores;
using Pms.Masterlists.FrontEnd.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Pms.Masterlists.FrontEnd.Commands
{
    public class Save : IRelayCommand
    {
        private readonly Models.Employees _model;
        private readonly EmployeeDetailVm _viewModel;


        public Save(EmployeeDetailVm viewModel, Models.Employees model)
        {
            _model = model;
            _viewModel = viewModel;

            _canExecute = true;
        }

        public void Execute(object parameter)
        {
            if (parameter is not null)
            {
                try
                {
                    if ((string)parameter == "PERSONAL")
                        _model.Save((IPersonalInformation)_viewModel.Employee);
                    else if ((string)parameter == "BANK")
                        _model.Save((IBankInformation)_viewModel.Employee);
                    else if ((string)parameter == "GOVERNMENT")
                        _model.Save((IGovernmentInformation)_viewModel.Employee);

                    MessageBoxes.ShowMessage("Changes has been successfully saved.", "");
                }
                catch (InvalidFieldValueException ex) { MessageBoxes.ShowError(ex.Message, ""); }
                catch (DuplicateBankInformationException ex) { MessageBoxes.ShowError(ex.Message, ""); }
            }
            else
                MessageBoxes.ShowMessage("No Changes has been saved.", "");
        }

        protected bool _canExecute;

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => _canExecute;

        public void NotifyCanExecuteChanged() =>
            CanExecuteChanged?.Invoke(this, new EventArgs());

    }
}
