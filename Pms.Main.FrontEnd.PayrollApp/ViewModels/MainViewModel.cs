using Pms.Main.FrontEnd.PayrollApp.Commands;
using Pms.Timesheets.Domain.SupportTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Pms.Masterlists.Domain;
using Pms.Main.FrontEnd.Common;
using Pms.Main.FrontEnd.Common.Messages;
using Microsoft.Toolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using Pms.Masterlists.Domain.Enums;
using Pms.MasterlistModule.FrontEnd.ViewModels;
using Pms.MasterlistModule.FrontEnd;
using Pms.MasterlistModule.FrontEnd.Models;
using Pms.TimesheetModule.FrontEnd.Models;
using Pms.TimesheetModule.FrontEnd.ViewModels;
using Pms.PayrollModule.FrontEnd.ViewModels;
using Pms.AdjustmentModule.FrontEnd.ViewModels;

namespace Pms.Main.FrontEnd.PayrollApp.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private SiteChoices site = SiteChoices.MANILA;
        public SiteChoices Site
        {
            get => site;
            set
            {
                SetProperty(ref site, value);
                Messenger.Send(new SelectedSiteChangedMessage(Site));
            }
        }
        public ObservableCollection<SiteChoices> Sites =>
            new ObservableCollection<SiteChoices>(Enum.GetValues(typeof(SiteChoices)).Cast<SiteChoices>());


        public Company Company { get; set; } = new();
        private string companyId;
        public string CompanyId

        {
            get => companyId;
            set
            {
                SetProperty(ref companyId, value);
                Company = companies.Where(c => c.CompanyId == companyId).First();
                Messenger.Send(new SelectedCompanyChangedMessage(Company));
            }
        }
        private IEnumerable<Company> companies;
        public IEnumerable<Company> Companies { get => companies; set => SetProperty(ref companies, value); }





        public PayrollCode PayrollCode { get; set; }
        private string payrollCodeId;
        public string PayrollCodeId
        {
            get => payrollCodeId;
            set
            {
                SetProperty(ref payrollCodeId, value);
                PayrollCode = PayrollCodes.Where(c => c.PayrollCodeId == payrollCodeId).First();
                CompanyId = PayrollCode.CompanyId;
                Site = Sites.Where(s => s.ToString() == PayrollCode.Site).FirstOrDefault();
                Messenger.Send(new SelectedPayrollCodeChangedMessage(PayrollCode));
            }
        }
        private IEnumerable<PayrollCode> payrollCodes;
        public IEnumerable<PayrollCode> PayrollCodes { get => payrollCodes; set => SetProperty(ref payrollCodes, value); }





        public Cutoff Cutoff { get; set; }
        private string cutoffId = "";
        public string CutoffId
        {
            get => cutoffId;
            set
            {
                SetProperty(ref cutoffId, value, true);
                if (cutoffId.Length >= 6)
                    Messenger.Send(new SelectedCutoffIdChangedMessage(cutoffId));
            }
        }
        public string[] cutoffIds;
        public string[] CutoffIds { get => cutoffIds; set => SetProperty(ref cutoffIds, value); }




        private readonly NavigationStore _navigationStore;
        public ObservableObject CurrentViewModel => _navigationStore.CurrentViewModel;

        public ICommand TimesheetCommand { get; }
        public ICommand EmployeeCommand { get; }
        public ICommand BillingCommand { get; }
        public ICommand PayrollCommand { get; }
        public ICommand AlphalistCommand { get; }

        public ICommand LoadFilterCommand { get; }

        public MainViewModel(PayrollCodes payrollCodes, Companies companies, TimesheetModule.FrontEnd.Models.Timesheets timesheetModel, PayrollModule.FrontEnd.Models.Payrolls payrollModel,
            NavigationStore navigationStore,
            NavigationService<TimesheetListingVm> timesheetNavigation,
            NavigationService<EmployeeListingVm> employeeNavigation,
            NavigationService<PayrollViewModel> payrollNavigation,
            NavigationService<AlphalistViewModel> alphalistNavigation,
            NavigationService<BillingListingVm> billingNavigation
        )
        {
            cutoffIds = new string[] { };
           
            TimesheetCommand = new NavigateCommand<TimesheetListingVm>(timesheetNavigation);
            EmployeeCommand = new NavigateCommand<EmployeeListingVm>(employeeNavigation);
            BillingCommand = new NavigateCommand<BillingListingVm>(billingNavigation);
            PayrollCommand = new NavigateCommand<PayrollViewModel>(payrollNavigation);
            AlphalistCommand = new NavigateCommand<AlphalistViewModel>(alphalistNavigation);


            LoadFilterCommand = new Listing(this, payrollModel, timesheetModel, payrollCodes, companies);
            LoadFilterCommand.Execute(null);

            IsActive = true;

            TimesheetCommand.Execute(null);

            _navigationStore = navigationStore;
            _navigationStore.CurrentViewModelChanged += OnCurrentViewModelChanged;
        }

        private void OnCurrentViewModelChanged() =>
            OnPropertyChanged(nameof(CurrentViewModel));


        protected override void OnActivated()
        {
            Messenger.Register<MainViewModel, CurrentSiteRequestMessage>(this, (r, m) => m.Reply(r.Site));
            Messenger.Register<MainViewModel, CurrentCompanyRequestMessage>(this, (r, m) => m.Reply(r.Company));
            Messenger.Register<MainViewModel, CurrentPayrollCodeRequestMessage>(this, (r, m) => m.Reply(r.PayrollCode));
            Messenger.Register<MainViewModel, CurrentCutoffIdRequestMessage>(this, (r, m) => m.Reply(r.CutoffId));
        }



    }
}
