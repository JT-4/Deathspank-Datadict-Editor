This GUI based editor was developed to provide an easier way of editing Deathspank ".datadict" files. By coincidence, the deathspank ".textdict" have the same format and can be edited using this same editor. This should allow for easier fan translations if that is your goal.


Using the program:
 - Open a deathspank datadict file using the file menu
 - This will populate a list of the objects described in the file in the listbox on the left side of the window
 - Selecting a object in the listbox on the left will show the attributes associated to that object
 - Modify the desired values by editing the "values" field in the right side window
 - Use File > Save As to output the changes to a new file.
 
 If you enter an incorrect value into a byte field, the texbox will be highlighted in red.
 
 The saved files have a slightly different format than the original files. The original files use a form of compression/deduplication that I purposfully removed. This is so every object attribute value has a unique entry in the file instead of having multiple attributes with pointers to the same data. This allows for editing a single object at a time without affecting any other object attributes. See this writeup for more information: https://overkillscripting.home.blog/2021/01/24/hero-to-the-downtrodden-reverse-engineering-and-improving-deathspank/ 
 
 
