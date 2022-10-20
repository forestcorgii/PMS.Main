using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using Pms.Main.FrontEnd.Common.Utils;
using Pms.MasterlistModule.FrontEnd.Models;
using Pms.MasterlistModule.FrontEnd.ViewModels;
using Pms.Masterlists.Domain.Entities.Employees;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.MasterlistModule.FrontEnd.Commands.Employees_
{
    public class MasterlistExport : IRelayCommand
    {
        private readonly Employees _model;
        private readonly EmployeeListingVm _viewModel;


        public MasterlistExport(EmployeeListingVm viewModel, Employees model)
        {
            _model = model;
            _viewModel = viewModel;
        }

        public async void Execute(object? parameter)
        {
            executable = false;
            NotifyCanExecuteChanged();
            await Task.Run(() =>
            {
                try
                {
                    _model.ExportMasterlist(_viewModel.Employees, _viewModel.PayrollCode);
                }
                catch (Exception ex) { MessageBoxes.Error(ex.Message); }
                _viewModel.SetAsFinishProgress();
            });
            executable = true;
            NotifyCanExecuteChanged();
        }

        protected bool executable = true;

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => executable;

        public void NotifyCanExecuteChanged() =>
            CanExecuteChanged?.Invoke(this, new EventArgs());

    }
}
