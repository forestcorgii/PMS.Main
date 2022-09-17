using System;
using System.Windows;

namespace Pms.Main.FrontEnd.Common
{
    public class MessageBoxes
    {
        public static void ShowError(string message, string caption) =>
            MessageBox.Show(message,
                caption,
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );

        public static void ShowMessage(string message, string caption) =>
            MessageBox.Show(message,
                caption,
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
    }
}