using CommunityToolkit.Mvvm.ComponentModel;
using Pms.MasterlistModule.FrontEnd.Commands.Payroll_Codes;
using Pms.MasterlistModule.FrontEnd.Models;
using Pms.Masterlists.Domain;
using Pms.Masterlists.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Pms.MasterlistModule.FrontEnd.ViewModels
{
    public class PayrollCodeDetailVm : ObservableObject
    {
        private PayrollCode selectedPayrollCode;
        public PayrollCode SelectedPayrollCode { get => selectedPayrollCode; set => SetProperty(ref selectedPayrollCode, value); }

        private ObservableCollection<PayrollCode> payrollCodes;
        public ObservableCollection<PayrollCode> PayrollCodes { get => payrollCodes; set => SetProperty(ref payrollCodes, value); }

        private ObservableCollection<string> companyIds;
        public ObservableCollection<string> CompanyIds { get => companyIds; set => SetProperty(ref companyIds, value); }

        public ObservableCollection<string> Sites
        {
            get
            {
                List<string> sites = new();
                foreach (SiteChoices site in Enum.GetValues(typeof(SiteChoices)))
                    sites.Add(site.ToString());

                return new ObservableCollection<string>(sites);
            }
        }

        public ObservableCollection<PayrollRegisterTypes> ProcessTypes =>
            new ObservableCollection<PayrollRegisterTypes>(Enum.GetValues(typeof(PayrollRegisterTypes)).Cast<PayrollRegisterTypes>());

        public ICommand Save { get; }
        public ICommand Listing { get; }

        public PayrollCodeDetailVm(PayrollCodes payrollCodesM, Companies companiesM)
        {
            SelectedPayrollCode = new();
            PayrollCodes = new();

            Save = new Save(this, payrollCodesM);
            Listing = new Listing(this, payrollCodesM, companiesM);
            Listing.Execute(null);
        }

    }
}
