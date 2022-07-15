
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Collections.Generic;
using System.ComponentModel;
using Pms.Main.FrontEnd.Wpf.Controller;
using Payroll.Employees.ServiceLayer.HRMS.Adapter;
using Payroll.Employees.Domain;

namespace Pms.Main.FrontEnd.Wpf
{
    /// <summary>
    /// Interaction logic for Employee.xaml
    /// </summary>
    public partial class EmployeeUi : Page
    {
        private CollectionViewSource employeeViewSource;
        private HRMSAdapter Adapter;
        private EmployeeController Controller;

        private FilterBinding FilterBinding;

        public EmployeeUi()
        {
            InitializeComponent();
            employeeViewSource = (CollectionViewSource)FindResource(nameof(employeeViewSource));
            Adapter = HRMSAdapterFactory.CreateAdapter(Shared.Configuration);

            //FilterBinding = (FilterBinding)FindResource(nameof(FilterBinding));
            //FilterBinding.PropertyChanged += FilterBinding_PropertyChanged;
        }

        private void Employee_Loaded(object sender, RoutedEventArgs e)
        {
            employeeViewSource.Source = new ObservableCollection<Employee>(Controller.LoadEmployees().ToList());
        }

        private void FilterBinding_PropertyChanged(object sender, PropertyChangedEventArgs e) =>
            employeeViewSource.Source = new ObservableCollection<Employee>(Controller.LoadEmployees().ToList());

        // employeeViewSource.Source = Controller.(FilterBinding.Filter, Shared.DefaultPayRegister.PayrollCode);


        private void btnSave_Click(object sender, RoutedEventArgs e)
        {

            // Controller.SaveChanges();
        }

        private async void btnSync_Click(object sender, RoutedEventArgs e)
        {
            IEnumerable<Employee> employees;
            if (cbSyncType.SelectedIndex == 0)
                employees = lstEmployees.Items.OfType<Employee>().ToList();
            else if (cbSyncType.SelectedIndex == 1)
                employees = Controller.LoadEmployees().ToList();
            else
                return;

            pb.Maximum = employees.Count();
            pb.Value = 0;

            foreach (Employee employee in employees)
            {
                //await Controller.SyncEmployeeAsync(employee, employee.Site);
                lbStatus.Text = $"Syncing {employee.EEId}";
                pb.Value += 1;
            }

            //Controller.SaveChanges();
            lbStatus.Text = "DONE!!";
        }

        private void lstEmployees_SelectionChanged(object sender, SelectionChangedEventArgs e) =>
            grbEmployeeDetail.DataContext = (Employee)e.AddedItems[0];
    }
}
