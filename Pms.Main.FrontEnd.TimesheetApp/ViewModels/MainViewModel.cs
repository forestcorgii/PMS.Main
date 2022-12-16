using Pms.Main.FrontEnd.TimesheetApp.Commands;
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

namespace Pms.Main.FrontEnd.TimesheetApp.ViewModels
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
                if (!string.IsNullOrEmpty(companyId))
                    Company = companies.Where(c => c.CompanyId == companyId).First();
                else Company = new();

                Messenger.Send(new SelectedCompanyChangedMessage(Company));
            }
        }
        private IEnumerable<Company> companies;
        public IEnumerable<Company> Companies { get => companies; set => SetProperty(ref companies, value); }





        public PayrollCode PayrollCode { get; set; } = new() { PayrollCodeId = string.Empty };
        private string payrollCodeId;
        public string PayrollCodeId
        {
            get => payrollCodeId;
            set
            {
                SetProperty(ref payrollCodeId, value);

                if (!string.IsNullOrEmpty(payrollCodeId))
                    PayrollCode = PayrollCodes.Where(c => c.PayrollCodeId == payrollCodeId).First();
                else PayrollCode = new() { PayrollCodeId = string.Empty };

                CompanyId = PayrollCode.CompanyId;
                Site = Sites.Where(s => s.ToString() == PayrollCode.Site).FirstOrDefault();
                Messenger.Send(new SelectedPayrollCodeChangedMessage(PayrollCode));
            }
        }
        private IEnumerable<PayrollCode> payrollCodes;
        public IEnumerable<PayrollCode> PayrollCodes
        {
            get => payrollCodes;
            set
            {
                SetProperty(ref payrollCodes, value);
                Messenger.Send(new SelectedPayrollCodesChangedMessage(PayrollCodes.Select(p => p.PayrollCodeId).ToArray()));
            }
        }




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
        public ICommand BillingRecordCommand { get; }
        public ICommand PayrollCommand { get; }
        public ICommand AlphalistCommand { get; }

        public ICommand LoadFilterCommand { get; }

        public MainViewModel(PayrollCodes payrollCodes, Companies companies, TimesheetModule.FrontEnd.Models.Timesheets timesheetModel,
            NavigationStore navigationStore,
            NavigationService<TimesheetListingVm> timesheetNavigation,
            NavigationService<EmployeeListingVm> employeeNavigation
            )
        {
            cutoffIds = new string[] { };

            TimesheetCommand = new NavigateCommand<TimesheetListingVm>(timesheetNavigation);
            EmployeeCommand = new NavigateCommand<EmployeeListingVm>(employeeNavigation);
            
            IsActive = true;

            LoadFilterCommand = new Listing(this, timesheetModel, payrollCodes, companies);
            LoadFilterCommand.Execute(null);


            TimesheetCommand.Execute(null);

            _navigationStore = navigationStore;
            _navigationStore.CurrentViewModelChanged += OnCurrentViewModelChanged;
        }

        private void OnCurrentViewModelChanged() =>
            OnPropertyChanged(nameof(CurrentViewModel));


        protected override void OnActivated()
        {
            Messenger.Register<MainViewModel, CurrentPayrollCodesRequestMessage>(this, (r, m) =>
            {
                m.Reply(r.PayrollCodes.Select(p => p.PayrollCodeId).ToArray());
            });

            Messenger.Register<MainViewModel, CurrentSiteRequestMessage>(this, (r, m) => m.Reply(r.Site));
            Messenger.Register<MainViewModel, CurrentCompanyRequestMessage>(this, (r, m) => m.Reply(r.Company));
            Messenger.Register<MainViewModel, CurrentPayrollCodeRequestMessage>(this, (r, m) => m.Reply(r.PayrollCode));
            Messenger.Register<MainViewModel, CurrentCutoffIdRequestMessage>(this, (r, m) => m.Reply(r.CutoffId));
        }



    }
}
