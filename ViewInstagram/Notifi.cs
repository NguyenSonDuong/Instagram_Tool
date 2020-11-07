using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ViewInstagram
{
    public delegate void NotificationActionYes();
    public delegate void NotificationActionNo();
    public partial class Notifi : Form
    {
        private String mess;

        private event NotificationActionYes actionYes;
        private event NotificationActionNo actionNo;

        public event NotificationActionYes ActionYes
        {
            add { this.actionYes += value; }
            remove { this.actionYes -= value; }
        }
        public event NotificationActionNo ActionNo
        {
            add { this.actionNo += value; }
            remove { this.actionNo -= value; }
        }
        public Notifi()
        {
            InitializeComponent();
        }
        public Notifi(String mess)
        {
            InitializeComponent();
            this.mess = mess;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        private void btnNo_Click(object sender, EventArgs e)
        {
            if(actionNo != null)
                actionNo();
            else
                this.Close();
        }

        private void btnYes_Click(object sender, EventArgs e)
        {
            if (actionYes != null)
                actionYes();
            else
                this.Close();
        }


        private void Notifi_Load(object sender, EventArgs e)
        {
            lbMessage.Text = mess;
        }
    }
}
