using CommunityToolkit.Mvvm.ComponentModel;
using Pms.TimesheetModule.FrontEnd.Commands;
using Pms.Timesheets.Domain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using static Pms.Payrolls.Domain.TimesheetEnums;

namespace Pms.TimesheetModule.FrontEnd.ViewModels
{
    public class TimesheetDetailVm : ObservableObject
    {
        public List<TimesheetBankChoices> BankChoices =>
            new List<TimesheetBankChoices>(Enum.GetValues(typeof(TimesheetBankChoices)).Cast<TimesheetBankChoices>());


        private Timesheet timesheet = new();
        public Timesheet Timesheet { get => timesheet; set => SetProperty(ref timesheet, value); }

        public ICommand Save { get; }

        public event EventHandler OnRequestClose;

        public TimesheetDetailVm(Models.Timesheets timesheets, Timesheet timesheet)
        {
            Timesheet = timesheet;
            Save = new Save(this, timesheets);
        }

        public void Close() => OnRequestClose?.Invoke(this, new EventArgs());

    }
}
