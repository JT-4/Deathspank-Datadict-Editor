
using System;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace Datadict_Editor
{
    public class ByteArraytoASCII_Converter : IValueConverter
    {
        /// <summary>
        /// Converts an array of bytes to an ascii string seperated by hyphens
        /// </summary>
        /// <param name="value"> the value to be converted</param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //cast the passed in value to a byte array
            byte[] bytes = (byte[])value;
            //convert the byte array to a string
            string asciistring = Encoding.ASCII.GetString(bytes);
            //return the string
            return asciistring;
                  
        }

        /// <summary>
        /// Converts the ascii string representation of a byte array back into a byte array
        /// </summary>
        /// <param name="value"> the ascii string to be converted back into a byte array</param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //cast the passed in value to a string
            string convertstring = (string)value;
            //convert the string to an array of bytes
            byte[] bytes = Encoding.ASCII.GetBytes(convertstring);
            //return the byte array
            return bytes;
        }
    }
}
