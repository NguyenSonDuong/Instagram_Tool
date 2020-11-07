using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ControllerInstagram;
namespace ViewInstagram
{
    public partial class Form1 : Form
    {
        private Instagram instagram;
        public Instagram Instagram1 { get => instagram; set => instagram = value; }

        public Form1()
        {
            InitializeComponent();
            instagram = new Instagram();
        }

        private async void bunifuButton4_Click(object sender, EventArgs e)
        {
            instagram.Cookie = tbCookie.Text;
            await ActionAsyn.RunInit(instagram, Error);
            if (String.IsNullOrEmpty(instagram.Cookie))
                this.downloadToolkit1.Instagram = instagram;
            if (String.IsNullOrEmpty(instagram.Id))
                return;
            tbIDUser.Text = instagram.Id;
            if (String.IsNullOrEmpty(instagram.Username))
                return;
            tbUserName.Text = instagram.Username;
            if (instagram.UserInfor == null)
                return;
            lbPostCount.Text = instagram.UserInfor.edge_owner_to_timeline_media.count + "";
            lbFollower.Text = instagram.UserInfor.edge_followed_by.count + "";
            lbFollwing.Text = instagram.UserInfor.edge_follow.count + "";
            picAvatar.ImageLocation = instagram.UserInfor.profile_pic_url_hd;
            this.downloadToolkit1.Instagram = instagram;
            ActionAsyn.RunSaveInfor(instagram, Error);
        }

        public void Error(Object ex)
        {
            Exception ex2 = ex as Exception;
            Notifi notifi = new Notifi(ex2.Message);
            notifi.ShowDialog();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                await ActionAsyn.RunReadInfor(instagram,Error);
                await ActionAsyn.RunInit(instagram, Error);
                
                if (String.IsNullOrEmpty(instagram.Id))
                    return;
                tbIDUser.Text = instagram.Id;
                if (String.IsNullOrEmpty(instagram.Username))
                    return;
                tbUserName.Text = instagram.Username;
                if (instagram.UserInfor == null)
                    return;
                if (String.IsNullOrEmpty(instagram.Cookie))
                    return;
                lbPostCount.Text = instagram.UserInfor.edge_owner_to_timeline_media.count+"";
                lbFollower.Text = instagram.UserInfor.edge_followed_by.count+"";
                lbFollwing.Text = instagram.UserInfor.edge_follow.count + "";
                tbCookie.Text = instagram.Cookie;
                picAvatar.ImageLocation = instagram.UserInfor.profile_pic_url_hd;
                this.downloadToolkit1.Instagram = instagram;

            }
            catch(Exception ex)
            {
                Notifi notifi = new Notifi("Thông tin người dùng chưa được cập nhật vui lòng cập nhật lại cookie");
                notifi.ShowDialog();
            }
        }

        private void bunifuButton1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void bunifuButton2_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void downloadToolkit1_Load(object sender, EventArgs e)
        {

        }
    }
}
