using CommunityToolkit.Mvvm.ComponentModel;
using Pms.MasterlistModule.FrontEnd.Commands.Companies_;
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
    public class CompanyDetailVm : ObservableObject
    {
        private Company selectedCompany;
        public Company SelectedCompany { get => selectedCompany; set => SetProperty(ref selectedCompany, value); }

        private ObservableCollection<Company> companies;
        public ObservableCollection<Company> Companies{ get => companies; set => SetProperty(ref companies, value); }

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

        
        public ICommand Save { get; }
        public ICommand Listing { get; }

        public CompanyDetailVm(Companies companiesM)
        {
            SelectedCompany = new();

            Save = new Save(this, companiesM);
            Listing = new Listing(this, companiesM);
            Listing.Execute(null);
        }
    }
}
