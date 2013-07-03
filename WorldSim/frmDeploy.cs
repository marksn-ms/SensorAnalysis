using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using WorldSim.Interface;

namespace WorldSim
{
    public partial class frmDeploy : Form
    {
        /// <summary>
        /// List initialized by user of this dialog box to indicate
        /// what object types can be added and the type that represents
        /// each object.
        /// </summary>
        public Dictionary<string, Type> m_deployTypes;

        /// <summary>
        /// The type of agent that should be created.
        /// </summary>
        public Type m_deployType;

        public frmDeploy()
        {
            InitializeComponent();
            m_deployTypes = new Dictionary<string, Type>();
            m_deployType = null;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            m_deployType = m_deployTypes[listBox1.SelectedItem.ToString()];
            this.Close();
        }

        private void frmDeploy_Load(object sender, EventArgs e)
        {
            if (m_deployTypes != null && m_deployTypes.Keys.Count > 0)
                foreach (string str in m_deployTypes.Keys)
                    listBox1.Items.Add(str);
            if (m_deployType == null)
                listBox1.SelectedIndex = -1;
            else
                listBox1.SelectedItem = m_deployType;

            //SetSelectionText(m_deployTypes[listBox1.SelectedItem.ToString()]);
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetSelectionText(m_deployTypes[listBox1.SelectedItem.ToString()]);
        }

        private void SetSelectionText(Type t)
        {
            try
            {
                World w = null;
                Deployer d = (Deployer)Activator.CreateInstance(t, w);
                txtInfo.Text = d.Info();
            }
            catch(Exception e)
            {
                // swallow any exceptions
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }
    }
}