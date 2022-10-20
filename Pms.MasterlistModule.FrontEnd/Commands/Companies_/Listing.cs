using CommunityToolkit.Mvvm.Input;
using Pms.Main.FrontEnd.Common.Utils;
using Pms.MasterlistModule.FrontEnd.Models;
using Pms.MasterlistModule.FrontEnd.ViewModels;
using Pms.Masterlists.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.MasterlistModule.FrontEnd.Commands.Companies_
{
    public class Listing : IRelayCommand
    {
        public event EventHandler? CanExecuteChanged;
        private bool executable = true;

        private readonly Companies Companies;
        private readonly CompanyDetailVm _viewModel;


        public Listing(CompanyDetailVm viewModel, Companies companies)
        {
            _viewModel = viewModel;
            Companies = companies;
        }


        public bool CanExecute(object? parameter) => executable;


        public async void Execute(object? parameter)
        {
            executable = false;
            NotifyCanExecuteChanged();
            try
            {
                IEnumerable<PayrollCode> payrollCodes = new List<PayrollCode>();
                IEnumerable<Company> companies = new List<Company>();
                await Task.Run(() =>
                {
                    companies = Companies.ListCompanies();
                });

                _viewModel.Companies = new(companies);
            }
            catch (Exception ex) { MessageBoxes.Error(ex.Message); }

            executable = true;
            NotifyCanExecuteChanged();
        }

        public void NotifyCanExecuteChanged() =>
            CanExecuteChanged?.Invoke(this, new EventArgs());
    }

}
