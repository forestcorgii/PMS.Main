using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using Pms.AdjustmentModule.FrontEnd.ViewModels.Billing_Records;
using Pms.Adjustments.Domain.Models;
using Pms.Main.FrontEnd.Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.AdjustmentModule.FrontEnd.Commands.Billing_Records
{
    public class Import : IRelayCommand
    {
        public event EventHandler? CanExecuteChanged;

        private BillingRecordListingVm ViewModel;

        private Models.BillingRecords Records;

        public Import(BillingRecordListingVm viewModel, Models.BillingRecords model)
        {
            Records = model;
            ViewModel = viewModel;
        }

        private bool executable = true;
        public bool CanExecute(object? parameter) => executable;

        public async void Execute(object? parameter)
        {
            executable = false;
            NotifyCanExecuteChanged();
            await Task.Run(() =>
            {
                try
                {
                    OpenFileDialog openFile = new() { Multiselect = true, Filter = "Billing Record Import Files(*.xls)|*.xls" };
                    bool? isValid = openFile.ShowDialog();
                    if (isValid is not null && isValid == true)
                    {
                        foreach (string filename in openFile.FileNames)
                        {
                            IEnumerable<BillingRecord> records = Records.Import(filename);
                            ViewModel.SetProgress("Saving Extracted Records..", records.Count());
                            foreach (BillingRecord record in records)
                            {
                                Records.SaveRecord(record);
                                ViewModel.ProgressValue++;
                            }
                        }
                        ViewModel.SetAsFinishProgress();
                    }
                    MessageBoxes.Prompt("Import has been successfully saved.");
                }
                catch (Exception ex) { MessageBoxes.Error(ex.Message); }
            });

            executable = true;
            NotifyCanExecuteChanged();
        }
        public void NotifyCanExecuteChanged() => CanExecuteChanged?.Invoke(this, new EventArgs());


    }
}
