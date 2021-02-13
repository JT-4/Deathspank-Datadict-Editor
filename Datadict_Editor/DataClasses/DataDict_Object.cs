using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Datadict_Editor
{
    /// <summary>
    /// This class is meant to represent the data structure of any given object found in a Deathspank
    /// Datadict file. The ObjectID value has been added for simple tracking purposes and to keep the order of the orginal 
    /// file structure (which objects came 1st 2nd 3rd etc.)
    /// </summary>
    public class DataDict_Object
    {
        public DataDict_Object()
        {
            //instantiate the DataDictObjects observable collection so objects can be added
            // using the .add() method
            DataDictObjects = new ObservableCollection<DataDict_Attribute>();
        }
        //An observable collection containing all of the attributes for the Datadict object
        public ObservableCollection<DataDict_Attribute> DataDictObjects { get ; set; }
        
        // The number of attributes that the Datadict file object contains
        public int NumOfAttributes { get; set; }

        //An ID given to the object starting at 0 and incrimenting for each addition object
        public int ObjectID { get; set; }

        //A string for UI binding in the object selector ListBox
        public string Display {
            get
            {
                return $"Object {ObjectID} (Number of Attributes: {NumOfAttributes})";
            } 
        }
        

    }

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
        public byte[] Value {
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
            } 
        }
      
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


    }
}
