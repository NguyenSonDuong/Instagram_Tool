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

namespace InsstagramTool
{
    public partial class Save : Form
    {
        public Save()
        {
            InitializeComponent();
        }

        private void Save_Load(object sender, EventArgs e)
        {
            new DiChuyenForm(this, panel1);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText("path.ini", folderBrowserDialog1.SelectedPath);
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
