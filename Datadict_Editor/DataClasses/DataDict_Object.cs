using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace Datadict_Editor
{
    /// <summary>
    /// This class is meant to represent the data structure of any given object found in a Deathspank
    /// Datadict file. The ObjectID value has been added for simple tracking purposes and to keep the order of the orginal 
    /// file structure (which objects came 1st 2nd 3rd etc.)
    /// </summary>
    public class DataDict_Object : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private bool displayashex { get; set; }

        public DataDict_Object()
        {
            //instantiate the DataDictObjects observable collection so objects can be added
            // using the .add() method
            DataDictObjects = new BindingList<DataDict_Attribute>();
        }
        //An observable collection containing all of the attributes for the Datadict object
        public BindingList<DataDict_Attribute> DataDictObjects { get;set;}

        // The number of attributes that the Datadict file object contains
        public int NumOfAttributes { get; set; }

        //An ID given to the object starting at 0 and incrimenting for each addition object
        public int ObjectID { get; set; }

        public string DisplayDesc {get;set; }

        //A string for UI binding in the object selector ListBox
        public string Display {
            get
            {
                return $"Object {ObjectID} | {DisplayDesc} | Attributes: {NumOfAttributes}";
            } 
        }

        public bool DisplayAsHex {
            get 
            {
                return displayashex;
            }
            set 
            {
                if (value != displayashex)
                {
                    displayashex = value;
                    UpdateAttributeDisplayAsHex();
                    OnPropertyChanged("DisplayAsHex");
                }
            }
        }

        private void UpdateAttributeDisplayAsHex()
        {
            foreach (DataDict_Attribute atr in DataDictObjects)
            {

                atr.DisplayAsHex ^= true;
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
