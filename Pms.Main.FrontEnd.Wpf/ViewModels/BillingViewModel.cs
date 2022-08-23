﻿using Pms.Adjustments.Domain;
using Pms.Main.FrontEnd.Wpf.Commands;
using Pms.Main.FrontEnd.Wpf.Models;
using Pms.Main.FrontEnd.Wpf.Stores;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Pms.Main.FrontEnd.Wpf.ViewModels
{
    public class BillingViewModel : ViewModelBase
    {
        private ObservableCollection<Billing> _billings;
        public ObservableCollection<Billing> Billings
        {
            get => _billings;
            set => SetProperty(ref _billings, value);
        }

        public ICommand GenerateBillings { get; }
        public ICommand ListBillings { get; }
        public ICommand ExportBillings { get; }

        private BillingStore _store;
        private BillingModel _model;

        public BillingViewModel(BillingStore store, BillingModel model, MainStore mainStore, EmployeeModel employeeModel)
        {
            _store = store;
            _store.Reloaded += BillingsReloaded;
            
            _billings = new ObservableCollection<Billing>();
            Billings = new ObservableCollection<Billing>(_store.Billings);

            _model = model;

            ExportBillings = new ListingCommand( store);
            GenerateBillings = new BillingGenerationCommand(this, _model, store, mainStore, employeeModel);
            ListBillings = new ListingCommand( store);
            ListBillings.Execute(null);
        }

        public override void Dispose()
        {
            _store.Reloaded -= BillingsReloaded;
            base.Dispose();
        }

        public void BillingsReloaded()
        {
            Billings = new ObservableCollection<Billing>(_store.Billings);
        }
    }
}
