using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using xNet;

namespace InsstagramTool
{
    public partial class FollowAllUser : Form
    {

        public string cookie;
        public int status = 0;
        public string query_hash = "";
        public string IDUser = "";
        public string data = "";
        public string next = "";
        public string IDFollow = "";
        public int count = 0;
        public UserFollow userFind;
        public List<UserFollow> follow;
        public List<string> idList;

        public FollowAllUser(string cookie)
        {
            this.cookie = cookie;
            follow = new List<UserFollow>();
            idList = new List<string>();
            InitializeComponent();
        }

        private void FollowAllUser_Load(object sender, EventArgs e)
        {
            new DiChuyenForm(this, panel1);
        }

        public bool Follow(string id, string cookie)
        {
            try
            {
                HttpRequest http = new HttpRequest();
                http.Cookies = new CookieDictionary();
                if (string.IsNullOrEmpty(cookie))
                {
                    MessageBox.Show("Error: Cookie bị lỗi hoặc không có cookie\nvui lòng bỏ sung cookie");
                    return false;
                }
                MainForm.AddCookie(http, cookie);
                http.AddHeader("x-csrftoken", ListFollow.getCsrftokenFromCookie(cookie));
                string link = "https://www.instagram.com/web/friendships/" + id + "/follow/";
                string json = http.Post(link).ToString();
                if (json.Contains("\"status\": \"ok\""))
                {
                    return true;
                }
                else
                {
                    return false;
                    MessageBox.Show("Lỗi không thể theo dõi được:\nNguyên nhân có thể do Cookie, Cookie bị block\nVui lòng kiểm tra lại");

                }

            }
            catch (Exception ex)
            {
                return false;
                MessageBox.Show("Lỗi không thể hủy theo dõi được:\nNguyên nhân có thể do Cookie hoặc đường truyền internet\nVui lòng kiểm tra lại");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog f = new OpenFileDialog();
                if (f.ShowDialog() == DialogResult.OK)
                {
                    string[] user = File.ReadAllLines(f.FileName);
                    foreach(string item in user)
                    {
                        string[] idAndUserName = item.Split('|');
                        UserFollow userFollow = new UserFollow(idAndUserName[0], idAndUserName[1], idAndUserName[2], idAndUserName[3]);
                        follow.Add(userFollow);
                    }

                    if (follow.Count > 0)
                    {

                        foreach (UserFollow item in follow)
                        {
                            idList.Add(item.username);
                        }
                        listBox1.DataSource = idList;
                    }
                }
                
            }
            catch (Exception ex)
            {

            }
        }
        public string getIDOfUser(ref String userName, string link, string cookie)
        {
            link = link.Trim();
            if (link.EndsWith("/"))
            {
                link += "?__a=1";
            }
            else
            {
                link += "/?__a=1";
            }
            try
            {
                HttpRequest http = new HttpRequest();
                http.Cookies = new CookieDictionary();
                if (!string.IsNullOrEmpty(cookie))
                    MainForm.AddCookie(http, cookie);
                string json = http.Get(link).ToString();
                InforUser.Rootobject root = JsonConvert.DeserializeObject<InforUser.Rootobject>(json);
                userName = root.graphql.user.username;
                return root.graphql.user.id;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            return "-1";
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (idList.Count <= 0)
                return;
            string selecString = listBox1.SelectedValue.ToString();
            flowLayoutPanel1.Controls.Clear();
            userFind = follow.Find(x => x.username.Equals(selecString));
            this.IDFollow = userFind.ID;
            this.next = getList("", IDFollow);
        }

        public string getList(string after, string id)
        {
            if (after.Equals("-1"))
                return "Error";
            string data = "{\"id\":\"" + id + "\",\"first\":24,\"after\":\"" + after + "\"}";
            string quety_hash = MainForm.query_hash["user_newfeed"].ToString();
            string link = MainForm.uri + "query_hash=" + quety_hash + "&variables=" + data;
            try
            {
                HttpRequest http = new HttpRequest();
                http.Cookies = new CookieDictionary();
                if (string.IsNullOrEmpty(cookie))
                    return "-1";
                string json = http.Get(link).ToString();
                ImageOfUser.Rootobject root = JsonConvert.DeserializeObject<ImageOfUser.Rootobject>(json);
                foreach (ImageOfUser.Edge item in root.data.user.edge_owner_to_timeline_media.edges)
                {
                    PictureBox pic = new PictureBox();
                    pic.Size = new Size(200, 200);
                    pic.Name = "GrapImage";
                    pic.BackColor = Color.Black;
                    pic.SizeMode = PictureBoxSizeMode.Zoom;
                    pic.TabIndex = 0;
                    pic.TabStop = false;
                    pic.ImageLocation = item.node.display_url;
                    flowLayoutPanel1.Controls.Add(pic);
                }
                if (root.data.user.edge_owner_to_timeline_media.page_info.has_next_page)
                    return root.data.user.edge_owner_to_timeline_media.page_info.end_cursor;
                else
                    return "-1";

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            return "-1";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (idList.Count <= 0)
                return;
            Thread t = new Thread(
                () =>
                {
                    if(Follow(userFind.ID, cookie))
                    {
                        MessageBox.Show("Follow thành công");
                    }
                });
            t.IsBackground = false;
            t.Start();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (idList.Count <= 0)
                return;
            progressBar1.Maximum = follow.Count;
            Thread t = new Thread(
                () =>
                {
                    int count = 0;
                    foreach(UserFollow item in follow)
                    {
                        bool re =  Follow(item.ID, cookie);
                        if (!re)
                            continue;
                        progressBar1.Invoke(new MethodInvoker(
                            ()=> {
                                progressBar1.PerformStep();
                                count++;
                            }));
                        Thread.Sleep(new Random().Next(1000, 3000));
                    }
                    
                });
            t.IsBackground = true;
            t.Start();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
