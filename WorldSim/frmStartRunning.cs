using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace WorldSim
{
    public partial class frmStartRunning : Form
    {
        internal TestSettings testSettings;
        public frmStartRunning()
        {
            InitializeComponent();
            testSettings = new TestSettings();
        }

        private void frmStartRunning_Load(object sender, EventArgs e)
        {
            txtTicks.Text = testSettings.Duration.ToString();
            txtRepeats.Text = testSettings.Repeats.ToString();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                testSettings.Duration = Int32.Parse(txtTicks.Text);
                testSettings.Repeats = Int32.Parse(txtRepeats.Text);
            }
            catch
            {
                MessageBox.Show("There was a problem processing your input.  Please correct.");
                return;
            }
            this.Close();
        }

        /// <summary>
        /// Write the current dialog settings to a config file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnConfig_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.CheckPathExists = true;
            sfd.DefaultExt = "xml";
            sfd.Filter = "Xml Files (*.xml)|*.xml";
            sfd.OverwritePrompt = true;
            sfd.Title = "Store Configuration File";
            if (sfd.ShowDialog() != DialogResult.Cancel)
            {
                XmlSerializer SerializerObj = new XmlSerializer(typeof(TestSettings));
                TextWriter WriteFileStream = new StreamWriter(sfd.FileName);
                SerializerObj.Serialize(WriteFileStream, testSettings);
                WriteFileStream.Close();
            }
        }
    }
}