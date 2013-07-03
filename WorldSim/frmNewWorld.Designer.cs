namespace WorldSim
{
    partial class frmNewWorld
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtSizeY = new System.Windows.Forms.TextBox();
            this.txtSizeX = new System.Windows.Forms.TextBox();
            this.txtTilesY = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtTilesX = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.lstTileShape = new System.Windows.Forms.ComboBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lstTileShape);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtSizeY);
            this.groupBox1.Controls.Add(this.txtSizeX);
            this.groupBox1.Controls.Add(this.txtTilesY);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtTilesX);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(182, 104);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "World Dimensions";
            // 
            // txtSizeY
            // 
            this.txtSizeY.Location = new System.Drawing.Point(113, 45);
            this.txtSizeY.Name = "txtSizeY";
            this.txtSizeY.Size = new System.Drawing.Size(46, 20);
            this.txtSizeY.TabIndex = 5;
            // 
            // txtSizeX
            // 
            this.txtSizeX.Location = new System.Drawing.Point(61, 45);
            this.txtSizeX.Name = "txtSizeX";
            this.txtSizeX.Size = new System.Drawing.Size(46, 20);
            this.txtSizeX.TabIndex = 4;
            // 
            // txtTilesY
            // 
            this.txtTilesY.Location = new System.Drawing.Point(113, 19);
            this.txtTilesY.Name = "txtTilesY";
            this.txtTilesY.Size = new System.Drawing.Size(46, 20);
            this.txtTilesY.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Tile &size";
            // 
            // txtTilesX
            // 
            this.txtTilesX.Location = new System.Drawing.Point(61, 19);
            this.txtTilesX.Name = "txtTilesX";
            this.txtTilesX.Size = new System.Drawing.Size(46, 20);
            this.txtTilesX.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "&Tiles";
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(236, 12);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 8;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(236, 41);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 9;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 77);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(45, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Tile &size";
            // 
            // lstTileShape
            // 
            this.lstTileShape.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.lstTileShape.FormattingEnabled = true;
            this.lstTileShape.Items.AddRange(new object[] {
            "Rectangle",
            "Hexagon"});
            this.lstTileShape.Location = new System.Drawing.Point(61, 74);
            this.lstTileShape.Name = "lstTileShape";
            this.lstTileShape.Size = new System.Drawing.Size(98, 21);
            this.lstTileShape.TabIndex = 7;
            // 
            // frmNewWorld
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(323, 138);
            this.ControlBox = false;
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "frmNewWorld";
            this.Text = "New World Settings";
            this.Load += new System.EventHandler(this.frmNewWorld_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TextBox txtSizeY;
        private System.Windows.Forms.TextBox txtSizeX;
        private System.Windows.Forms.TextBox txtTilesY;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtTilesX;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox lstTileShape;
        private System.Windows.Forms.Label label3;
    }
}