using Pms.Adjustments.Domain;
using Pms.Main.FrontEnd.Wpf.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Main.FrontEnd.Wpf.Stores
{
    public class BillingStore : IStore
    {
        private string _cutoffId;
        private BillingModel _model;

        public Lazy<Task> _initializeLazy { get; set; }

        private IEnumerable<Billing> _billings { get; set; }
        public IEnumerable<Billing> Billings { get; set; }

        public Action? Reloaded { get; set; }

        public BillingStore(BillingModel model)
        {
            _model = model;
            _initializeLazy = new Lazy<Task>(Initialize);

            Billings = new List<Billing>();
            _billings = new List<Billing>();

            _cutoffId = string.Empty;
        }

        public async Task Load()
        {
            try
            {
                await _initializeLazy.Value;
            }
            catch (Exception ex)
            {
                _initializeLazy = new Lazy<Task>(Initialize);
            }
        }

        public async Task Reload()
        {
            _initializeLazy = new Lazy<Task>(Initialize);
            await Load();
        }


        private async Task Initialize()
        {
            IEnumerable<Billing> billings = new List<Billing>();
            await Task.Run(() =>
            {
                billings = _model.GetBillings(_cutoffId.Substring(0,4));
            });

            _billings = billings;
            Billings = billings;
            Reloaded?.Invoke();
        }


        public async void SetCutoffId(string cutoffId)
        {
            _cutoffId = cutoffId;
            await Reload();
        }

        public void SetPayrollCode(string payrollCode)
        {
            Billings = _billings.Where(ts => ts.PayrollCode == payrollCode);
            Reloaded?.Invoke();
        }
        
    }
}
