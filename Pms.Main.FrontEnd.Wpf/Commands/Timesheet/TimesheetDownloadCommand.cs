﻿using Microsoft.Toolkit.Mvvm.Input;
using Pms.Main.FrontEnd.Wpf.Models;
using Pms.Main.FrontEnd.Wpf.Stores;
using Pms.Main.FrontEnd.Wpf.ViewModels;
using Pms.Timesheets.Domain;
using Pms.Timesheets.Domain.SupportTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Pms.Main.FrontEnd.Wpf.Commands
{
    public class TimesheetDownloadCommand : IRelayCommand
    {
        private TimesheetViewModel _viewModel;
        private MainStore _cutoffStore;
        private TimesheetModel _cutoffTimesheet;

        public TimesheetDownloadCommand(TimesheetViewModel viewModel, MainStore cutoffStore, TimesheetModel cutoffTimesheet)
        {
            _viewModel = viewModel;
            _cutoffStore = cutoffStore;
            _cutoffTimesheet = cutoffTimesheet;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public async void Execute(object? parameter)
        {
            Cutoff cutoff = _cutoffStore.Cutoff;
            string payrollCode = _cutoffStore.PayrollCode.Name;
            string site = _cutoffStore.Site;

            if (parameter is null)
            {
                _viewModel.SetProgress("Retrieving Download content summary", 1);
                int[] pages; 
                pages = await _cutoffTimesheet.DownloadContentSummary(cutoff, payrollCode, site);

                await StartDownload(pages, cutoff, payrollCode, site);
            }
            else
                await StartDownload((int[])parameter, cutoff, payrollCode, site);
            _viewModel.EvaluateCommand.Execute(null);
        }

        public async Task StartDownload(int[] pages, Cutoff cutoff, string payrollCode, string site)
        {
            _viewModel.SetProgress("Downloading Timesheets", pages.Length);

            foreach (int page in pages)
            {
                while (true)
                {
                    try
                    {
                        IEnumerable<Timesheet> timesheets = await _cutoffTimesheet.DownloadContent(cutoff, payrollCode, site, page);
                        foreach (Timesheet timesheet in timesheets)
                            _viewModel.Timesheets.Add(timesheet);
                        break;
                    }
                    catch (Exception ex)
                    {
                        if (MessageBox.Show($"{ex.Message}... Do You want to Retry?", "Timesheet Download Error...", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                            break;
                    }
                }
                _viewModel.ProgressValue++;
            }
            _viewModel.SetAsFinishProgress();
        }

        public void NotifyCanExecuteChanged() { }
    }
}
