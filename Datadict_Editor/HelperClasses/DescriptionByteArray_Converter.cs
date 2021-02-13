using System;
using System.Globalization;
using System.Windows.Data;

namespace Datadict_Editor
{
    class DescriptionByteArray_Converter : IValueConverter
    {
        /// <summary>
        /// Converts a byte array to a string
        /// </summary>
        /// <param name="value">The passed in value to be converted</param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            byte[] bytes = (byte[])value;
            return BitConverter.ToString(bytes);
        }

        /// <summary>
        /// Currently not implemented because this will only be bound in one direction and will
        /// not need to be converted back
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
