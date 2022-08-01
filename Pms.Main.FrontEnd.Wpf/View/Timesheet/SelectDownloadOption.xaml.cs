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
using System.Windows.Shapes;
using Pms.Timesheets.ServiceLayer.TimeSystem.Services;
using static Pms.Timesheets.ServiceLayer.TimeSystem.Services.Enums;

namespace Pms.Main.FrontEnd.Wpf
{
    /// <summary>
    /// Interaction logic for SelectDownloadOption.xaml
    /// </summary>
    public partial class SelectDownloadOption : Window
    {
        public DownloadOptions DownloadOptions;

        public SelectDownloadOption()
        {
            InitializeComponent();
        }

        private void BtnDownloadAll_Click(object sender, RoutedEventArgs e)
        {
            DownloadOptions = DownloadOptions.All;
            DialogResult = true;
            Close();
        }

        private void BtnDownloadUnconfirmed_Click(object sender, RoutedEventArgs e)
        {
            DownloadOptions = DownloadOptions.UnconfirmedOnly;
            DialogResult = true;
            Close();
        }
    }
}
