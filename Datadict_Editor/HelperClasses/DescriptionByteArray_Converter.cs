using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace Datadict_Editor
{
    class DescriptionByteArray_Converter : IValueConverter
    {

        /// <summary>
        /// Converts a byte array to a string
        /// Converts knows description bytes into a descriptive string
        /// otherwise it returns 4 hex bytes with hyphens in between
        /// </summary>
        /// <param name="value">The passed in value to be converted</param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            byte[] bytes = (byte[])value;
            string output = BitConverter.ToString(bytes);

            UInt32 intValue = BitConverter.ToUInt32(bytes, 0);
            //check the hash names dictionary for a match
            if (DeathSpankDatadictHashNames.Names.ContainsKey(intValue))
            {
                return DeathSpankDatadictHashNames.Names[intValue];
            }
            else
            {
                //if no match, return the hex string
                return output;
            }
          
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
