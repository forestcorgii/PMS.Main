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
    public class UnknownTin : IRelayCommand
    {
        private readonly Employees _model;
        private readonly EmployeeListingVm ListingVm;


        public UnknownTin(EmployeeListingVm viewModel, Employees model)
        {
            _model = model;
            ListingVm = viewModel;
            ListingVm.CanExecuteChanged += ListingVm_CanExecuteChanged;
        }

        public async void Execute(object? parameter)
        {
            await Task.Run(() =>
            {
                ListingVm.SetProgress("Exporting Unknown TIN.", 1);
                try
                {
                    _model.ExportMasterlist(ListingVm.Employees.Where(e => string.IsNullOrEmpty(e.TIN)), ListingVm.PayrollCode, "NO TIN");
                }
                catch (Exception ex) { MessageBoxes.Error(ex.Message); }
                ListingVm.SetAsFinishProgress();
            });
            }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => ListingVm.Executable;
        private void ListingVm_CanExecuteChanged(object? sender, bool e) => NotifyCanExecuteChanged();
        public void NotifyCanExecuteChanged() =>
            CanExecuteChanged?.Invoke(this, new EventArgs());

    }
}
