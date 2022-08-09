using Microsoft.Toolkit.Mvvm.Input;
using Pms.Employees.Domain;
using Pms.Main.FrontEnd.Wpf.Models;
using Pms.Main.FrontEnd.Wpf.Stores;
using Pms.Main.FrontEnd.Wpf.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Main.FrontEnd.Wpf.Commands
{
    public class EmployeeDownloadCommand : IAsyncRelayCommand
    {
        private ViewModelBase _viewModel;
        private EmployeeModel _employeeModel;
        private CutoffStore _cutoffStore;

        public Task? ExecutionTask => throw new NotImplementedException();

        public bool CanBeCanceled => false;

        public bool IsCancellationRequested => false;

        public bool IsRunning => false;

        public event EventHandler? CanExecuteChanged;
        public event PropertyChangedEventHandler? PropertyChanged;

        public EmployeeDownloadCommand(ViewModelBase viewModel, CutoffStore cutoffStore, EmployeeModel employeeModel)
        {
            _viewModel = viewModel;
            _cutoffStore = cutoffStore;
            _employeeModel = employeeModel;

        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public async void Execute(object? parameter)=>
            await ExecuteAsync(parameter);


        public async Task ExecuteAsync(object? parameter)
        {
            string[] eeIds;
            if (parameter is not null && parameter is string[])
                eeIds = (string[])parameter;
            else
                eeIds = _cutoffStore.Employees.Select(ee => ee.EEId).ToArray();

            _viewModel.SetProgress("Syncing Unknown Employees", eeIds.Length);

            foreach (string eeId in eeIds)
            {
                try
                {
                    Employee? employeeFound = await _employeeModel.FindEmployeeAsync(eeId, _cutoffStore.Site);
                    if (employeeFound is null)
                        employeeFound = new Employee() { EEId = eeId, Active = false };

                    _employeeModel.SaveEmployee(employeeFound);
                    _viewModel.ProgressValue++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            _viewModel.SetAsFinishProgress();
        }




        public void NotifyCanExecuteChanged() { }
        public void Cancel() { }
    }
}
