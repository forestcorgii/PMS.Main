using Pms.Timesheets.Domain.SupportTypes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Pms.Main.FrontEnd.Wpf.Converters
{
    public class CutoffToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is Cutoff)
            {
                Cutoff cutoff = (Cutoff)value;
                if (cutoff is not null)
                    return cutoff.CutoffId;
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is string)
                return new Cutoff((string)value);
            return null;
        }
    }
}
