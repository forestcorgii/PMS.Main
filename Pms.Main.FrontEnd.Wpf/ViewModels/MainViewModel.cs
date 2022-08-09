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
        private readonly MainStore _cutoffStore;
        private readonly NavigationStore _navigationStore;
        public ObservableObject CurrentViewModel => _navigationStore.CurrentViewModel;

        public List<string> cutoffIds;
        public List<string> CutoffIds
        {
            get => cutoffIds;
            private set => SetProperty(ref cutoffIds, value);
        }
        public List<string> payrollCodes;
        public List<string> PayrollCodes
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
                    _cutoffStore.SetCutoff(cutoff);
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
                    _cutoffStore.SetPayrollCode(payrollCode);
            }
        }
        public static ValidationResult ValidatePayrollCode(string name,ValidationContext context)
        {
            MainViewModel instance = (MainViewModel)context.ObjectInstance;
            if (PayrollCodeValidator.Validate(instance.PayrollCode))
                return ValidationResult.Success;

            return new("Invalid Payroll Code.");
        }

        public ICommand TimesheetCommand { get; }
        public ICommand EmployeeCommand { get; }
        public ICommand LoadFilterCommand { get; }

        public MainViewModel(MainStore cutoffStore, NavigationStore navigationStore, NavigationService<TimesheetViewModel> timesheetNavigation, NavigationService<EmployeeViewModel> employeeNavigation)
        {
            _navigationStore = navigationStore;
            _cutoffStore = cutoffStore;
            _cutoffStore.FiltersReloaded += _cutoffStore_FiltersReloaded;
            TimesheetCommand = new NavigateCommand<TimesheetViewModel>(timesheetNavigation);
            EmployeeCommand = new NavigateCommand<EmployeeViewModel>(employeeNavigation);

            cutoffIds = new List<string>();
            payrollCodes = new List<string>();

            LoadFilterCommand = new FilterListingCommand(_cutoffStore);
            LoadFilterCommand.Execute(null);

            _navigationStore.CurrentViewModelChanged += OnCurrentViewModelChanged;
        }

        private void _cutoffStore_FiltersReloaded()
        {
            CutoffId = _cutoffStore.Cutoff.CutoffId;
            CutoffIds = _cutoffStore.CutoffIds;
            PayrollCodes = _cutoffStore.PayrollCodes;
        }


        private void OnCurrentViewModelChanged()
        {
            OnPropertyChanged(nameof(CurrentViewModel));
        }

    }
}
