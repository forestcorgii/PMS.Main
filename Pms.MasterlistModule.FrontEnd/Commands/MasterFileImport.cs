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

namespace Pms.MasterlistModule.FrontEnd.Commands
{
    public class MasterFileImport : IRelayCommand
    {
        private readonly Employees _model;
        private readonly EmployeeListingVm _viewModel;


        public MasterFileImport(EmployeeListingVm viewModel, Employees model)
        {
            _model = model;
            _viewModel = viewModel;

        }

        public async void Execute(object? parameter)
        {
            executable = false;
            await Task.Run(() =>
            {
                _viewModel.SetProgress("Select Master file.", 0);

                OpenFileDialog openFile = new();
                bool? isValid = openFile.ShowDialog();
                if (isValid is not null && isValid == true)
                {
                    try
                    {
                        IEnumerable<IMasterFileInformation> extractedEmployee = _model.ImportMasterFile(openFile.FileName);

                        _viewModel.SetProgress($"Saving Extracted employees job codes from {Path.GetFileName(openFile.FileName)}.", extractedEmployee.Count());
                        foreach (IMasterFileInformation employee in extractedEmployee)
                        {
                            _model.Save(employee);
                            _viewModel.ProgressValue++;
                        }
                    }
                    catch (Exception ex) { MessageBoxes.Error(ex.Message); }
                    _viewModel.SetAsFinishProgress();
                }
            });
            executable = true;
        }

        protected bool executable = true;

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => executable;

        public void NotifyCanExecuteChanged() =>
            CanExecuteChanged?.Invoke(this, new EventArgs());

    }
}
