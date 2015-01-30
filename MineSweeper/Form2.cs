using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MineSweeper
{
    public partial class FrmSettings : Form
    {
        public FrmSettings()
        {
            InitializeComponent();
        }

        private void FrmSettings_Load(object sender, EventArgs e)
        {
            FrmMain M = this.Owner as FrmMain;
            textBox1.Text = M.W.ToString();
            textBox2.Text = M.H.ToString();
            textBox3.Text = M.TotalMine.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int W = 0, H = 0, TotalMine = 0;
            bool OK = true;
            try
            {
                W = Convert.ToInt32(textBox1.Text);
                H = Convert.ToInt32(textBox2.Text);
                TotalMine = Convert.ToInt32(textBox3.Text);
            }
            catch
            {
                OK = false;
                MessageBox.Show("Invalid input!");
            }
            if (OK)
            {
                if (W < 5 || W > 80 || H < 5 || H > 80 )
                {
                    MessageBox.Show("Out of range! (Width and height should be 5~80)");
                }
                else if (TotalMine < 1 || TotalMine > W * H - 1)
                {
                    MessageBox.Show("Out of range! (Mines should be 1~" + (W * H - 1).ToString() + ")");
                }
                else
                {
                    FrmMain M = this.Owner as FrmMain;
                    M.W = W;
                    M.H = H;
                    M.TotalMine = TotalMine;
                    M.NewGame();
                    this.Dispose();
                }
            }
        }
    }
}
