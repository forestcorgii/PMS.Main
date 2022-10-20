using CommunityToolkit.Mvvm.ComponentModel;
using Pms.AdjustmentModule.FrontEnd.Commands.Billing_Records;
using Pms.AdjustmentModule.FrontEnd.Models;
using Pms.Adjustments.Domain.Enums;
using Pms.Adjustments.Domain.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Pms.AdjustmentModule.FrontEnd.ViewModels.Billing_Records
{
    public class BillingRecordDetailVm : ObservableObject
    {
        private BillingRecord record;
        public BillingRecord Record { get => record; set => SetProperty(ref record, value); }

        private string eeId;
        public string EEId
        {
            get => eeId;
            set
            {
                SetProperty(ref eeId, value);

                record.EE = Employees.Find(value);
                if (record.EE is not null)
                {
                    record.EEId = eeId;
                    Fullname = record.EE.Fullname;
                }

            }
        }

        private string fullname;
        public string Fullname { get => fullname; set => SetProperty(ref fullname, value); }


        public ObservableCollection<AdjustmentTypes> AdjustmentTypes =>
            new ObservableCollection<AdjustmentTypes>(Enum.GetValues(typeof(AdjustmentTypes)).Cast<AdjustmentTypes>());

        public ObservableCollection<DeductionOptions> DeductionOptions =>
            new ObservableCollection<DeductionOptions>(Enum.GetValues(typeof(DeductionOptions)).Cast<DeductionOptions>());

        public ObservableCollection<BillingRecordStatus> BillingRecordStatus =>
            new ObservableCollection<BillingRecordStatus>(Enum.GetValues(typeof(BillingRecordStatus)).Cast<BillingRecordStatus>());


        private Employees Employees;

        public ICommand Save { get; }

        public event EventHandler OnRequestClose;

        public BillingRecordDetailVm(BillingRecord record, BillingRecords model, Employees employees)
        {
            Employees = employees;
            Record = record;

            if (record.EEId is not null)
            {
                EEId = record.EEId;
                Fullname = record.EE.Fullname;
            }

            Save = new Save(this, model);
        }


        public void Close() => OnRequestClose?.Invoke(this, new EventArgs());
    }
}
