using Pms.Main.FrontEnd.Wpf.Models;
using Pms.Payrolls.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Pms.Payrolls.Domain.Enums;

namespace Pms.Main.FrontEnd.Wpf.Stores
{
    public class PayrollStore : IStore
    {
        private string _cutoffId { get; set; } = string.Empty;
        private BankType _bankType { get; set; } = BankType.LBP;

        private readonly PayrollModel _model;

        public Lazy<Task> _initializeLazy { get; set; }
        public IEnumerable<Payroll> _payrolls { get; set; }
        public IEnumerable<Payroll> Payrolls { get; set; }
        public Action? Reloaded { get; set; }

        public PayrollStore(PayrollModel model)
        {
            _initializeLazy = new Lazy<Task>(Initialize);

            _payrolls = new List<Payroll>();
            Payrolls = new List<Payroll>();
            _model = model;
        }


        public async Task Load()
        {
            try
            {
                await _initializeLazy.Value;
            }
            catch (Exception)
            {
                _initializeLazy = new Lazy<Task>(Initialize);
            }
        }

        public async Task Reload()
        {
            _initializeLazy = new Lazy<Task>(Initialize);
            await _initializeLazy.Value;
        }


        private async Task Initialize()
        {
            IEnumerable<Payroll> payrolls = new List<Payroll>();
            await Task.Run(() =>
            {
                payrolls = _model.Get(_cutoffId, _bankType);
            });

            _payrolls = payrolls;
            Payrolls = payrolls;

            Reloaded?.Invoke();
        }

        public async void SetCutoffId(string cutoffId)
        {
            _cutoffId = cutoffId;
            await Reload();
        }

        public void SetPayrollCode(string payrollCode)
        {
            Payrolls = _payrolls.Where(p => p.PayrollCode == payrollCode);
            Reloaded?.Invoke();
        }
    }
}
