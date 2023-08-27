# **Levi's Notepad Clone**

This is a simple text editor built to resemble the functionality of the classic Windows Notepad application, with additional features like dark mode and text statistics.


### **Features:**
1. Basic Text Editing: You can write, edit, and manipulate text just like in any text editor.

2. Open & Save: You can open text files from your computer, edit them, and then save the changes either back to the same file or as a new file.

3. Syntax Highlighting: The application supports basic syntax highlighting for strings, keywords, and comments.

4. Print & Preview: It's possible to print the text directly from the application, and there's also a feature to preview the text before printing.

5. Find & Replace: You can find specific text within the document and replace it with something else.

6. AutoSave: The application supports an auto-save feature, which saves your work automatically at regular intervals.

7. Dark Mode: For those who prefer working in a low-light environment, there's a dark mode feature which can be toggled on or off.

8. Text Statistics: The application can display statistics for your text, such as the number of words, characters, sentences, etc.

9. Insert Current Time & Date: You can easily insert the current time and date into your text with just one click.

10. Font Customization: The application lets you choose the font style, size, and other attributes for the text.


### **Code Structure:**
The application primarily consists of a single Form called Form1, which contains most of the core functionality:

- richTextBox1_TextChanged: Checks if any change is made to the text and accordingly enables or disables some menu items like "Undo", "Find", and "Find & Replace".

- HighlightSyntax: Implements the basic syntax highlighting feature for strings, keywords, and comments.

- File Handling: The methods openToolStripMenuItem_Click, SaveFile, and saveAsToolStripMenuItem_Click provide functionalities to open, save, and save-as the text respectively.

- Print & Preview: PrintDocument and PrintPreview methods are used for printing the document and previewing before printing.

- Find & Replace: The methods findToolStripMenuItem_Click and findAndReplaceToolStripMenuItem_Click manage the 'Find' and 'Find & Replace' functionalities.

- AutoSave: The autoSaveTimer_Tick method saves the document automatically, based on the timer's interval.

- Dark Mode: The ToggleDarkMode method lets users switch between dark and light themes.

- Text Statistics: statisticsToolStripMenuItem_Click displays a form with details about word count, character count, etc.


### **How to Use:**
1. Start the application.
2. You can immediately start typing, or open a text file using the "File" > "Open" option.
3. Use the various options in the menu bar to access all features like "Save", "Print", "Find", "Dark Mode", etc.
4. To view text statistics, go to the menu and select the appropriate option to get details like word count, sentence count, etc.
