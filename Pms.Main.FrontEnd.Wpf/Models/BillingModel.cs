using Pms.Adjustments.Domain;
using Pms.Adjustments.Domain.Services;
using Pms.Employees.ServiceLayer.EfCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Main.FrontEnd.Wpf.Models
{
    public class BillingModel 
    {
        private IManageBillingService _billingManager;
        private IProvideBillingService _billingProvider;
        private IGenerateBillingService _billingGenerater;


        public BillingModel(IManageBillingService manageBilling, IProvideBillingService provideBilling, IGenerateBillingService generateBilling)
        {
            _billingManager = manageBilling;
            _billingProvider = provideBilling;
            _billingGenerater = generateBilling;
        }


        public IEnumerable<Billing> GetBillings(string cutoffId) => _billingProvider.GetBillings(cutoffId);

        public double GetTotalAdvances(string eeId, string cutoffId) => _billingProvider.GetTotalAdvances(eeId, cutoffId);


        public IEnumerable<Billing> GenerateBillings(string cutoffId, string eeId)
        {
            _billingManager.ResetBillings(eeId, cutoffId);
            return _billingGenerater.GenerateBillingFromTimesheetView(eeId, cutoffId);
        }

        public void AddBilling(Billing billing) => _billingManager.AddBilling(billing);
    }
}
