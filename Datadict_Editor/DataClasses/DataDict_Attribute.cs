using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datadict_Editor
{
    /// <summary>
    /// This class is meant to describe the data structure of a Datadict object attribute including it's 
    /// description, type, offset location and value. Notify property changed is implimented to track any changes made 
    /// to an attribute value. On application closing, a check is made to see if there are any outstanding changes that 
    /// have not been saved.
    /// </summary>
    public class DataDict_Attribute : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        //set NumChanges to -1 to track the number of changes to the attribute value
        //setting the value to -1 initially allows the value to be 0 after the initial set
        public DataDict_Attribute()
        {
            this.NumChanges = -1;
        }

        private float mfvalue;

        //private backing to Value
        private byte[] mvalue;

        //the attribute Description
        public byte[] Description { get; set; }

        //The offset (in the final datablock) to the attribute value
        public int Offset { get; set; }

        //The type of attribute. This can represent a 4,8, or 16 byte field or a
        //variable length byte array that represents a string value
        public byte Type { get; set; }

        //A tracker for the number of changes made to the attribute value
        public int NumChanges { get; set; }

        //The value of the attribute. Impliments OnPropertyChanged.
        //incriments NumChanges on each change event
        public byte[] Value
        {
            get
            {
                return mvalue;
            }
            set
            {
                if (value != mvalue)
                {
                    mvalue = value;
                    NumChanges++;
                    OnPropertyChanged("Value");
                }
                if (Type == 9)
                {
                    fValue = FloatFromArray(value);
                }
            }
        }

        public float fValue
        {
            get
            {
                return mfvalue;
            }
            set
            {
                if (value != mfvalue)
                {
                    mfvalue = value;
                    OnPropertyChanged("fValue");
                }
            }
        }

        public bool DisplayAsHex { get; set; }

        /// <summary>
        /// Raises the Property Changed event for the Member that was changed
        /// </summary>
        /// <param  name="propertyName" > Represents the name of the member who was changed</param>
        private void OnPropertyChanged(string propertyName)
        {
            var propertyChanged = PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        private float FloatFromArray(byte[] value)
        {
            return BitConverter.ToSingle(value, 0);
        }
        

    }
}
