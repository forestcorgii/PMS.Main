using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Pms.Main.FrontEnd.Wpf.Services;
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

namespace Pms.Main.FrontEnd.Wpf.ViewModels
{
    public class MainViewModel : ObservableValidator
    {
        private readonly MainStore _mainStore;
        private readonly NavigationStore _navigationStore;
        public ObservableObject CurrentViewModel => _navigationStore.CurrentViewModel;

        public string[] cutoffIds;
        public string[] CutoffIds
        {
            get => cutoffIds;
            private set => SetProperty(ref cutoffIds, value);
        }
        public string[] payrollCodes;
        public string[] PayrollCodes
        {
            get => payrollCodes;
            private set => SetProperty(ref payrollCodes, value);
        }

        private string cutoffId = "";
        [Required]
        [MinLength(6)]
        [MaxLength(7)]
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
                    ClearErrors(nameof(CutoffId));
                }
            }
        }


        private string payrollCode = "";
        [MaxLength(6)]
        [CustomValidation(typeof(MainViewModel), nameof(ValidatePayrollCode))]
        public string PayrollCode
        {
            get => payrollCode;
            set
            {
                SetProperty(ref payrollCode, value, true);
                if (PayrollCodeValidator.Validate(payrollCode))
                    _mainStore.SetPayrollCode(payrollCode);
            }
        }
        public static ValidationResult ValidatePayrollCode(string name, ValidationContext context)
        {
            MainViewModel instance = (MainViewModel)context.ObjectInstance;
            if (PayrollCodeValidator.Validate(instance.PayrollCode))
                return ValidationResult.Success;

            return new("Invalid Payroll Code.");
        }

        public ICommand TimesheetCommand { get; }
        public ICommand EmployeeCommand { get; }
        public ICommand BillingCommand { get; }
        public ICommand PayrollCommand { get; }
        public ICommand AlphalistCommand { get; }

        public ICommand LoadFilterCommand { get; }

        public MainViewModel(MainStore mainStore, NavigationStore navigationStore,
            NavigationService<TimesheetViewModel> timesheetNavigation,
            NavigationService<EmployeeViewModel> employeeNavigation,
            NavigationService<PayrollViewModel> payrollNavigation,
            NavigationService<AlphalistViewModel> alphalistNavigation,
            NavigationService<BillingViewModel> billingNavigation
        )
        {
            _navigationStore = navigationStore;
            _mainStore = mainStore;
            _mainStore.Reloaded += _cutoffStore_FiltersReloaded;

            TimesheetCommand = new NavigateCommand<TimesheetViewModel>(timesheetNavigation);
            EmployeeCommand = new NavigateCommand<EmployeeViewModel>(employeeNavigation);
            BillingCommand = new NavigateCommand<BillingViewModel>(billingNavigation);
            PayrollCommand = new NavigateCommand<PayrollViewModel>(payrollNavigation);
            AlphalistCommand = new NavigateCommand<AlphalistViewModel>(alphalistNavigation);

            cutoffIds = new string[] { };
            payrollCodes = new string[] { };

            LoadFilterCommand = new ListingCommand(_mainStore);
            LoadFilterCommand.Execute(null);

            TimesheetCommand.Execute(null);

            _navigationStore.CurrentViewModelChanged += OnCurrentViewModelChanged;
        }

        private void _cutoffStore_FiltersReloaded()
        {
            CutoffId = _mainStore.Cutoff.CutoffId;
            CutoffIds = _mainStore.CutoffIds;
            PayrollCodes = _mainStore.PayrollCodes;
        }


        private void OnCurrentViewModelChanged() =>
            OnPropertyChanged(nameof(CurrentViewModel));

    }
}
