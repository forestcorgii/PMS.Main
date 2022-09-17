using Pms.Masterlists.Domain;
using Pms.Main.FrontEnd.Wpf.Commands;
using Pms.Main.FrontEnd.Wpf.Models;
using Pms.Main.FrontEnd.Wpf.Stores;
using Pms.Timesheets.BizLogic.Concrete;
using Pms.Timesheets.Domain;
using Pms.Timesheets.Domain.SupportTypes;
using Pms.Timesheets.Persistence;
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
using Pms.Main.FrontEnd.Wpf.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace Pms.Main.FrontEnd.Wpf.ViewModels
{
    public class TimesheetViewModel : ViewModelBase
    {
        #region Properties
        public bool IsBusy { get; private set; }

        private TimesheetStore _timesheetStore { get; set; }

        private DownloadOptions options = DownloadOptions.All;
        public DownloadOptions Options
        {
            get => options;
            set => SetProperty(ref options, value);
        }


        private ObservableCollection<Timesheet> _timesheets;
        private Company company1;

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


        public TimesheetViewModel(TimesheetModel cutoffTimesheet, MasterlistModel employeeModel, MainStore cutoffStore, TimesheetStore timesheetStore, MasterlistStore employeeStore)
        {
            _timesheetStore = timesheetStore;
            _timesheetStore.Reloaded += _cutoffStore_TimesheetsReloaded;

            LoadTimesheetCommand = new ListingCommand(_timesheetStore);
            LoadFilterCommand = new FilterListingCommand(cutoffStore);

            EmployeeDownloadCommand = new Download(this, cutoffStore, employeeStore, employeeModel);

            DownloadCommand = new TimesheetDownloadCommand(this, cutoffStore, cutoffTimesheet);
            EvaluateCommand = new TimesheetEvaluationCommand(this, cutoffTimesheet, cutoffStore);
            ExportCommand = new TimesheetExportCommand(this, cutoffTimesheet, cutoffStore);

            _timesheets = new();
            Timesheets = new ObservableCollection<Timesheet>(_timesheetStore.Timesheets);

            IsActive = true;
        }

        public override void Dispose()
        {
            _timesheetStore.Reloaded -= _cutoffStore_TimesheetsReloaded;
            base.Dispose();
        }
        private void _cutoffStore_TimesheetsReloaded()
        {
            Timesheets = new ObservableCollection<Timesheet>(_timesheetStore.Timesheets);
        }


        public Company Company { get; set; }
        public PayrollCode PayrollCode { get; set; }
        public Cutoff Cutoff { get; set; }
        protected override void OnActivated()
        {
            Messenger.Register<TimesheetViewModel, SelectedCompanyChangedMessage>(this, (r, m) => r.Company = m.Value);
            Messenger.Register<TimesheetViewModel, SelectedPayrollCodeChangedMessage>(this, (r, m) => r.PayrollCode = m.Value);
            Messenger.Register<TimesheetViewModel, SelectedCutoffChangedMessage>(this, (r, m) => r.Cutoff = m.Value);
        }
    }
}

