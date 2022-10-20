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

namespace Pms.MasterlistModule.FrontEnd.Views.Widgets
{
    /// <summary>
    /// Interaction logic for SelectDateWidget.xaml
    /// </summary>
    public partial class SelectDateWidget : Window
    {
        public DateTime SelectedDate { get; set; }


        public SelectDateWidget()
        {
            SelectedDate = DateTime.Now.AddDays(-15);
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
