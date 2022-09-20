using Pms.Adjustments.Domain;
using Pms.Adjustments.Domain.Services;
using Pms.Adjustments.ServiceLayer.Files;
using System;
using System.Collections.Generic;

namespace Pms.AdjustmentModule.FrontEnd.Models
{
    public class Billings
    {
        private IManageBillingService _billingManager;
        private IProvideBillingService _billingProvider;
        private IGenerateBillingService _billingGenerator;
        private BillingExporter _billingExporter;


        public Billings(IManageBillingService manageBilling, IProvideBillingService provideBilling, IGenerateBillingService generateBilling, BillingExporter billingExporter)
        {
            _billingManager = manageBilling;
            _billingProvider = provideBilling;
            _billingGenerator = generateBilling;
            _billingExporter = billingExporter;
        }


        public IEnumerable<Billing> GetBillings(string cutoffId) => _billingProvider.GetBillings(cutoffId);


        public IEnumerable<string> GetEmployeesWithPcv(string cutoffId) =>
            _billingGenerator.CollectEEIdWithPcv(cutoffId);

        public double GetTotalAdvances(string eeId, string cutoffId) => _billingProvider.GetTotalAdvances(eeId, cutoffId);


        public IEnumerable<Billing> GenerateBillings(string cutoffId, string eeId)
        {
            _billingManager.ResetBillings(eeId, cutoffId);
            return _billingGenerator.GenerateBillingFromTimesheetView(eeId, cutoffId);
        }

        public void AddBilling(Billing billing) => _billingManager.AddBilling(billing);

        public void Export(IEnumerable<Billing> billings, string adjustmentName, string filename) =>
            _billingExporter.ExportBillings(billings, adjustmentName, filename);
    }
}
