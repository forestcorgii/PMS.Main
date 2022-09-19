using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Pms.AdjustmentModule.FrontEnd
{
    /// <summary>
    /// Interaction logic for AdjustmentRecord.xaml
    /// </summary>
    public partial class AdjustmentRecord : Page
    {
        private CollectionViewSource RecordViewSource;

        //private RecordService RecordService;
        //private RecordImportService ImportService;

        public AdjustmentRecord()
        {
            InitializeComponent();
            RecordViewSource = (CollectionViewSource)FindResource(nameof(RecordViewSource));

            //RecordService = new();
            //ImportService = new();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //if (Shared.DefaultPayRegister is not null)
            //    RecordViewSource.Source = RecordService.CollectRecordsByPayRegister(Shared.DefaultPayRegister.PayrollCode);
        }

        private void btnImport_Click(object sender, RoutedEventArgs e)
        {

            //ImportService.ImportRecords();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {

            //RecordService.SaveRecords();
        }

        private void btnAddRecord_Click(object sender, RoutedEventArgs e)
        {
            //AddAdjustmentRecord rec = new();
            //rec.ShowDialog();
        }
    }
}
