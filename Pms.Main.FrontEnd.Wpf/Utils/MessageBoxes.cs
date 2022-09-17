using System;
using System.Windows;

namespace Pms.Main.FrontEnd.Wpf
{
    public class MessageBoxes
    {
        public static void ShowError(string message, string caption) =>
            MessageBox.Show(message,
                caption,
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );
    }
}