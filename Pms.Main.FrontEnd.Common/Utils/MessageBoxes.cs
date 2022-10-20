using System;
using System.Windows;

namespace Pms.Main.FrontEnd.Common.Utils
{
    public class MessageBoxes
    {
        public static void Error(string message, string caption = "") =>
            MessageBox.Show(message,
                caption,
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );

        public static void Prompt(string message, string caption = "") =>
            MessageBox.Show(message,
                caption,
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );

        public static bool Inquire(string message, string caption = "") =>
            MessageBox.Show(message,
                caption,
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            ) == MessageBoxResult.Yes;
    }
}