﻿using Pms.Masterlists.Domain;
using Pms.Masterlists.Domain.Exceptions;
using Pms.Masterlists.ServiceLayer.HRMS.Exceptions;
using Pms.MasterlistModule.FrontEnd.Models;
using Pms.MasterlistModule.FrontEnd.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.Input;
using Pms.Main.FrontEnd.Common;
using Pms.Main.FrontEnd.Common.Utils;
using Pms.Masterlists.Domain.Entities.Employees;
using Pms.MasterlistModule.FrontEnd.Views.Widgets;

namespace Pms.MasterlistModule.FrontEnd.Commands.Employees_
{
    public class SyncResigned : IAsyncRelayCommand
    {
        private readonly Employees Model;
        private readonly EmployeeListingVm ListingVm;


        public SyncResigned(EmployeeListingVm listingVm, Employees model)
        {
            Model = model;
            ListingVm = listingVm;
            ListingVm.CanExecuteChanged += ListingVm_CanExecuteChanged; ;
        }

        public Task? ExecutionTask { get; }

        public bool CanBeCanceled => false;

        public bool IsCancellationRequested => false;

        public bool IsRunning => false;

        public event EventHandler? CanExecuteChanged;
        public event PropertyChangedEventHandler? PropertyChanged;

        public bool CanExecute(object? parameter) => ListingVm.Executable;



        public async void Execute(object? parameter) =>
            await ExecuteAsync(parameter);


        public async Task ExecuteAsync(object? parameter)
        {
            DateTime? selectedDate = null;
            if (parameter is DateTime dateParam)
                selectedDate = dateParam;

            if (selectedDate is null)
            {
                SelectDateWidget dateSelector = new();
                if (dateSelector.ShowDialog() is bool isSuccess && isSuccess)
                    selectedDate = dateSelector.SelectedDate;
            }
            if (selectedDate is not null)
            {

                List<Exception> exceptions = new();
                Employee[] employees = (await Model.SyncResignedAsync(selectedDate.Value, ListingVm.Site.ToString())).ToArray();

                ListingVm.SetProgress($"Found {employees.Length} resigned employees", employees.Length);
                await Task.Run(() =>
                {
                    if (employees.Length == 0) return;//exit if empty

                    try
                    {
                        foreach (Employee employee in employees)
                        {
                            try
                            {
                                if (employee is not null && employee.JobRemarks != "TRANSFERRED")
                                {
                                    employee.Active = false;
                                    Model.Save(employee);
                                }
                            }
                            catch (InvalidFieldValuesException ex) { exceptions.Add(ex); }
                            catch (InvalidFieldValueException ex) { exceptions.Add(ex); }
                            catch (DuplicateBankInformationException ex) { exceptions.Add(ex); }
                            catch (Exception ex) { exceptions.Add(ex); }


                            if (!ListingVm.IncrementProgress())
                                break;
                        }

                        Model.ReportExceptions(exceptions, new PayrollCode(), $"{ListingVm.Site}-RESIGNED");
                    }
                    catch (HttpRequestException) { MessageBoxes.Error("HTTP Request failed, please check Your HRMS Configuration."); }
                    catch (Exception ex) { MessageBoxes.Error(ex.Message); }


                });


                ListingVm.SetAsFinishProgress($"{exceptions.Count} error/s found.");
                ListingVm.LoadEmployees.Execute(null);
            }
        }


        private void ListingVm_CanExecuteChanged(object? sender, bool e) => NotifyCanExecuteChanged();
        public void NotifyCanExecuteChanged() => CanExecuteChanged?.Invoke(this, new EventArgs());

        public void Cancel() { }
    }
}
