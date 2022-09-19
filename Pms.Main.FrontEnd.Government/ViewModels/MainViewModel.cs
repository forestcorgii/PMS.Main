using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Pms.Adjustments.Domain.Models;
using Pms.Masterlists.Domain; 
using Pms.Main.FrontEnd.Government.Commands;
using Pms.Main.FrontEnd.Common;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Pms.Main.FrontEnd.Government.ViewModels
{
    public class MainViewModel : ObservableValidator
    {
        //private readonly NavigationStore _navigationStore;
        //public ObservableObject CurrentViewModel => _navigationStore.CurrentViewModel;

        public string[] cutoffIds;
        public string[] CutoffIds
        {
            get => cutoffIds;
            private set => SetProperty(ref cutoffIds, value);
        }
        public IEnumerable<PayrollCode> payrollCodes;
        public IEnumerable<PayrollCode> PayrollCodes
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
                    ClearErrors(nameof(CutoffId));
                }
            }
        }


        private string payrollCode = "";
        [MaxLength(6)]
        public string PayrollCode
        {
            get => payrollCode;
            set=>
                SetProperty(ref payrollCode, value, true);
        }

        public ICommand PayrollCommand { get; }

        public ICommand LoadFilterCommand { get; }

        public MainViewModel(MainStore mainStore, 
            NavigationService<PayrollListingViewModel> payrollNavigation,
            NavigationService<PayrollDetailViewModel> payrollDetailNavigation
        )
        {
            //_navigationStore = navigationStore;
            _mainStore = mainStore;
            _mainStore.Reloaded += _cutoffStore_FiltersReloaded;

            PayrollCommand = new NavigateCommand<PayrollListingViewModel>(payrollNavigation);

            cutoffIds = new string[] { };
            payrollCodes = new List<PayrollCode>();

            LoadFilterCommand = new ListingCommand(_mainStore);
            LoadFilterCommand.Execute(null);

            //_navigationStore.CurrentViewModelChanged += OnCurrentViewModelChanged;
        }

        private void _cutoffStore_FiltersReloaded()
        {
            CutoffId = _mainStore.Cutoff.CutoffId;
            CutoffIds = _mainStore.CutoffIds;
            PayrollCodes = _mainStore.PayrollCodes;
        }


        //private void OnCurrentViewModelChanged() =>
        //    OnPropertyChanged(nameof(CurrentViewModel));

    }
}
