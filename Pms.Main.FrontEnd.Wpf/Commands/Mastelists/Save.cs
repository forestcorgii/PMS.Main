﻿using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Win32;
using Pms.Masterlists.Domain;
using Pms.Masterlists.Domain.Exceptions;
using Pms.Main.FrontEnd.Wpf.Models;
using Pms.Main.FrontEnd.Wpf.ViewModels;
using Pms.Payrolls.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Pms.Main.FrontEnd.Common;
using Pms.Main.FrontEnd.Common.Utils;

namespace Pms.Main.FrontEnd.Wpf.Commands
{
    public class Save : IRelayCommand
    {
        private readonly MasterlistModel _model;
        private readonly MasterlistViewModel _viewModel;


        public Save(MasterlistViewModel viewModel, MasterlistModel model)
        {
            _model = model;
            _viewModel = viewModel;

            _canExecute = true;
        }

        public void Execute(object? parameter)
        {
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
        }

        protected bool _canExecute;

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => _canExecute;

        public void NotifyCanExecuteChanged() =>
            CanExecuteChanged?.Invoke(this, new EventArgs());

    }
}