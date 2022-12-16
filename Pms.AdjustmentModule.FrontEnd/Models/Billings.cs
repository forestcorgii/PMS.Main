using Pms.Adjustments.Domain;
using Pms.Adjustments.Domain.Enums;
using Pms.Adjustments.Domain.Services;
using Pms.Adjustments.ServiceLayer.Files;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public IEnumerable<Billing> GetBillings(string cutoffId, string payrollCodeId) =>
            _billingProvider.GetBillings(cutoffId).Where(p => p.EE.PayrollCode == payrollCodeId);


        public IEnumerable<string> GetEmployeesWithPcv(string payrollCodeId, string cutoffId) =>
            _billingGenerator.CollectEEIdWithPcv(payrollCodeId, cutoffId);

        public IEnumerable<string> GetEmployeesWithBillingRecord(string payrollCodeId, string cutoffId) =>
                    _billingGenerator.CollectEEIdWithBillingRecord(payrollCodeId, cutoffId);

        public double GetTotalAdvances(string eeId, string cutoffId) => _billingProvider.GetTotalAdvances(eeId, cutoffId);


        public void ResetBillings(string cutoffId, string eeId) =>
            _billingManager.ResetBillings(eeId, cutoffId);


        public IEnumerable<Billing> GenerateBillingFromTimesheetView(string cutoffId, string eeId) =>
            _billingGenerator.GenerateBillingFromTimesheetView(eeId, cutoffId);

        public IEnumerable<Billing> GenerateBillingFromBillingRecord(string cutoffId, string eeId) =>
                    _billingGenerator.GenerateBillingFromRecords(eeId, cutoffId);

        public void AddBilling(Billing billing) => _billingManager.AddBilling(billing);

        public void Export(IEnumerable<Billing> billings, string cutoffId, string payrollCodeId, AdjustmentTypes adjustmentName) =>
            _billingExporter.ExportBillings(billings, cutoffId, payrollCodeId, adjustmentName);
    }
}
