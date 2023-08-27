using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NotepadClone
{
    public partial class Form1 : Form
    {
        private bool textChanged = false;
        private int searchStart = 0;  // To remember the last search position
        private bool isDarkMode = false;  // To track the current theme
        private string currentFilePath = null;  // To remember the current file name

        public Form1()
        {
            InitializeComponent();
            this.ActiveControl = richTextBox1;

            // Disable the menu items that are not available at startup
            undoToolStripMenuItem.Enabled = false;
            findToolStripMenuItem.Enabled = false;
            findAndReplaceToolStripMenuItem.Enabled = false;

            richTextBox1.TextChanged += richTextBox1_TextChanged;  // Subscribe to the TextChanged event
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            textChanged = true;
            HighlightSyntax(richTextBox1);

            if (string.IsNullOrWhiteSpace(richTextBox1.Text))
            {
                undoToolStripMenuItem.Enabled = false;
                findToolStripMenuItem.Enabled = false;
                findAndReplaceToolStripMenuItem.Enabled = false;
            }
            else
            {
                undoToolStripMenuItem.Enabled = richTextBox1.CanUndo;
                findToolStripMenuItem.Enabled = true;
                findAndReplaceToolStripMenuItem.Enabled = true;
            }
        }

        private void HighlightSyntax(RichTextBox rtb)
        {
            // Backup current selection
            int selectionStart = rtb.SelectionStart;
            int selectionLength = rtb.SelectionLength;

            // Set the default color
            rtb.SelectAll();
            rtb.SelectionColor = Color.Black;

            // Highlighting strings enclosed in double quotes: "..."
            int stringStartIdx = 0;
            int stringEndIdx = 0;

            while (stringStartIdx < rtb.Text.Length)
            {
                // Find the start of the string (first occurrence of a double quote)
                stringStartIdx = rtb.Find("\"", stringStartIdx, RichTextBoxFinds.None);

                if (stringStartIdx == -1) break; // No more strings found, exit the loop

                // Find the end of the string (next occurrence of a double quote after the starting one)
                stringEndIdx = rtb.Find("\"", stringStartIdx + 1, RichTextBoxFinds.None);

                if (stringEndIdx == -1) break; // No closing quote found, exit the loop

                // Select the string and change its color
                rtb.Select(stringStartIdx, stringEndIdx - stringStartIdx + 1);
                rtb.SelectionColor = Color.Brown;

                // Move to the next character after the end quote for the next iteration
                stringStartIdx = stringEndIdx + 1;
            }


            // Keywords
            Dictionary<string, Color> keywords = new Dictionary<string, Color>()
            {
                { "abstract", Color.Blue },
                { "bool", Color.Blue },
                { "break", Color.Blue },
                { "byte", Color.Blue },
                { "case", Color.Blue },
                { "catch", Color.Blue },
                { "char", Color.Blue },
                { "class", Color.Blue },
                { "const", Color.Blue },
                { "continue", Color.Blue },
                { "decimal", Color.Blue },
                { "do", Color.Blue },
                { "double", Color.Blue },
                { "else", Color.Blue },
                { "enum", Color.Blue },
                { "false", Color.Blue },
                { "float", Color.Blue },
                { "for", Color.Blue },
                { "foreach", Color.Blue },
                { "goto", Color.Blue },
                { "if", Color.Blue },
                { "int", Color.Blue },
                { "interface", Color.Blue },
                { "long", Color.Blue },
                { "namespace", Color.Blue },
                { "null", Color.Blue },
                { "object", Color.Blue },
                { "override", Color.Blue },
                { "params", Color.Blue },
                { "private", Color.Blue },
                { "protected", Color.Blue },
                { "public", Color.Blue },
                { "readonly", Color.Blue },
                { "return", Color.Blue },
                { "sbyte", Color.Blue },
                { "short", Color.Blue },
                { "sizeof", Color.Blue },
                { "static", Color.Blue },
                { "string", Color.Blue },
                { "struct", Color.Blue },
                { "switch", Color.Blue },
                { "throw", Color.Blue },
                { "true", Color.Blue },
                { "try", Color.Blue },
                { "uint", Color.Blue },
                { "ulong", Color.Blue },

            };
            
            foreach (var k in keywords)
            {
                int startIndex = 0;
                while (startIndex < rtb.TextLength)
                {
                    // Find the word's starting index
                    int wordStartIndex = rtb.Find(k.Key, startIndex, RichTextBoxFinds.WholeWord);

                    if (wordStartIndex == -1)  // If the word is not found, exit
                        break;

                    // Select the word using the index and the length of the word
                    rtb.SelectionStart = wordStartIndex;
                    rtb.SelectionLength = k.Key.Length;

                    // Highlight the word
                    rtb.SelectionColor = k.Value;

                    // Move the search index to the end of the word
                    startIndex = wordStartIndex + k.Key.Length;
                }
            }

            // Highlighting Python-style comments: #
            int commentIdx = 0;
            while ((commentIdx = rtb.Find("#", commentIdx, RichTextBoxFinds.None)) != -1)
            {
                int endOfLine = rtb.Text.IndexOf('\n', commentIdx);
                if (endOfLine == -1) endOfLine = rtb.Text.Length;
                rtb.Select(commentIdx, endOfLine - commentIdx);
                rtb.SelectionColor = Color.Green;
                commentIdx = endOfLine;
            }

            // Highlighting C/C++/C# style comments: //
            int cCommentIdx = 0;
            while ((cCommentIdx = rtb.Find("//", cCommentIdx, RichTextBoxFinds.None)) != -1)
            {
                int endOfLine = rtb.Text.IndexOf('\n', cCommentIdx);
                if (endOfLine == -1) endOfLine = rtb.Text.Length;
                rtb.Select(cCommentIdx, endOfLine - cCommentIdx);
                rtb.SelectionColor = Color.Green;
                cCommentIdx = endOfLine;
            }

            // Restore the original selection
            rtb.SelectionStart = selectionStart;
            rtb.SelectionLength = selectionLength;
            rtb.SelectionColor = Color.Black;
        }

        #region File Menu

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if(openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    richTextBox1.Text = System.IO.File.ReadAllText(openFileDialog.FileName);
                    textChanged = false;  // Reset the textChanged flag after loading the file

                    this.Text = Path.GetFileName(openFileDialog.FileName) + " - Notepad Clone";  // Set the title of the form to the name of the file
                }
            }
        }

        #region Printing
        // The printing method
        private void PrintDocument()
        {
            PrintDialog printDialog = new PrintDialog();
            PrintDocument printDocument = new PrintDocument();

            printDialog.Document = printDocument;  // Add the document to the dialog box

            // If the user clicks the OK button, print the document
            if (printDialog.ShowDialog() == DialogResult.OK)
            {
                printDocument.PrintPage += new PrintPageEventHandler(PrintDocument_PrintPage);  // Delegate a method to handle printing a page
                printDocument.Print();
            }
        }

        // Handle the PrintPage event to provide the print logic for the document
        private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            // Print the content of RichTextBox. Store the last character printed.
            e.Graphics.DrawString(richTextBox1.Text, new Font("Times New Roman", 12, FontStyle.Regular), Brushes.Black, new PointF(100, 100));
        }

        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PrintDocument();
        }

        // Print Preview
        private void PrintPreview()
        {
            PrintDocument printDocument = new PrintDocument();
            printDocument.PrintPage += new PrintPageEventHandler(PrintDocument_PrintPage);

            PrintPreviewDialog printPreviewDialog = new PrintPreviewDialog();
            printPreviewDialog.Document = printDocument;

            // Set the PrintPreviewDialog to open in maximized form
            printPreviewDialog.WindowState = FormWindowState.Maximized;

            printPreviewDialog.ShowDialog();
        }

        private void printPreviewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PrintPreview();
        }
        #endregion

        #region Closing while unsaved
        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseApplication();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (textChanged)
            {
                DialogResult result = MessageBox.Show("Do you want to save changes?", "Unsaved Changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    SaveFile();  // Calling the SaveFile method here
                }
                else if (result == DialogResult.Cancel)
                {
                    e.Cancel = true;  // Prevents the form from closing
                }
            }
        }

        private void CloseApplication()
        {
            if (textChanged)
            {
                DialogResult result = MessageBox.Show("Do you want to save changes?", "Unsaved Changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    SaveFile();  // Calling the SaveFile method here
                }
                else if (result == DialogResult.No)
                {
                    Application.Exit();
                }
            }
            else
            {
                Application.Exit();
            }
        }
        #endregion

        #region Save/Save As
        private void saveFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFile();
        }

        private void SaveFile()
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.InitialDirectory = "c:\\";
                saveFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                saveFileDialog.FilterIndex = 2;
                saveFileDialog.RestoreDirectory = true;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    System.IO.File.WriteAllText(saveFileDialog.FileName, richTextBox1.Text);
                }
            }

            textChanged = false;
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveAs();
        }

        private void SaveAs()
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                saveFileDialog.Title = "Save As";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    currentFilePath = saveFileDialog.FileName;
                    SaveFile(currentFilePath);
                }
            }
        }

        private void SaveFile(string path)
        {
            try
            {
                File.WriteAllText(path, richTextBox1.Text);
                this.Text = Path.GetFileName(path) + " - Notepad Clone";  // Set the title of the form to the name of the file
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while saving the file: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Auto Save
        private void autoSaveTimer_Tick(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(currentFilePath))
            {
                SaveAs();
            }
            else
            {
                SaveFile(currentFilePath);
            }
        }

        private void autoSaveCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (autoSaveCheckBox.Checked)
            {
                autoSaveTimer.Start();
            }
            else
            {
                autoSaveTimer.Stop();
            }
        }
        #endregion

        #endregion

        #region Edit Menu
        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (richTextBox1.CanUndo)
            {
                richTextBox1.Undo();
                // Clear the undo buffer to prevent redo
                richTextBox1.ClearUndo();
            }
        }

        
        #region Find
        private void findToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string searchQuery = PromptInput("Find", "Enter text to search:");

            if (!string.IsNullOrEmpty(searchQuery))
            {
                FindAndHighlight(searchQuery);
            }
        }

        private string PromptInput(string title, string promptText)
        {
            string input = Microsoft.VisualBasic.Interaction.InputBox(promptText, title, "", -1, -1);
            return input;
        }

        private void FindAndHighlight(string searchQuery)
        {
            int index = richTextBox1.Find(searchQuery, searchStart, RichTextBoxFinds.None);

            if (index != -1)
            {
                // Text found. Select the text
                richTextBox1.Select(index, searchQuery.Length);
                searchStart = index + searchQuery.Length;  // Set the searchStart to the next character after the found text
            }
            else
            {
                MessageBox.Show($"Unable to find \"{searchQuery}\".", "Not found", MessageBoxButtons.OK, MessageBoxIcon.Information);
                searchStart = 0;  // Reset the searchStart
            }
        }

        #endregion

        // Replace
        private void findAndReplaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FindReplaceForm findReplaceForm = new FindReplaceForm(richTextBox1);
            findReplaceForm.Show();
        }

        private void timeAndDateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string currentTimeAndDate = DateTime.Now.ToString("HH:mm yyyy-MM-dd");
            richTextBox1.SelectedText = currentTimeAndDate;  // Insert the current time and date at the current cursor position
        }

        // Change font
        private void changeFontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (FontDialog fontDialog = new FontDialog())
            {
                fontDialog.Font = richTextBox1.SelectionFont;  // Set the current font in the font dialog

                if (fontDialog.ShowDialog() == DialogResult.OK)
                {
                    // If user has selected text, change the font of the selected text only.
                    // Otherwise, change the font of the entire text in the RichTextBox
                    if (richTextBox1.SelectedText.Length > 0)
                        richTextBox1.SelectionFont = fontDialog.Font;
                    else
                        richTextBox1.Font = fontDialog.Font;
                }
            }
        }

        #endregion

        #region View Menu
        private void darkModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToggleDarkMode();
        }

        private void ToggleDarkMode()
        {
            if (isDarkMode)
            {
                // Switch to light mode
                this.BackColor = SystemColors.Control;
                richTextBox1.BackColor = Color.White;
                richTextBox1.ForeColor = Color.Black;
                menuStrip1.BackColor = SystemColors.Menu;
                menuStrip1.ForeColor = Color.Black;

                isDarkMode = false;
                darkModeToolStripMenuItem.Text = "Dark Mode";
            }
            else
            {
                // Switch to dark mode
                this.BackColor = Color.Black;
                richTextBox1.BackColor = Color.Black;
                richTextBox1.ForeColor = Color.White;
                menuStrip1.BackColor = Color.Black;
                menuStrip1.ForeColor = Color.White;

                isDarkMode = true;
                darkModeToolStripMenuItem.Text = "Light Mode";  // Change the text of the menu item to let the user know that they can switch to light mode
            }
        }

        private void statisticsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TextStatisticsForm statsForm = new TextStatisticsForm();

            // Calculate the statistics using the TextAnalyzer class
            int wordCount = TextAnalyzer.CountWords(richTextBox1.Text);
            int charCount = TextAnalyzer.CountCharacters(richTextBox1.Text, true);
            int charCountNoSpaces = TextAnalyzer.CountCharacters(richTextBox1.Text, false);
            int sentenceCount = TextAnalyzer.CountSentences(richTextBox1.Text);

            // Assign the values to the labels in the form
            statsForm.wordCountLabel.Text = $"{wordCount}";
            statsForm.charCountLabel.Text = $"{charCount}";
            statsForm.charWoSpaceLabel.Text = $"{charCountNoSpaces}";
            statsForm.sentenceCountLabel.Text = $"{sentenceCount}";

            // Show the form
            statsForm.ShowDialog();
        }
        #endregion

    }
}
