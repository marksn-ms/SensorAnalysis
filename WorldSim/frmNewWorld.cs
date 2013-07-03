using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WorldSim
{
    public partial class frmNewWorld : Form
    {
        public Size Tiles;
        public Size TileSize;
        public string TileShape;

        public frmNewWorld()
        {
            InitializeComponent();
            Tiles = new Size(10, 10);
            TileSize = new Size(100, 100);
            TileShape = "Rectangle";
        }

        private void frmNewWorld_Load(object sender, EventArgs e)
        {
            txtSizeX.Text = TileSize.Width.ToString();
            txtSizeY.Text = TileSize.Height.ToString();
            txtTilesX.Text = Tiles.Width.ToString();
            txtTilesY.Text = Tiles.Height.ToString();
            lstTileShape.SelectedItem = TileShape;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                TileSize.Width = Int32.Parse(txtSizeX.Text);
                TileSize.Height = Int32.Parse(txtSizeY.Text);
                Tiles.Width = Int32.Parse(txtTilesX.Text);
                Tiles.Height = Int32.Parse(txtTilesY.Text);
                TileShape = lstTileShape.SelectedItem.ToString();
            }
            catch
            {
                MessageBox.Show("Sorry, but there is a problem with the numbers you entered.  Please correct them and try again.");
                return;
            }

            if (TileSize.Width < 10 || TileSize.Height < 10 || Tiles.Width < 3 || Tiles.Height < 3)
            {
                MessageBox.Show("Minimum 3 x 3 tiles of size 10 x 10 pixels.  Please correct them and try again.");
                return;
            }

            this.Close();
        }
    }
}