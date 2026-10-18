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
            //switch to a string for the known value
            switch (output)
            {
                case "AB-DF-3E-E3": return "Cooldown Timer";
                case "02-C6-27-00": return "Version (1-3)";
                case "99-2E-83-95": return "Items Per Stack";
                case "D4-55-79-DF": return "Heal Amount";
                case "72-8C-3C-B1": return "Ability Level";
                case "14-AC-83-51": return "Description";
                case "23-F7-A3-8A": return "Displayed Description";
                case "64-BB-70-74": return "Melee / Ranged";
                case "3C-39-02-FF": return "Image FileID";
                case "88-0A-51-3A": return "Elemental Type";
                case "A3-35-1E-F0": return "AoE distance";
                case "E0-19-1A-A9": return "Effect Length Time";
                case "C0-8F-38-EA": return "Damage Amount (xlvlx100)";
                case "F7-E3-D3-0C": return "Damage Amount (xlvlx100)";
                default: return output;
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
