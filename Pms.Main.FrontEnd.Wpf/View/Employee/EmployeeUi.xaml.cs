
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Collections.Generic;
using System.ComponentModel;
using Pms.Main.FrontEnd.Wpf.ViewModel;
using Pms.Employees.ServiceLayer.HRMS.Adapter;
using Pms.Employees.Domain;

namespace Pms.Main.FrontEnd.Wpf.Views
{
    /// <summary>
    /// Interaction logic for Employee.xaml
    /// </summary>
    public partial class EmployeeUi : UserControl
    {
        //private CollectionViewSource employeeViewSource;
        //private FilterEmployeesViewModel FilterEmployeesVM;
        //private DownloadEmployeesViewModel DownloadEmployeesVM;

        //private FilterBinding FilterBinding;

        public EmployeeUi()
        {
            InitializeComponent();
            //employeeViewSource = (CollectionViewSource)FindResource(nameof(employeeViewSource));

        }

        private void Employee_Loaded(object sender, RoutedEventArgs e)
        {
            //if (Shared.DefaultPayrollCode is not null)
            //{
                //FilterEmployeesVM = new(Shared.DefaultPayrollCode);
                //DownloadEmployeesVM = new(Shared.DefaultPayrollCode);
                //employeeViewSource.Source = new ObservableCollection<Employee>(FilterEmployeesVM.ListEmployeesByPayrollCode().ToList());

                //FilterBinding = (FilterBinding)FindResource(nameof(FilterBinding));
                //FilterBinding.PropertyChanged += FilterBinding_PropertyChanged;

                //InitializeEvents();
            //}
        }

        //private void InitializeEvents()
        //{

            //DownloadEmployeesVM.EmployeeDownloadStarted += EEDownloadController_EmployeeDownloadStarted;
            //DownloadEmployeesVM.EmployeeDownloadSucceed += EEDownloadController_EmployeeDownloadSucceed;
            //DownloadEmployeesVM.EmployeeDownloadError += EEDownloadController_EmployeeDownloadError;

        //}
















        #region EMPLOYEE DOWNLOAD
        private void EEDownloadController_EmployeeDownloadStarted(object sender, int totalEmployees)
        {
            //LbStatusMessage.Text = "Downloading Employees";
            //PbDownloadProgress.Maximum = totalEmployees + 1;
            //PbDownloadProgress.Value = 0;

            //LbPbMaximum.Text = PbDownloadProgress.Maximum.ToString();
            //LbPbValue.Text = "0";
        }
        private void EEDownloadController_EmployeeDownloadError(object sender, string eeId, string errorMessage)
        {
            //MessageBox.Show(errorMessage, "EEDownloadController_EmployeeDownloadError", MessageBoxButton.OK, MessageBoxImage.Error);
            //PbDownloadProgress.Value = 0;
        }
        private void EEDownloadController_EmployeeDownloadSucceed(object sender, string eeId)
        {
            //PbDownloadProgress.Value++;
            //LbStatusMessage.Text = $"Downloading Employee: {eeId}";
            //LbPbValue.Text = PbDownloadProgress.Value.ToString();
        }
        #endregion















        //private void FilterBinding_PropertyChanged(object sender, PropertyChangedEventArgs e) =>
        //    employeeViewSource.Source = new ObservableCollection<Employee>(FilterEmployeesVM.ListEmployeesByPayrollCode(FilterBinding.Filter).ToList());



        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            // Controller.SaveChanges();
        }

        private async void btnSync_Click(object sender, RoutedEventArgs e)
        {
            //string[] employeeIds;
            //if (cbSyncType.SelectedIndex == 0)
            //    employeeIds = FilterEmployeesVM.ListEmployeesByPayrollCode().Select(ee => ee.EEId).ToArray();
            //else if (cbSyncType.SelectedIndex == 1)
            //    employeeIds = FilterEmployeesVM.ListAllEmployees().Select(ee => ee.EEId).ToArray();
            //else
            //    return;

            //PbDownloadProgress.Maximum = employeeIds.Count();
            //PbDownloadProgress.Value = 0;


            //await DownloadEmployeesVM.FindEmployeeAsync(employeeIds);

            //foreach (string eeId in employeeIds)
            //{
            //    //await Controller.SyncEmployeeAsync(employee, employee.Site);
            //    LbStatusMessage.Text = $"Syncing {eeId}";
            //    PbDownloadProgress.Value += 1;
            //}

            ////Controller.SaveChanges();
            //LbStatusMessage.Text = "DONE!!";
        }

        //private void lstEmployees_SelectionChanged(object sender, SelectionChangedEventArgs e) =>
        //    grbEmployeeDetail.DataContext = (Employee)e.AddedItems[0];
    }
}
