using Pms.Main.FrontEnd.Wpf.Stores;
using Pms.Main.FrontEnd.Wpf.Commands;
using Pms.Timesheets.Domain.SupportTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Pms.Masterlists.Domain;
using Pms.Main.FrontEnd.Common;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Pms.Main.FrontEnd.Wpf.Messages;
using Microsoft.Toolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;

namespace Pms.Main.FrontEnd.Wpf.ViewModels
{
    public class MainViewModel : ObservableRecipient
    {
        private string companyName;
        public string CompanyName
        {
            get => companyName; 
            set
            {
                SetProperty(ref companyName, value);
                Company = companies.Where(c => c.CompanyId == companyName).First();
                SendCompanyMessage();
            }
        }
        
        private Company Company;
        
        private IEnumerable<Company> companies;
        public IEnumerable<Company> Companies { get => companies; set => SetProperty(ref companies, value); }

        private IEnumerable<PayrollCode> payrollCodes;
        public IEnumerable<PayrollCode> PayrollCodes { get => payrollCodes; set => SetProperty(ref payrollCodes, value); }
        private string payrollCode;
        public string PayrollCode
        {
            get => payrollCode;
            set
            {
                SetProperty(ref payrollCode, value);
                _mainStore.SetPayrollCode(payrollCode);
            }
        }

        public string[] cutoffIds;
        public string[] CutoffIds
        {
            get => cutoffIds;
            private set => SetProperty(ref cutoffIds, value);
        }
        private string cutoffId = "";

        public string CutoffId
        {
            get => cutoffId;
            set
            {
                SetProperty(ref cutoffId, value, true);

                if (cutoffId.Length >= 6)
                {
                    Cutoff cutoff = new Cutoff(cutoffId);
                    _mainStore.SetCutoff(cutoff);
                }
            }
        }

        private readonly MainStore _mainStore;
        private readonly NavigationStore _navigationStore;
        public ObservableObject CurrentViewModel => _navigationStore.CurrentViewModel;

        public ICommand TimesheetCommand { get; }
        public ICommand EmployeeCommand { get; }
        public ICommand BillingCommand { get; }
        public ICommand PayrollCommand { get; }
        public ICommand AlphalistCommand { get; }

        public ICommand LoadFilterCommand { get; }

        public MainViewModel(MainStore mainStore, NavigationStore navigationStore,
            NavigationService<TimesheetViewModel> timesheetNavigation,
            NavigationService<MasterlistViewModel> employeeNavigation,
            NavigationService<PayrollViewModel> payrollNavigation,
            NavigationService<AlphalistViewModel> alphalistNavigation,
            NavigationService<BillingViewModel> billingNavigation
        )
        {
            _navigationStore = navigationStore;
            _mainStore = mainStore;
            _mainStore.Reloaded += _cutoffStore_FiltersReloaded;

            TimesheetCommand = new NavigateCommand<TimesheetViewModel>(timesheetNavigation);
            EmployeeCommand = new NavigateCommand<MasterlistViewModel>(employeeNavigation);
            BillingCommand = new NavigateCommand<BillingViewModel>(billingNavigation);
            PayrollCommand = new NavigateCommand<PayrollViewModel>(payrollNavigation);
            AlphalistCommand = new NavigateCommand<AlphalistViewModel>(alphalistNavigation);

            cutoffIds = new string[] { };

            LoadFilterCommand = new ListingCommand(_mainStore);
            LoadFilterCommand.Execute(null);

            TimesheetCommand.Execute(null);

            _navigationStore.CurrentViewModelChanged += OnCurrentViewModelChanged;
            IsActive = true;
        }

        private void _cutoffStore_FiltersReloaded()
        {
            CutoffId = _mainStore.Cutoff.CutoffId;
            CutoffIds = _mainStore.CutoffIds;
            PayrollCodes = _mainStore.PayrollCodes;
            Companies = _mainStore.Companies;
        }


        private void OnCurrentViewModelChanged() =>
            OnPropertyChanged(nameof(CurrentViewModel));


        protected override void OnActivated()
        {
            Messenger.Register<MainViewModel, CurrentCompanyRequestMessage>(this, (r, m) => m.Reply(r.Company));
        }

        public void SendCompanyMessage()
        {
            Messenger.Send(new SelectedCompanyChangedMessage(Company));
        }
    }
}
