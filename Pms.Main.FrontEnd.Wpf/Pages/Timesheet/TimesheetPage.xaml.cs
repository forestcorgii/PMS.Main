using System;
using System.Collections.Generic; 
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using Microsoft.Extensions.Configuration;

namespace Pms.Main.FrontEnd.Wpf
{

    public partial class TimesheetPage
    {
        private readonly CollectionViewSource TimesheetViewSource;

        //private readonly EmployeeService EmployeeService;
        // private readonly TimesheetService TimesheetService;
        //private readonly TimesheetOutputGenerationService DBFGeneratorService;

       
        public TimesheetPage(IConfigurationRoot? conf)
        {
            InitializeComponent();

            TimesheetViewSource = (CollectionViewSource)FindResource(nameof(TimesheetViewSource));

            // TimesheetService = new TimesheetService();
            //EmployeeService = new EmployeeService(conf);
            //DBFGeneratorService = new TimesheetOutputGenerationService();
        }

        private void GenerateDBFPage_Loaded(object sender, RoutedEventArgs e)
        {
            //if (Shared.DefaultPayRegister is not null)
            //{
            //    PayrollCode = Shared.DefaultPayRegister.PayrollCode;
            //    PayRegisterId = Shared.DefaultPayRegister.PayRegisterId;
            //    PayrollDate = Shared.DefaultPayRegister.PayrollDate;

            //    IEnumerable<Timesheet> timesheets = DBFGeneratorService.GetTimesheetByPayRegisterId(PayRegisterId);
            //    TimesheetViewSource.Source = timesheets;

            //    LbTotalEE.Text = $"Total:   {timesheets.Count()}";
            //    LbUnconfirmedEE.Text = $"Unconfirmed:   {timesheets.Count(ts => !ts.IsConfirmed)}";
            //    LbUnconfirmedEEWithAttendance.Text = $"Unconfirmed With Attendance:   {timesheets.Count(ts => !ts.IsConfirmed && ts.TotalHours > 0)}";
            //}
        }

        private void btnExportDBF_Click(object sender, RoutedEventArgs e)
        {
            //if (Shared.DefaultPayRegister is not null)
            //{
            //    string dbfPath = $@"{AppDomain.CurrentDomain.BaseDirectory}/DBF/{PayrollDate:yyyyMMdd}";
            //    Directory.CreateDirectory(dbfPath);

            //    IEnumerable<string> bankCategories = EmployeeService.GetEmployeeBankCategory(PayrollCode);
            //    foreach (string bankCategory in bankCategories)
            //        DBFGeneratorService.SavePayrollTimeToDBF(PayrollDate, PayrollCode, PayRegisterId, bankCategory, dbfPath);
            //}
        }

        private void btnExportEFile_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CbBankCategory_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            //if (Shared.DefaultPayRegister is not null && e.AddedItems is not null)
            //    TimesheetViewSource.Source = DBFGeneratorService.FilterExportableTimesheets(Shared.DefaultPayRegister.PayRegisterId,
            //        ((System.Windows.Controls.ComboBoxItem)e.AddedItems[0]).Content.ToString());
        }
    }
}