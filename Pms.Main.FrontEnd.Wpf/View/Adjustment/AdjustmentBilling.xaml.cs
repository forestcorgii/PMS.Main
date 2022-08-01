using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Pms.Main.FrontEnd.Wpf
{
    /// <summary>
    /// Interaction logic for AdjustmentBilling.xaml
    /// </summary>
    public partial class AdjustmentBilling : Page
    {
        //private BillingGenerationService GenerationService;
        //private BillingService BillingService;
        //private BillingImportService ImportService;

        private CollectionViewSource AdjustmentBillingViewSource;
        private CollectionViewSource AdjustmentNameViewSource;
        public AdjustmentBilling()
        {
            InitializeComponent();

            //AdjustmentBillingViewSource = (CollectionViewSource)FindResource(nameof(AdjustmentBillingViewSource));
            //AdjustmentNameViewSource = (CollectionViewSource)FindResource(nameof(AdjustmentNameViewSource));

            //GenerationService = new();
            //BillingService = new();
            //ImportService = new();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //if (Shared.DefaultPayRegister is not null)
            //    AdjustmentBillingViewSource.Source = BillingService.CollectBillingsByPayRegister(Shared.DefaultPayRegister, null);

            //AdjustmentNameViewSource.Source = BillingService.CollectAdjustmentNames();
        }

        private void btnImport_Click(object sender, RoutedEventArgs e)
        {
            //ImportService.ImportBillings();
        }


        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            //WriteService.SaveBillings();
        }

        private void btnGenerateBilling_Click(object sender, RoutedEventArgs e)
        {
            //if (Shared.DefaultCutoff is not null)
            //    GenerationService.GenerateBillings(Shared.DefaultCutoff.PayrollDate);
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            //GenerationService.ExportBillings(CbAdjustmentName.Text, Shared.DefaultPayRegister);
        }

        private void CbAdjustmentName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (Shared.DefaultPayRegister is not null && e.AddedItems is not null)
            //    AdjustmentBillingViewSource.Source =
            //        BillingService.CollectBillingsByPayRegister(Shared.DefaultPayRegister, e.AddedItems[0].ToString());
        }
    }
}
