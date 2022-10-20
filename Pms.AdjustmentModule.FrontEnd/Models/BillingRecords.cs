using Pms.Adjustments.Domain;
using Pms.Adjustments.Domain.Enums;
using Pms.Adjustments.Domain.Models;
using Pms.Adjustments.Domain.Services;
using Pms.Adjustments.ServiceLayer.EfCore.Billing_Records;
using Pms.Adjustments.ServiceLayer.Files;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pms.AdjustmentModule.FrontEnd.Models
{
    public class BillingRecords
    {
        private BillingRecordManager Manager;
        private BillingRecordProvider Provider;


        public BillingRecords(BillingRecordManager manageBilling, BillingRecordProvider provideBilling)
        {
            Manager = manageBilling;
            Provider = provideBilling;
        }


        public IEnumerable<BillingRecord> Get() =>
            Provider.GetBillingRecords();

        public IEnumerable<BillingRecord> GetByPayrollCode(string payrollCode) =>
            Provider.GetBillingRecordsByPayrollCode(payrollCode);


        public void SaveRecord(BillingRecord record) =>
            Manager.Save(record);

        public IEnumerable<BillingRecord> Import(string filePath) =>
            new BillingRecordImporter().Import(filePath);
    }
}
