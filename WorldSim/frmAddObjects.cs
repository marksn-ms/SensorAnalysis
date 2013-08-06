using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using WorldSim.Interface;

namespace WorldSim
{
    public partial class frmAddObjects : Form
    {
        /// <summary>
        /// List initialized by user of this dialog box to indicate
        /// what object types can be added and the type that represents
        /// each object.
        /// </summary>
        public Dictionary<string,Type> m_agentTypes;

        /// <summary>
        /// The type of agent that should be created.
        /// </summary>
        public Type m_agentType;
        public String m_strAgentType;

        /// <summary>
        /// The number of objects of the selected type that should be created.
        /// </summary>
        public int m_nPopulation;

        public frmAddObjects()
        {
            InitializeComponent();
            
            m_agentTypes = new Dictionary<string, Type>();
            m_agentType = null;
            m_nPopulation = 100;
        }

        private void lstAgentType_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                propertyGrid1.SelectedObject = Activator.CreateInstance(m_agentTypes[lstAgentType.SelectedItem.ToString()], null);
                textBox1.Text = ((SelectableObject)propertyGrid1.SelectedObject).Info();
            }
            catch(Exception ex)
            {
                MessageBox.Show("Unable to create that object, try another.");
                propertyGrid1.SelectedObject = null;
                textBox1.Text = "";
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                m_agentType = m_agentTypes[lstAgentType.SelectedItem.ToString()];
                m_nPopulation = Convert.ToInt32(nPopulation.Value);

                string strKind = "Inhabitant";
                object o = propertyGrid1.SelectedObject;

                m_strAgentType = JsonHelper.WorldTypeToJson(strKind, o, m_nPopulation);
            }
            catch
            {
                MessageBox.Show("Unable to validate value.  Try again.");
                DialogResult = DialogResult.None;
                return;
            }
            this.Close();
        }

        private void frmAddObjects_Load(object sender, EventArgs e)
        {
            if (m_agentTypes != null && m_agentTypes.Keys.Count > 0)
                foreach (string str in m_agentTypes.Keys)
                    lstAgentType.Items.Add(str);
            if (m_agentType == null)
                lstAgentType.SelectedIndex = -1;
            else
                lstAgentType.SelectedItem = m_agentType;
            nPopulation.Value = m_nPopulation;

            Attribute filterAttribute = new CategoryAttribute("Initialization");
            propertyGrid1.BrowsableAttributes = new AttributeCollection(new Attribute[] { filterAttribute });
        }

    }
}