using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace NotepadClone
{
    public partial class FindReplaceForm : Form
    {
        public string FindText => textBox1.Text;
        public string ReplaceText => textBox2.Text;

        private RichTextBox _associatedRichTextBox;

        public FindReplaceForm(RichTextBox richTextBox)
        {
            InitializeComponent();
            _associatedRichTextBox = richTextBox;
        }

        // Find the next match
        private void button1_Click(object sender, EventArgs e)
        {
            int startIndex = 0;

            // If there is a current selection, start searching from the end of the selection
            if (_associatedRichTextBox.SelectionLength > 0)
            {
                startIndex = _associatedRichTextBox.SelectionStart + _associatedRichTextBox.SelectionLength;
            }
            else
            {
                startIndex = _associatedRichTextBox.SelectionStart;
            }

            // Search for the next match
            int foundIndex = _associatedRichTextBox.Text.IndexOf(FindText, startIndex);

            if (foundIndex != -1)  // If a match was found
            {
                _associatedRichTextBox.Select(foundIndex, FindText.Length);
                _associatedRichTextBox.ScrollToCaret();  // Scroll to the selection and make it visible
            }
            else
            {
                MessageBox.Show("Reached the end of document", "Find", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // Replace the currently selected text if it matches the search string
        private void button2_Click(object sender, EventArgs e)
        {
            // Replace the currently selcted text if it matches the search string
            if (_associatedRichTextBox.SelectedText == FindText)
            {
                _associatedRichTextBox.SelectedText = ReplaceText;
            }

            // Find the next match
            button1.PerformClick();
        }

        // Replace all matches
        private void button3_Click(object sender, EventArgs e)
        {
            _associatedRichTextBox.Text = _associatedRichTextBox.Text.Replace(FindText, ReplaceText);
        }
    }
}
