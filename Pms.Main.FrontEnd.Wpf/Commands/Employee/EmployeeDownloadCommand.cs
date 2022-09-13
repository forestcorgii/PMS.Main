using Microsoft.Toolkit.Mvvm.Input;
using Pms.Employees.Domain;
using Pms.Employees.Domain.Exceptions;
using Pms.Employees.ServiceLayer.HRMS.Exceptions;
using Pms.Main.FrontEnd.Wpf.Models;
using Pms.Main.FrontEnd.Wpf.Stores;
using Pms.Main.FrontEnd.Wpf.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Pms.Main.FrontEnd.Wpf.Commands
{
    public class EmployeeDownloadCommand : IAsyncRelayCommand
    {
        private ViewModelBase _viewModel;
        private EmployeeModel _model;
        private EmployeeStore _store;
        private MainStore _mainStore;

        public Task? ExecutionTask { get; }

        public bool CanBeCanceled => false;

        public bool IsCancellationRequested => false;

        public bool IsRunning => false;

        public event EventHandler? CanExecuteChanged;
        public event PropertyChangedEventHandler? PropertyChanged;

        public EmployeeDownloadCommand(ViewModelBase viewModel, MainStore mainStore, EmployeeStore store, EmployeeModel model)
        {
            _viewModel = viewModel;
            _mainStore = mainStore;
            _store = store;
            _model = model;

        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public async void Execute(object? parameter) =>
            await ExecuteAsync(parameter);


        public async Task ExecuteAsync(object? parameter)
        {
            string[] eeIds;
            if (parameter is not null && parameter is string[])
                eeIds = (string[])parameter;
            else
                eeIds = _store.Employees.Select(ee => ee.EEId).ToArray();

            _viewModel.SetProgress("Syncing Unknown Employees", eeIds.Length);

            try
            {
                foreach (string eeId in eeIds)
                {
                    try
                    {
                        IPersonalInformation employee;
                        IPersonalInformation employeeFoundOnServer = await _model.FindEmployeeAsync(eeId, _mainStore.Site);
                        IPersonalInformation employeeFoundLocally = _model.FindEmployee(eeId);

                        if (employeeFoundOnServer is null && employeeFoundLocally is null)
                        {
                            employee = new Employee() { EEId = eeId, Active = false };
                            _model.Save(employee);
                        }
                        else if (employeeFoundOnServer is null && employeeFoundLocally is not null)
                        {
                            employeeFoundLocally.Active = false;
                            _model.Save(employeeFoundLocally);
                        }
                        else if (employeeFoundOnServer is not null)
                        {
                            employeeFoundOnServer.Active = true;
                            _model.Save(employeeFoundOnServer);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message,
                                "Employee Sync Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error
                            );
                        break;
                    }

                    _viewModel.ProgressValue++;
                }
            }
            catch (HttpRequestException)
            {
                _viewModel.StatusMessage = "HTTP Request failed, please check Your HRMS Configuration.";
            }
            _viewModel.SetAsFinishProgress();
            await _store.Reload();
        }




        public void NotifyCanExecuteChanged() { }
        public void Cancel() { }
    }
}
