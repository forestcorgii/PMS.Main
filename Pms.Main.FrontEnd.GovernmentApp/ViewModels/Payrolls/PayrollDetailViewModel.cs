using Pms.Main.FrontEnd.Common;
using Pms.Payrolls.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Main.FrontEnd.Government.ViewModels
{
    public class PayrollDetailViewModel : ViewModelBase
    {
        private Payroll[] MonthlyPayroll { get; set; }
        public string PayrollId => MonthlyPayroll[0].PayrollId;

        public string EEId => MonthlyPayroll[0].EEId;
        public EmployeeView EE => MonthlyPayroll[0].EE;

        public string CutoffId => MonthlyPayroll[0].CutoffId;
        public Cutoff Cutoff { get => new Cutoff(CutoffId); }

        public string CompanyId => MonthlyPayroll[0].CompanyId;

        public string PayrollCode => MonthlyPayroll[0].PayrollCode;


        public double MonthlyGrossPay => MonthlyPayroll.Sum(p => p.GrossPay);
        public double MonthlyRegularPay => MonthlyPayroll.Sum(p => p.RegularPay);
        public double MonthlyNetPay => MonthlyPayroll.Sum(p => p.NetPay);

        public double EmployeeSSS => MonthlyPayroll.Sum(p => p.EmployeeSSS);
        public double EmployeePhilHealth => MonthlyPayroll.Sum(p => p.EmployeePhilHealth);
        public double EmployeePagibig => MonthlyPayroll.Sum(p => p.EmployeePagibig);
        public double WithholdingTax => MonthlyPayroll.Sum(p => p.WithholdingTax);

        public double GeneratedEmployeePhilHealth { get; set; }

        public double GeneratedEmployeeSSS { get; set; }
        public double GeneratedEmployerSSS { get; set; }

        public double GeneratedEmployeePagibig { get; set; }
        public double GeneratedEmployerPagibig { get; set; }



        public PayrollDetailViewModel(Payroll[] monthlyPayroll)
        {
            MonthlyPayroll = monthlyPayroll;

            ComputePagibig();
            ComputePhilHealth();
            ComputeSSS();
            ComputeWTAX();
        }


        public void ComputePagibig()
        {
            GeneratedEmployeePagibig = MonthlyGrossPay * 0.02d;
            if (GeneratedEmployeePagibig >= 21d & GeneratedEmployeePagibig < 100d)
                GeneratedEmployerPagibig = GeneratedEmployeePagibig;
            else if (GeneratedEmployeePagibig < 21d)
                GeneratedEmployerPagibig = GeneratedEmployeePagibig * 2d;
            else
                GeneratedEmployerPagibig = 100;
        }

        public void ComputeSSS()
        {
            int multiplier = (int)((long)Math.Round(MonthlyGrossPay * 2d - 2750d) / 500L);

            double ER_rsc = Math.Min(255d + 42.5d * multiplier, 1700d);
            double EE_rsc = Math.Min(135d + 22.5d * multiplier, 900d);

            double ER_ec = multiplier <= 23 ? 10 : 30;
            double EE_ec = 0d;

            int multiplier_mpc = Math.Max(0, multiplier - 34);
            double ER_mpf = Math.Min(42.5d * multiplier_mpc, 425d);
            double EE_mpf = Math.Min(22.5d * multiplier_mpc, 225d);

            GeneratedEmployeeSSS = EE_rsc + EE_ec + EE_mpf;
            GeneratedEmployerSSS = ER_rsc + ER_ec + ER_mpf;
        }

        public void ComputePhilHealth()
        {
            switch (MonthlyGrossPay)
            {
                case var @case when @case >= 70000d:
                    GeneratedEmployeePhilHealth = 1800;
                    break;
                case var case1 when case1 >= 10000.01d:
                    GeneratedEmployeePhilHealth = MonthlyGrossPay * 0.03d;
                    break;
                case var case2 when case2 <= 10000d:
                    GeneratedEmployeePhilHealth = 300;
                    break;
            }
        }

        public double ComputeWTAX()
        {
            double wtax = 0d;
            switch (MonthlyRegularPay)
            {
                case var case3 when case3 >= 666667d:
                    wtax = 200833.33d + ((MonthlyRegularPay - 666667d) * 0.35d);
                    break;
                case var case4 when case4 >= 166667d:
                    wtax = 40833.33d + ((MonthlyRegularPay - 166667d) * 0.32d);
                    break;
                case var case5 when case5 >= 66667d:
                    wtax = 10833.33d + ((MonthlyRegularPay - 66667d) * 0.3d);
                    break;
                case var case6 when case6 >= 33333d:
                    wtax = 2500d + ((MonthlyRegularPay - 33333d) * 0.25d);
                    break;
                case var case7 when case7 >= 20833.01d:
                    wtax = 0d + ((MonthlyRegularPay - 20833.01d) * 0.2d);
                    break;
                case var case8 when case8 <= 20833d:
                    wtax = 0d;
                    break;
            }
            return wtax;
        }

    }
}
