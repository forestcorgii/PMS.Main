using Microsoft.Extensions.Configuration;
using Pms.Timesheets.Domain.SupportTypes;
using System;
using System.Windows;

namespace Pms.Main.FrontEnd.Wpf.Utils
{
    internal static class Shared
    {
        public static IConfigurationRoot? Configuration { get; set; }

        public static Cutoff? DefaultCutoff { get; set; }
        public static string? DefaultPayrollCode { get; set; }



    }
    internal class MessageBoxes
    {
        public static void ShowError(string message, string caption) =>
            MessageBox.Show(message,
                caption,
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );
    }
}