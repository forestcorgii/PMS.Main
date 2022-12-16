using Microsoft.Toolkit.Mvvm.Input;
using Pms.Main.FrontEnd.Common.Utils;
using Pms.TimesheetModule.FrontEnd.Models;
using Pms.TimesheetModule.FrontEnd.ViewModels;
using Pms.TimesheetModule.FrontEnd.Views;
using Pms.Timesheets.Domain;
using Pms.Timesheets.Domain.SupportTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Pms.TimesheetModule.FrontEnd.Commands
{
    public class Detail : IRelayCommand
    {
        private Models.Timesheets Timesheets;
        private TimesheetListingVm ListingVm;
        public Detail(TimesheetListingVm viewModel, Models.Timesheets timesheets)
        {
            Timesheets = timesheets;
            ListingVm = viewModel;
            ListingVm.CanExecuteChanged += ListingVm_CanExecuteChanged;
        }

        public event EventHandler? CanExecuteChanged;

        public void Execute(object? parameter)
        {
            try
            {
                TimesheetDetailVm detailVm;
                if (parameter is Timesheet timesheet)
                    detailVm = new(Timesheets, timesheet);
                else detailVm = new(Timesheets, new() { CutoffId = ListingVm.Cutoff.CutoffId });

                TimesheetDetailView detailView = new() { DataContext = detailVm };

                detailVm.OnRequestClose += (s, e) => detailView.Close();

                detailView.ShowDialog();
            }
            catch (Exception ex) { MessageBoxes.Error(ex.Message); }

        }

        public bool CanExecute(object? parameter) => ListingVm.Executable;
        private void ListingVm_CanExecuteChanged(object? sender, bool e) => NotifyCanExecuteChanged();
        public void NotifyCanExecuteChanged() =>
            CanExecuteChanged?.Invoke(this, new EventArgs());
    }
}
