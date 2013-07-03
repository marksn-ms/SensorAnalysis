namespace WorldSim
{
    partial class frmAddIndividuals
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.chkNonUniformIncidentDistribution = new System.Windows.Forms.CheckBox();
            this.nMaxTurnsBeforeMove = new System.Windows.Forms.NumericUpDown();
            this.label17 = new System.Windows.Forms.Label();
            this.nConstantIncident = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.lstAgentType = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.chkPointDeployment = new System.Windows.Forms.CheckBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtSensorRange = new System.Windows.Forms.TextBox();
            this.nPopulation = new System.Windows.Forms.NumericUpDown();
            this.label15 = new System.Windows.Forms.Label();
            this.txtRepeats = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtLogTicks = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.txtTicks = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btnConfig = new System.Windows.Forms.Button();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nMaxTurnsBeforeMove)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nConstantIncident)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nPopulation)).BeginInit();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(273, 12);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 31;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(273, 41);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 32;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.chkNonUniformIncidentDistribution);
            this.groupBox2.Controls.Add(this.nMaxTurnsBeforeMove);
            this.groupBox2.Controls.Add(this.label17);
            this.groupBox2.Controls.Add(this.nConstantIncident);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Location = new System.Drawing.Point(12, 141);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(255, 104);
            this.groupBox2.TabIndex = 18;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Incidents";
            // 
            // chkNonUniformIncidentDistribution
            // 
            this.chkNonUniformIncidentDistribution.AutoSize = true;
            this.chkNonUniformIncidentDistribution.Location = new System.Drawing.Point(11, 72);
            this.chkNonUniformIncidentDistribution.Name = "chkNonUniformIncidentDistribution";
            this.chkNonUniformIncidentDistribution.Size = new System.Drawing.Size(138, 17);
            this.chkNonUniformIncidentDistribution.TabIndex = 23;
            this.chkNonUniformIncidentDistribution.Text = "N&on-uniform Distribution";
            this.toolTip1.SetToolTip(this.chkNonUniformIncidentDistribution, "Checked means all sensors deployed from same spot; unchecked means random deploym" +
                    "ent across field.");
            this.chkNonUniformIncidentDistribution.UseVisualStyleBackColor = true;
            // 
            // nMaxTurnsBeforeMove
            // 
            this.nMaxTurnsBeforeMove.Location = new System.Drawing.Point(178, 45);
            this.nMaxTurnsBeforeMove.Maximum = new decimal(new int[] {
            32767,
            0,
            0,
            0});
            this.nMaxTurnsBeforeMove.Name = "nMaxTurnsBeforeMove";
            this.nMaxTurnsBeforeMove.Size = new System.Drawing.Size(61, 20);
            this.nMaxTurnsBeforeMove.TabIndex = 22;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(8, 47);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(115, 13);
            this.label17.TabIndex = 21;
            this.label17.Text = "Ma&x turns before move";
            // 
            // nConstantIncident
            // 
            this.nConstantIncident.Location = new System.Drawing.Point(178, 19);
            this.nConstantIncident.Name = "nConstantIncident";
            this.nConstantIncident.Size = new System.Drawing.Size(61, 20);
            this.nConstantIncident.TabIndex = 20;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 21);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(50, 13);
            this.label3.TabIndex = 19;
            this.label3.Text = "Incid&ents";
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.lstAgentType);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.textBox3);
            this.groupBox3.Controls.Add(this.textBox2);
            this.groupBox3.Controls.Add(this.textBox1);
            this.groupBox3.Controls.Add(this.chkPointDeployment);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.txtSensorRange);
            this.groupBox3.Controls.Add(this.nPopulation);
            this.groupBox3.Controls.Add(this.label15);
            this.groupBox3.Location = new System.Drawing.Point(12, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(255, 123);
            this.groupBox3.TabIndex = 1;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "&Inhabitants";
            // 
            // lstAgentType
            // 
            this.lstAgentType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.lstAgentType.FormattingEnabled = true;
            this.lstAgentType.Location = new System.Drawing.Point(72, 13);
            this.lstAgentType.Name = "lstAgentType";
            this.lstAgentType.Size = new System.Drawing.Size(167, 21);
            this.lstAgentType.TabIndex = 1;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(8, 16);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(58, 13);
            this.label5.TabIndex = 22;
            this.label5.Text = "&Agent type";
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(210, 92);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(29, 20);
            this.textBox3.TabIndex = 21;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(175, 92);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(29, 20);
            this.textBox2.TabIndex = 20;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(140, 92);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(29, 20);
            this.textBox1.TabIndex = 19;
            // 
            // chkPointDeployment
            // 
            this.chkPointDeployment.AutoSize = true;
            this.chkPointDeployment.Location = new System.Drawing.Point(11, 95);
            this.chkPointDeployment.Name = "chkPointDeployment";
            this.chkPointDeployment.Size = new System.Drawing.Size(107, 17);
            this.chkPointDeployment.TabIndex = 18;
            this.chkPointDeployment.Text = "&Point deployment";
            this.toolTip1.SetToolTip(this.chkPointDeployment, "Checked means all sensors deployed from same spot; unchecked means random deploym" +
                    "ent across field.");
            this.chkPointDeployment.UseVisualStyleBackColor = true;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(8, 69);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(70, 13);
            this.label9.TabIndex = 16;
            this.label9.Text = "&Sensor range";
            // 
            // txtSensorRange
            // 
            this.txtSensorRange.Location = new System.Drawing.Point(178, 66);
            this.txtSensorRange.Name = "txtSensorRange";
            this.txtSensorRange.Size = new System.Drawing.Size(61, 20);
            this.txtSensorRange.TabIndex = 17;
            this.txtSensorRange.Text = "0";
            this.txtSensorRange.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // nPopulation
            // 
            this.nPopulation.Location = new System.Drawing.Point(178, 40);
            this.nPopulation.Name = "nPopulation";
            this.nPopulation.Size = new System.Drawing.Size(61, 20);
            this.nPopulation.TabIndex = 3;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(8, 42);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(57, 13);
            this.label15.TabIndex = 2;
            this.label15.Text = "&Population";
            // 
            // txtRepeats
            // 
            this.txtRepeats.Location = new System.Drawing.Point(178, 71);
            this.txtRepeats.Name = "txtRepeats";
            this.txtRepeats.Size = new System.Drawing.Size(61, 20);
            this.txtRepeats.TabIndex = 30;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 74);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(99, 13);
            this.label4.TabIndex = 29;
            this.label4.Text = "N&umber of test runs";
            // 
            // txtLogTicks
            // 
            this.txtLogTicks.Location = new System.Drawing.Point(178, 45);
            this.txtLogTicks.Name = "txtLogTicks";
            this.txtLogTicks.Size = new System.Drawing.Size(61, 20);
            this.txtLogTicks.TabIndex = 28;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 27;
            this.label2.Text = "&Log ticks:";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.txtRepeats);
            this.groupBox5.Controls.Add(this.txtLogTicks);
            this.groupBox5.Controls.Add(this.label4);
            this.groupBox5.Controls.Add(this.txtTicks);
            this.groupBox5.Controls.Add(this.label2);
            this.groupBox5.Controls.Add(this.label1);
            this.groupBox5.Location = new System.Drawing.Point(12, 251);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(254, 105);
            this.groupBox5.TabIndex = 24;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Test Run Options";
            // 
            // txtTicks
            // 
            this.txtTicks.Location = new System.Drawing.Point(178, 19);
            this.txtTicks.Name = "txtTicks";
            this.txtTicks.Size = new System.Drawing.Size(61, 20);
            this.txtTicks.TabIndex = 26;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 13);
            this.label1.TabIndex = 25;
            this.label1.Text = "T&icks to run:";
            // 
            // btnConfig
            // 
            this.btnConfig.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnConfig.Location = new System.Drawing.Point(273, 71);
            this.btnConfig.Name = "btnConfig";
            this.btnConfig.Size = new System.Drawing.Size(75, 23);
            this.btnConfig.TabIndex = 33;
            this.btnConfig.Text = "&Config...";
            this.btnConfig.UseVisualStyleBackColor = true;
            this.btnConfig.Click += new System.EventHandler(this.btnConfig_Click);
            // 
            // frmAddIndividuals
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(360, 368);
            this.ControlBox = false;
            this.Controls.Add(this.btnConfig);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "frmAddIndividuals";
            this.ShowInTaskbar = false;
            this.Text = "Add Individuals";
            this.Load += new System.EventHandler(this.frmAddIndividuals_Load);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nMaxTurnsBeforeMove)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nConstantIncident)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nPopulation)).EndInit();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtSensorRange;
        private System.Windows.Forms.NumericUpDown nPopulation;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.NumericUpDown nMaxTurnsBeforeMove;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.NumericUpDown nConstantIncident;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtRepeats;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtLogTicks;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.TextBox txtTicks;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chkPointDeployment;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.CheckBox chkNonUniformIncidentDistribution;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox lstAgentType;
        private System.Windows.Forms.Button btnConfig;
    }
}