using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.VisualBasic.CompilerServices;

namespace Pms.Main.FrontEnd.Wpf
{

    public partial class ProcessPayreg
    {
        //private readonly PayrollImportService ImportService;
        //private readonly EmployeeService EmployeeService;
        public ProcessPayreg()
        {
            //bgProcess = new BackgroundWorker();

            //// This call is required by the designer.
            InitializeComponent();

            //// Add any initialization after the InitializeComponent() call.
            //ImportService = new(Shared.Configuration);
            //EmployeeService = new();
        }

        private void ProcessPayreg_Loaded(object sender, RoutedEventArgs e) { }

        private void btnStartProcess_Click(object sender, RoutedEventArgs e) { }

        private BackgroundWorker BgProcess;
        private void bgProcess_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                //string[] payRegisters = (string[])e.Argument;
                //Dispatcher.Invoke(() =>
                //    {
                //        pb.Maximum = payRegisters.Count();
                //        pb.Value = 0d;
                //    });
                //for (int i = 0, loopTo = payRegisters.Count() - 1; i <= loopTo; i++)
                //{
                //    ImportService.ImportPayRegister(payRegisters[i], Shared.DefaultPayRegister.PayRegisterId);
                //    Dispatcher.Invoke(() => { pb.Value += 1d; });
                //}
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void btnUploadPayreg_Click(object sender, RoutedEventArgs e)
        {
            //using (var openFile = new OpenFileDialog())
            //{
            //    openFile.Filter = "Pay Register Files(*.xls)|*.xls";
            //    openFile.Multiselect = true;
            //    if (openFile.ShowDialog() == DialogResult.OK)
            //    {
            //        BgProcess.RunWorkerAsync(openFile.FileNames);
            //    }
            //}
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void BntSupply_Click(object sender, RoutedEventArgs e)
        {
            //using (var openFile = new OpenFileDialog())
            //{
            //    openFile.Filter = "Pay Register Files(*.xls)|*.xls";
            //    openFile.Multiselect = false;
            //    if (openFile.ShowDialog() == DialogResult.OK)
            //    {
            //        //await EmployeeService.SupplyGovernmentDetailsAsync(openFile.FileName);
            //    }
            //}
        }
    }
}