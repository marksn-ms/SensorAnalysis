using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using WorldSim.Interface;
using System.Xml.Serialization;

namespace WorldSim
{
    public partial class frmAddIndividuals : Form
    {
        public List<string> m_strAgentTypes;
        internal TestSettings m_testSettings;
        
        /// <summary>
        /// Constructor for the frmAddIndividuals form.
        /// </summary>
        public frmAddIndividuals()
        {
            InitializeComponent();
            m_strAgentTypes = new List<string>();
            m_testSettings = new TestSettings();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                m_testSettings.Agent = new string[] { lstAgentType.SelectedItem.ToString() };
                m_testSettings.Population = new int[] { Convert.ToInt32(nPopulation.Value) };
                m_testSettings.SensorRange = new int[] { Convert.ToInt32(txtSensorRange.Text) };
                m_testSettings.PointDeployment = chkPointDeployment.Checked;

                m_testSettings.Incident = Convert.ToInt32(nConstantIncident.Value);
                m_testSettings.IncidentMaxTurnsBeforeMove = Convert.ToInt32(nMaxTurnsBeforeMove.Value);
                m_testSettings.NonUniformIncidentDistribution = chkNonUniformIncidentDistribution.Checked;

                m_testSettings.LogFrequency = Int32.Parse(txtLogTicks.Text);
                m_testSettings.Duration = Int32.Parse(txtTicks.Text);
                m_testSettings.Repeats = Int32.Parse(txtRepeats.Text);
                m_testSettings.RewardScaleP_s = double.Parse(textBox1.Text);
                m_testSettings.RewardScaleP_e = double.Parse(textBox2.Text);
                m_testSettings.RewardScaleP_n = double.Parse(textBox3.Text);
            }
            catch
            {
                MessageBox.Show("Unable to validate value.  Try again.");
                DialogResult = DialogResult.None;
                return;
            }
            this.Close();
        }

        private void frmAddIndividuals_Load(object sender, EventArgs e)
        {
            lstAgentType.Items.AddRange(m_strAgentTypes.ToArray());
            if (m_testSettings.Agent== null || m_testSettings.Agent.Length == 0)
                lstAgentType.SelectedIndex = -1;
            else
                lstAgentType.SelectedItem = m_testSettings.Agent[0];
            nPopulation.Value = m_testSettings.Population[0];
            nConstantIncident.Value = m_testSettings.Incident;
            nMaxTurnsBeforeMove.Value = m_testSettings.IncidentMaxTurnsBeforeMove;
            txtSensorRange.Text = m_testSettings.SensorRange[0].ToString();
            txtLogTicks.Text = m_testSettings.LogFrequency.ToString();
            txtTicks.Text = m_testSettings.Duration.ToString();
            txtRepeats.Text = m_testSettings.Repeats.ToString();
            chkPointDeployment.Checked = m_testSettings.PointDeployment;
            chkNonUniformIncidentDistribution.Checked = m_testSettings.NonUniformIncidentDistribution;
            textBox1.Text = m_testSettings.RewardScaleP_s.ToString();
            textBox2.Text = m_testSettings.RewardScaleP_e.ToString();
            textBox3.Text = m_testSettings.RewardScaleP_n.ToString();
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
                SerializerObj.Serialize(WriteFileStream, m_testSettings);
                WriteFileStream.Close();
            }
        }
    }
}