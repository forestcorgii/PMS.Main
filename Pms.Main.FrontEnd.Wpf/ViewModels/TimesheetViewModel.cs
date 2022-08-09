﻿using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Pms.Employees.Domain;
using Pms.Main.FrontEnd.Wpf.Commands;
using Pms.Main.FrontEnd.Wpf.Models;
using Pms.Main.FrontEnd.Wpf.Stores;
using Pms.Timesheets.BizLogic.Concrete;
using Pms.Timesheets.Domain;
using Pms.Timesheets.Domain.SupportTypes;
using Pms.Timesheets.Persistence;
using Pms.Timesheets.ServiceLayer.EfCore.Concrete;
using Pms.Timesheets.ServiceLayer.EfCore.Queries;
using Pms.Timesheets.ServiceLayer.Outputs;
using Pms.Timesheets.ServiceLayer.TimeSystem.Adapter;
using Pms.Timesheets.ServiceLayer.TimeSystem.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using static Pms.Timesheets.ServiceLayer.TimeSystem.Services.Enums;

namespace Pms.Main.FrontEnd.Wpf.ViewModels
{
    public class TimesheetViewModel : ViewModelBase
    {
        #region Properties
        public bool IsBusy { get; private set; }

        private CutoffStore _cutoffStore { get; set; }

        private DownloadOptions options = DownloadOptions.All;
        public DownloadOptions Options
        {
            get => options;
            set => SetProperty(ref options, value);
        }


        private ObservableCollection<Timesheet> _timesheets;
        public ObservableCollection<Timesheet> Timesheets
        {
            get => _timesheets;
            set => SetProperty(ref _timesheets, value);
        }
        #endregion

        public ICommand DownloadCommand { get; }
        public IAsyncRelayCommand EmployeeDownloadCommand { get; }
        public ICommand EvaluateCommand { get; }
        public ICommand ExportCommand { get; }
        public ICommand LoadFilterCommand { get; }


        public ICommand LoadTimesheetCommand { get; }


        public TimesheetViewModel(CutoffTimesheet cutoffTimesheet, EmployeeModel employeeModel, CutoffStore cutoffStore)
        {
            _cutoffStore = cutoffStore;
            _cutoffStore.TimesheetsReloaded += _cutoffStore_TimesheetsReloaded;

            LoadTimesheetCommand = new TimesheetListingCommand(this, cutoffStore);
            LoadFilterCommand = new FilterListingCommand(_cutoffStore);

            EmployeeDownloadCommand = new EmployeeDownloadCommand(this, cutoffStore, employeeModel);

            DownloadCommand = new TimesheetDownloadCommand(this, cutoffStore, cutoffTimesheet);
            EvaluateCommand = new TimesheetEvaluationCommand(this, cutoffTimesheet, cutoffStore);
            ExportCommand = new TimesheetExportCommand(this, cutoffTimesheet, cutoffStore);

            _timesheets = new();
            Timesheets = new ObservableCollection<Timesheet>(_cutoffStore.Timesheets);
        }

        #region Events
        private void _cutoffStore_TimesheetsReloaded()
        {
            Timesheets = new ObservableCollection<Timesheet>(_cutoffStore.Timesheets); 
        }
        #endregion
    }

}




//private string name;
//public string Name
//{
//    get => name;
//    set => SetProperty(ref name, value);
//}

//private string name;
//public string Name
//{
//    get => name;
//    set => SetProperty(ref name, value);
//}