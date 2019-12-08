using Microsoft.Win32;
using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace InsstagramTool
{
    public partial class Save : Form
    {
        RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
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

        private void button3_Click(object sender, EventArgs e)
        {
            MainForm.CreateShortcut("My InstagramTool", Environment.GetFolderPath(Environment.SpecialFolder.Desktop), Assembly.GetExecutingAssembly().Location);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                rkApp.SetValue("InstagramTool", Application.ExecutablePath);
            }
            else
            {
                rkApp.DeleteValue("InstagramTool", false);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
