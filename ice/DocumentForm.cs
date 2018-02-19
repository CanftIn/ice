using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace ice
{
    public partial class DocumentForm : Form //DockContent
    {
        private PaintForm mPaintForm = new PaintForm();

        private string mFilePath = String.Empty;
        private string mTitle = "no title";
        private bool mEdited = false;
        private bool mStopPaint = false;
        public DocumentForm()
        {
            InitializeComponent();

            RefreshTitle();
        }

        public DocumentForm(string filePath)
        {
            InitializeComponent();

            loadDocument(filePath);
        }

        private void RefreshTitle()
        {
            this.Text = String.Format("{0}{1}", mTitle, mEdited ? "*" : String.Empty);
        }

        /// <summary>
        /// load document
        /// </summary>
        /// <param name="filePath">file path</param>
        private void loadDocument(string filePath)
        {
            mFilePath = filePath;
            mTitle = Path.GetFileNameWithoutExtension(filePath);

            try
            {
                richTextBox_main.Text = File.ReadAllText(mFilePath, Encoding.UTF8);
            }
            catch (Exception e)
            {
                MessageBox.Show(String.Format("Open file\"{0}\"fail. \n\n{1}", mFilePath, e.ToString()), "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            RefreshTitle();
        }

        /// <summary>
        /// save document
        /// </summary>
        /// <param name="filePath">file path</param>
        private void saveDocument(string filePath)
        {
            mFilePath = filePath;
            mTitle = Path.GetFileNameWithoutExtension(filePath);

            try
            {
                File.WriteAllText(mFilePath, richTextBox_main.Text, Encoding.UTF8);
                mEdited = false;
            }
            catch (Exception e)
            {
                MessageBox.Show(String.Format("Save file\"{0}\"fail. \n\n{1}", mFilePath, e.ToString()), "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                mFilePath = String.Empty;
            }

            RefreshTitle();
        }

        public string FilePath
        {
            get { return mFilePath; }
        }

        public bool Edited
        {
            get { return mEdited; }
        }



        private void ToolStripMenuItem_Save_Click(object sender, EventArgs e)
        {
            if(String.IsNullOrEmpty(mFilePath))
            {
                ToolStripMenuItem_SaveAs_Click(sender, e);
            }
            else
            {
                saveDocument(mFilePath);
            }
        }

        private void DocumentForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(mEdited)
            {
                if (DialogResult.No == MessageBox.Show("The current document has not been saved yet, do you want to close it?", "warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning))
                    e.Cancel = true;
            }
        }

        private void ToolStripMenuItem_SaveAs_Click(object sender, EventArgs e)
        {
            SaveFileDialog ofd = new SaveFileDialog();
            ofd.DefaultExt = "ice";
            ofd.Filter = "ice (*.ice)|*.ice";
            ofd.Title = "Save ice file";
            if(ofd.ShowDialog() == DialogResult.OK)
            {
                saveDocument(ofd.FileName);
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            mPaintForm.Show();
            mPaintForm.Activate();
            mPaintForm.SubmitSourceCode(richTextBox_main.Text);
        }
    }
}
