using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace EncodingNormalizerVsx.View
{
    public class ConvertDashPercent : IValueConverter
    {
        public double Width { set; get; }
        public double StrokeThickness { set; get; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return new DoubleCollection();
            }
            double percent = 0;
            if (value is double)
            {
                percent = (double)value;
            }
            else
            {
                string str = value as string;
                if (string.IsNullOrEmpty(str))
                {
                    return new DoubleCollection();
                }
                percent = double.Parse(str);
            }
            System.Windows.Media.DoubleCollection strokeDashArray = new DoubleCollection();

            //n = (Width - StrokeThickness )*Pi *p
            double n = (Width - StrokeThickness) / StrokeThickness * Math.PI * percent;
            strokeDashArray.Add(n);
            strokeDashArray.Add(double.MaxValue);
            return strokeDashArray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
