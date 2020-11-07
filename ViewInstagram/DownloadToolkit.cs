using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ControllerInstagram;
using InsstagramTool.ObjectData;
using ModelInstagram.DataRecive;

namespace ViewInstagram
{
    public partial class DownloadToolkit : UserControl
    {
        private Instagram instagram;
        public DownloadToolkit()
        {
            InitializeComponent();
        }

        public Instagram Instagram { get => instagram; set => instagram = value; }

        private async void bunifuButton1_Click(object sender, EventArgs e)
        {
            if (instagram == null)
                return;
            if (String.IsNullOrEmpty(instagram.Cookie))
                return;
            instagram.processLoading += Instagram_processLoading;
            instagram.successLoading += Instagram_successLoading;
            instagram.errorLoading += Instagram_errorLoading;
            String id = tbID.Text;
            if (String.IsNullOrEmpty(id))
                return;
            Int32 idint = 0;
            if(Int32.TryParse(id,out idint))
                ActionAsyn.RunDownloadFile(instagram, id, Error);
            if(id.StartsWith("https://www.instagram.com/") || !id.Contains("/p/"))
            {
                if (id.EndsWith("/"))
                    id = id.Remove(id.LastIndexOf("/"));
                UserInforRecive.User user = instagram.getAllInforUser(id);
                if (user != null)
                {
                    tbIDUser.Text = user.id;
                    tbUserName.Text = user.username;
                    lbPostCount.Text = user.edge_owner_to_timeline_media.count + "";
                    lbFollwing.Text = user.edge_follow.count+"";
                    lbFollower.Text = user.edge_followed_by.count +"";
                    picAvatar.ImageLocation = user.profile_pic_url_hd;
                    ActionAsyn.RunDownloadFile(instagram, user.id, Error);
                }
            }
            if (id.StartsWith("https://www.instagram.com/") || id.Contains("/p/"))
            {
                
            }
        }

        private void Instagram_errorLoading(object ex)
        {
            ControllerInstagram.Action.InvokeRun(richTextBox1, () =>
            {
                Exception exception = ex as Exception;
                richTextBox1.SelectionColor = Color.Red;
                richTextBox1.AppendText("Tải lỗi: " + exception.Message);
            });
        }

        private void Instagram_successLoading(object data)
        {
            ControllerInstagram.Action.InvokeRun(richTextBox1, () =>
            {
                richTextBox1.SelectionColor = Color.Green;
                richTextBox1.AppendText("Hoàn thành: " + data);
            });
            instagram.processLoading -= Instagram_processLoading;
            instagram.successLoading -= Instagram_successLoading;
            instagram.errorLoading -= Instagram_errorLoading;
        }

        private void Instagram_processLoading(object data, int process)
        {
            CustomResourcePostData dataPost = data as CustomResourcePostData;
            ControllerInstagram.Action.Download(dataPost, "C:/ABC/" + dataPost.id);
            ControllerInstagram.Action.InvokeRun(richTextBox1, () =>
            {
                richTextBox1.SelectionColor = Color.Teal;
                richTextBox1.AppendText($"Đã tải: {dataPost.id}\n");
            });
        }

        public void Error(Object ex)
        {
            Exception ex2 = ex as Exception;
            Notifi notifi = new Notifi(ex2.Message);
            notifi.ShowDialog();
        }
        

        private void bunifuButton2_Click(object sender, EventArgs e)
        {
            instagram.IsStop = true;
        }

        private void bunifuButton3_Click(object sender, EventArgs e)
        {
            String id = tbID.Text;
            if (String.IsNullOrEmpty(id))
                return;
            Int32 idint = 0;
            if (Int32.TryParse(id, out idint))
                return;
            if (id.StartsWith("https://www.instagram.com/") || !id.Contains("/p/"))
            {
                if (id.EndsWith("/"))
                    id = id.Remove(id.LastIndexOf("/"));
                UserInforRecive.User user = instagram.getAllInforUser(id);
                if (user != null)
                {
                    tbIDUser.Text = user.id;
                    tbUserName.Text = user.username;
                    lbPostCount.Text = user.edge_owner_to_timeline_media.count + "";
                    lbFollwing.Text = user.edge_follow.count + "";
                    lbFollower.Text = user.edge_followed_by.count + "";
                    picAvatar.ImageLocation = user.profile_pic_url_hd;
                }
            }
        }
    }
}
