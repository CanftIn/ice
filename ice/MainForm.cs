using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace ice
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void aboutAToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (AboutForm tForm = new AboutForm())
            {
                tForm.ShowDialog();
            }
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DocumentForm tForm = new DocumentForm();
            tForm.Show();// tForm.Show(dockPanel_main, DockState.Document);
        }

        private void exitEToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void openOToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.DefaultExt = "ice";
            ofd.Filter = "ice (*.ice)|*.ice";
            ofd.Title = "Open ice file";
            
            if (ofd.ShowDialog() == DialogResult.OK && ofd.FileName.Length > 0)
            {
                DocumentForm tForm = new DocumentForm(ofd.FileName);
                tForm.Show();//tForm.Show(dockPanel_main, DockState.Document);
            }
        }
    }
}
