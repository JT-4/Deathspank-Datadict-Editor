using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Datadict_Editor
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        #region Constructor
        /// <summary>
        /// Link the relay commands to private Methods and instantiate all Lists 
        /// and observable collections
        /// </summary>
        public MainWindowViewModel()
        {
            this.OpenFile_Command = new RelayCommand(OpenFile_Execute);
            this.ExitApplication_Command = new RelayCommand(ExitApplication_Execute);
            this.SaveAs_Command = new RelayCommand(SaveAs_Execute);
            this.Info_Command = new RelayCommand(Info_Execute);
            this.ReadMe_Command = new RelayCommand(ReadMe_Execute);

            DDObjects = new ObservableCollection<DataDict_Object>();
            SaveAsFinalData = new List<byte>();
            SaveAsOutput = new List<byte>();
            SaveAsAttributeDescription = new List<byte>();
            
        }
        #endregion

        /// <summary>
        /// private backing objects to allow for onpropertychanged implimentation
        /// </summary>
        #region Private Objects
        //private backing for OpenFileBytes to impliment onpropertychanged
        private byte[] openFileBytes;
        //private backing for CurrentOpenFileName to impliment onpropertychanged
        private string currentOpenFileName;
        //private backing for UnsavedChanges to impliment onpropertychanged
        private int unsavedChanges;
        //private backing for SelectedObject to impliment onpropertychanged
        private DataDict_Object selectedobject;
        #endregion

        #region Public Objects
        //interface for onproperty changed
        public event PropertyChangedEventHandler PropertyChanged;



        /// <summary>
        /// Save a string representing the filename of the currently opened
        /// Datadict file. Implement notify on property changed.
        /// </summary>
        public string CurrentOpenFileName 
        { get 
            {
                return currentOpenFileName;
            } set 
            {
                if (value != currentOpenFileName)
                {
                    currentOpenFileName = value;
                    OnPropertyChanged("CurrentOpenFileName");
                }
            } 
        }
        /// <summary>
        /// Save a string representing the full filepath of the currently opened
        /// Datadict file. Implement notify on property changed.
        /// </summary>
        public string CurrentOpenFileFullName { get; set; }

        /// <summary>
        /// store the bytes of the currently open file as a byte array. This is used to populate the data
        /// into class objects as well as to build the beginning of the output file when a "save as"
        /// operation is performed. Implement notify on property changed.
        /// </summary>
        public byte[] OpenFileBytes
        {
            get
            {
                return openFileBytes;
            }
            set
            {
                if (value != openFileBytes)
                {
                    openFileBytes = value;
                    OnOpenFileBytesChanged();
                    //OnPropertyChanged("OpenFileBytes");
                }
            }
        }

        /// <summary>
        /// An observable collection that is bound to the Datadict objects listbox in the UI.
        /// This holds the data in OpenFileBytes for easier manipulation when a user changes
        /// values
        /// </summary>
        public ObservableCollection<DataDict_Object> DDObjects { get; set; }

        /// <summary>
        /// The final byte array that will be written to a new file when the save as command is run
        /// </summary>
        public List<byte> SaveAsOutput { get; set; }
        /// <summary>
        /// A list that is added to SaveAsOutput during the saveas process
        /// this contains the attribute description portion of the 
        /// output file
        /// </summary>
        public List<byte> SaveAsAttributeDescription { get; set; }
        /// <summary>
        /// A list that is added to SaveAsOutput during the saveas process
        /// This contains all of the values for each of the SaveAsAttributeDescription
        /// objects.
        /// </summary>
        public List<byte> SaveAsFinalData { get; set; }

        //The currently selected object from the object selector ListBox
        //in the UI. Impliments OnPropertyChanged
        public DataDict_Object SelectedObject 
        { 
            get 
            {
                return selectedobject;
            }
            set
            {
                if (value != selectedobject)
                {
                    selectedobject = value;
                    OnPropertyChanged("SelectedObject");
                }
            } 
        }

        /// <summary>
        /// not fully implimented at the moment. Used to track the number of changes to datadict objects
        /// so the ui can prompt the user if they want to exit the application without saving
        /// Impliments onproperty changed 
        /// </summary>
        public int UnsavedChanges 
        {
            get
            {
                return unsavedChanges;
            }
            set
            {
                if(value != unsavedChanges)
                {
                    unsavedChanges = value;
                    OnPropertyChanged("UnsavedChanges");
                }
            }
        }
        #endregion

        #region Relay Commands
        public ICommand OpenFile_Command { get; set; }

        public ICommand SaveAs_Command { get; set; }

        public ICommand ExitApplication_Command { get; set; }

        public ICommand TitleBar_Exit { get; set; }

        public ICommand Info_Command { get; set; }
        public ICommand ReadMe_Command { get; set; }

        #endregion

        #region OnChanged Methods
        /// <summary>
        /// Populate an observable collection of DataDict_Objects
        /// from the bytes in the currently opened files any time the currently
        /// opened file changes
        /// </summary>
        private void OnOpenFileBytesChanged()
        {
            //Clear all objects from the observable collection
            DDObjects.Clear();
            //Declare the number of objects
            //derived by bytes 4-8 of OpenFileBytes array
            int numobjects =  BitConverter.ToInt32(OpenFileBytes.Skip(4).Take(4).ToArray(), 0);
            //declare the datablock length from bytes 12-16
            int datablocklength = BitConverter.ToInt32(OpenFileBytes.Skip(12).Take(4).ToArray(), 0);
            //declare the attribute description block length by taking bytes 8-12 as an integer and multiplying by 8
            //because each attribute description is 8 bytes long
            int attributedescriptionblocklength = BitConverter.ToInt32(OpenFileBytes.Skip(8).Take(4).ToArray(), 0) * 8;

            //define the block of data defining the number of attributes per object
            byte[] objectattributeblock = OpenFileBytes.Skip(16).Take(numobjects * 8).ToArray();
            //define the lock of data defining the details of each object attribute
            byte[] attributedescriptionblock = OpenFileBytes.Skip(16 + objectattributeblock.Length).Take(attributedescriptionblocklength).ToArray();
            //define the block of data where all of the attribute values are held
            byte[] datablock = OpenFileBytes.Skip(OpenFileBytes.Length -datablocklength).Take(datablocklength).ToArray();

            //create a loop that repeats for every object in the file
            for (int i = 0; i < numobjects; i++)
            {

                //Get the 8 bits that define the current object and the number of attributes it has
                byte[] currentobject = objectattributeblock.Skip(i * 8).Take(8).ToArray();
                //take the last 4 bits of the current object byte array to define the number of attributes the object has
                int numattributes = BitConverter.ToInt32(currentobject.Skip(4).Take(4).ToArray(), 0);
                //define where in the attribute description block this object's attributes start
                int attrstartOFfset = (BitConverter.ToInt32(currentobject.Skip(0).Take(4).ToArray(), 0) * 8);
                
                //create a new datadict_object to represent the file object
                //populate the number of attributes and give it an object ID
                DataDict_Object ddobject = new DataDict_Object() {
                    NumOfAttributes = numattributes,
                    ObjectID = i + 1
                };


                //Loop through the attributes and populate the attributes into to DataDict_Object
                for (int a = 0; a < numattributes; a++)
                {
                    //locate and define the current attribute description object (8 bytes)
                    byte[] currentattribute = attributedescriptionblock.Skip((a * 8) + attrstartOFfset).Take(8).ToArray();

                    //create a new datadict_attribute object and populate the description, offset and type
                    DataDict_Attribute ddattribute = new DataDict_Attribute()
                    {
                        Description = currentattribute.Skip(0).Take(4).ToArray(),
                        Offset = ReturnOffset(currentattribute.Skip(4).Take(3).ToArray()),
                        Type = currentattribute[7]
                        
                    };
                    //use the get value method to return the value for this attribute based on its type and offset
                    ddattribute.Value = GetValue(ddattribute.Offset, ddattribute.Type, datablock);
                    //add the current attribute to the object's observable collection of attributes
                    ddobject.DataDictObjects.Add(ddattribute);
                }
               

                //Add the current object to the bound observablecollection of datadict objects
                DDObjects.Add(ddobject);
            }

        
    }

        /// <summary>
        /// Raises the property changed event based on the property named passed in
        /// </summary>
        /// <param name="propertyName" > A string representing the display name of the propery
        /// raising the Property changed event</param>
        public void OnPropertyChanged(string propertyName)
        {
            var propertyChanged = PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Open a Datadict file and store the bytes in the public byte array OpenFileBytes.
        /// Filter the open dialog to show only datadict files.
        /// </summary>

        private void OpenFile_Execute()
        {
            //create a new open file dialog and filter it to only show datadict files
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "DeathSpank Data Files (*.datadict)|*.datadict|DeathSpank TextDict Files(*.textdict)|*.textdict|All files (*.*)|*.*";

            
            if (openFileDialog.ShowDialog() == true) 
            {
                byte[] PreOpenFormatCheck = File.ReadAllBytes(openFileDialog.FileName);
                bool IsDeathspankDictFormat = DeathspankFormatCheck(PreOpenFormatCheck.Skip(0).Take(4).ToArray());

                
                if (IsDeathspankDictFormat == true)
                {
                    //store the file as a byte array
                    OpenFileBytes = File.ReadAllBytes(openFileDialog.FileName);
                }
                else
                {
                    WrongFileFormat();
                    return;
                }
                
                //store the full file path of the file
                CurrentOpenFileFullName = openFileDialog.FileName;
                //get and store the filename portion of the full file path
                int index = openFileDialog.FileName.LastIndexOf('\\') + 1;
                CurrentOpenFileName =  openFileDialog.FileName.Substring(index, openFileDialog.FileName.Length - index) ;
                
             }
        }

        /// <summary>
        /// show a popup widow with simple information about the app
        /// </summary>
        private void Info_Execute()
        {
            // Initializes the variables to pass to the MessageBox.Show method.
            string message = "Program: Deathspank Datadict Editor \n" +
                "Author: Overkill_Killer \n" +
                $"Version: {Assembly.GetExecutingAssembly().GetName().Version} \n" +
                "Date: 2021";

            string caption = "Program Information";
            MessageBoxButton buttons = MessageBoxButton.OK;

            // Displays the MessageBox
            MessageBoxResult result = MessageBox.Show(message, caption, buttons);
        }

        /// <summary>
        /// Open a web browser to the readme github page
        /// </summary>
        private void ReadMe_Execute()
        {
            System.Diagnostics.Process.Start("https://github.com/JT-4/Deathspank-Datadict-Editor");
        }

        /// <summary>
        /// Display a message box letting the user know 
        /// that the file was not a compatible format and
        /// cannot be opened
        /// </summary>
        private void WrongFileFormat()
        {
            // Initializes the variables to pass to the MessageBox.Show method.
            string message = "This File is not recognized as a Deathspank Datadict or textdict file";
            string caption = "Error Reading the File";
            MessageBoxButton buttons = MessageBoxButton.OK;
            
            // Displays the MessageBox
            MessageBoxResult result = MessageBox.Show(message, caption, buttons);
        }

        /// <summary>
        /// using the first 4 bytes of the file the user is trying to open, determine if the file is a compatible format with 
        /// this editor
        /// </summary>
        /// <param name="bytes">the first four bytes of the file</param>
        /// <returns></returns>
        private bool DeathspankFormatCheck(byte[] bytes)
        {
            //the file identifier used by both Datadict files and textdict files
            byte[] datadictid = new byte[] { 1, 0, 199, 209 };
            
            if(bytes.SequenceEqual(datadictid))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Take the passed in byte array assuing it represents a string. Pad the input with zeros until
        /// the total length is divisable evenly by four. Return the Zero padded string
        /// </summary>
        /// <param name="inputbytes"> a byte array that represents an ascii string that needs to be 
        /// zero padded until it is evenly divisible by 4 </param>
        /// <returns></returns>
        private byte[] ReturnZeroPaddedString( byte[] inputbytes)
        {
            //set a boolean used to check if the byte array length is divisable by 4
            bool remainder = false;
            //convert the input byte array to a list so bytes can be added to the end of the list easily
            List<byte> inputbyteslist = inputbytes.ToList();

            do
            {
                //add a 0 byte to the end of the list
                inputbyteslist.Add(0);
                //create an integer to hold the remainder in a future division operation
                int mFourCheck;

                //divide the number of bytes in the list by 4. Output the remainder to mFourCheck
                int mquotient = Math.DivRem(inputbyteslist.Count, 4, out mFourCheck);
                //check if the remainder is 0 a.k.a the list is evenly divisible by 4
                if (mFourCheck == 0)
                {
                    //set boolean to true to exit the do/while loop
                    remainder = true;
                }
                //if boolean is false repeat the loop
            } while (remainder == false);
            //once the list is evenly divisible by 4 convert it back to an array and return that array
            return inputbyteslist.ToArray();
        }

        private void SaveAs_Execute() 
        {
            SaveAsOutput.Clear();
            SaveAsAttributeDescription.Clear();
            SaveAsFinalData.Clear();
            SaveAsOutput = OpenFileBytes.Skip(0).Take(16 + (DDObjects.Count * 8)).ToList();

            for(int i =0; i< DDObjects.Count; i++)
            {
                ObservableCollection<DataDict_Attribute> SaveAsAttributes = DDObjects[i].DataDictObjects;
                int SaveAsNumAttributes = DDObjects[i].NumOfAttributes;
                // loop through all atrributes, save to the output attribute description list and final data list
                for (int a = 0; a < SaveAsNumAttributes; a++)
                {
                    SaveAsAttributeDescription.AddRange(DDObjects[i].DataDictObjects[a].Description.ToList<byte>());
                    
                    //Set the new offset based on the current length of the final data block
                    byte[] cbytes = BitConverter.GetBytes(SaveAsFinalData.Count);
                    SaveAsAttributeDescription.AddRange(cbytes.Skip(0).Take(3).ToList());
                    
                    SaveAsAttributeDescription.Add(DDObjects[i].DataDictObjects[a].Type);

                    if (DDObjects[i].DataDictObjects[a].Type == 13) {
                        SaveAsFinalData.AddRange(ReturnZeroPaddedString(DDObjects[i].DataDictObjects[a].Value));
                    }
                    else
                    {
                        SaveAsFinalData.AddRange(DDObjects[i].DataDictObjects[a].Value);
                    }

                }

            }

            SaveAsOutput.AddRange(SaveAsAttributeDescription);
            SaveAsOutput.AddRange(SaveAsFinalData);

            //update bytes 12-16 to represent the new length of data
            byte[] newdatalength = BitConverter.GetBytes(SaveAsFinalData.Count);

            //update the output bytes 4-8 with the new datablock length
            UpdateDatablockLength(newdatalength);

            
            //prepare to write the changes to a file by creating a new savefiledialog
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            //Set the window options to default to .datadict file format
            saveFileDialog1.Filter = "DeathSpank Data Files (*.datadict)| *.datadict";
            //set the save as window title
            saveFileDialog1.Title = "Save a DeathSpank Data File";
            //display the save as dialog option
            saveFileDialog1.ShowDialog();

            if (saveFileDialog1.FileName != "")
            {
                //Create a new filestream from the open file dialog options
                using (FileStream fs = (System.IO.FileStream)saveFileDialog1.OpenFile())
                {
                    //create a binary writer to write to the filestream
                    BinaryWriter bw = new BinaryWriter(fs);
                    //Write data to the new file 
                    bw.Write(SaveAsOutput.ToArray());

                    //flush and close the binarywriter
                    bw.Flush();
                    bw.Close();

                }
            }
        }

        ///change the orignal file's datablock length to match the length of the new data block length
        private void UpdateDatablockLength(byte[] newdatalength)
        {
            SaveAsOutput[12] = newdatalength[0];
            SaveAsOutput[13] = newdatalength[1];
            SaveAsOutput[14] = newdatalength[2];
            SaveAsOutput[15] = newdatalength[3];
        }

        /// <summary>
        /// this converts a 3 byte array into a 4 byte integer when populating a datadict attribute
        /// class object
        /// </summary>
        /// <param name="vs"> the initial 3 byte offset value</param>
        /// <returns></returns>
        private int ReturnOffset(byte[] vs)
        {
            //convert the 3 byte array into a list
            List<byte> bytelist = vs.ToList();
            //add a 0 byte to the end of the list
            bytelist.Add(0);
            //convert the list to a 32bit (4byte) integer
            int returnableint = BitConverter.ToInt32(bytelist.ToArray(),0);
            //return the integer value
            return returnableint;
        }

        /// <summary>
        /// A method that will return the value of any attribute when given the data type and
        /// its starting offset in the final data block
        /// </summary>
        /// <param name="offset"> The offset value for the start of the data in the data block</param>
        /// <param name="type">the type of data which determines the data length</param>
        /// <param name="datablock"> the datablock to search within</param>
        /// <returns></returns>
        private byte[] GetValue(int offset, byte type, byte[] datablock)
        {
            //create a new integer to track the number of bytes to return
            int bytestotake;

            if (type == 1 || type == 9)
            {
                //type 1 and 9 are always 4 bytes
                bytestotake = 4;
            }
            else if (type == 6 || type == 10)
            {
                //type 6 and 10 are always 8 bytes
                bytestotake = 8;
            }
            else if (type == 12)
            {
                //type 12 is always 16 bytes
                bytestotake = 16;
            }
            else
            {
                
                //to find the length of the string start at the initial offset given
                //then search forward for the first instance of a "0" byte to find
                //the offset of the "0" byte. subtract the original offset value to get the length of the string
                bytestotake = Array.IndexOf<byte>(datablock, 0, offset) - offset;
            }
         
            //return the bytes representing the attribute value
            return datablock.Skip(offset).Take(bytestotake).ToArray();
        }

        /// <summary>
        /// Shuts down the currently running application
        /// </summary>
        private void ExitApplication_Execute()
        {
            if (UnsavedChanges == 0) { 
                Application.Current.Shutdown(); 
            }
            
        }


        #endregion
    }
}
