using System;
using System.Globalization;
using System.Windows.Data;

namespace Datadict_Editor
{
    public class DecimalBytetoHex_Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //cast the passed in value to a byte array
            byte mbytes = (byte)value;
            string hexbytes = string.Format("{0:X2}", mbytes);
            return hexbytes;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!string.IsNullOrWhiteSpace((string)value) && value.ToString().Length == 2) 
            {
                string hexstring = value.ToString();

                int mdecimal = System.Convert.ToInt32(hexstring, 16);

                return mdecimal;
            }
            
            return null;
        }
    }
}
