using Microsoft.Extensions.Configuration;
using Pms.Timesheets.Domain.SupportTypes;
using System; 

namespace Pms.Main.FrontEnd.Wpf
{
    internal static class Shared
    {
        public static IConfigurationRoot? Configuration { get; set; }

        public static Cutoff DefaultCutoff { get; set; }
        public static string DefaultPayrollCode { get; set; }
    }
}