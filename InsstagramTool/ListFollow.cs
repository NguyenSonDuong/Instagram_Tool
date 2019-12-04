using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using xNet;
using Newtonsoft.Json;
using System.Threading;
using System.IO;

namespace InsstagramTool
{
    public partial class ListFollow : Form
    {
        public int status = 0;
        public string cookie = "";
        public string query_hash = "";
        public string IDUser = "";
        public string data = "";
        public string next = "";
        public string IDFollow = "";
        public int count = 0;
        public UserFollow userFind;
        public List<UserFollow> follow;
        List<string> idList;
        public ListFollow(string cookie, string IDUser, string query_hash)
        {
            this.cookie = cookie;
            this.IDUser = IDUser;
            this.query_hash = query_hash;
            this.data = "{\"id\":\"" + IDUser + "\",\"include_reel\":true,\"fetch_mutual\":false,\"first\":24,\"after\":\"\"}";
            this.follow = new List<UserFollow>();
            this.idList = new List<string>();
            InitializeComponent();


        }

        private void ListFollow_Load(object sender, EventArgs e)
        {
            new DiChuyenForm(this, panel1);
            getUserFollow();
            if (follow.Count > 0)
            {

                foreach (UserFollow item in follow)
                {
                    idList.Add(item.username);
                }
                listBox1.DataSource = idList;
            }
            status = 10;
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



        public void getUserFollow()
        {
            try
            {
                HttpRequest http = new HttpRequest();
                http.Cookies = new CookieDictionary();
                if (string.IsNullOrEmpty(cookie))
                {
                    MessageBox.Show("Error: Cookie bị lỗi hoặc không có cookie\nvui lòng bỏ sung cookie");
                    return;
                }
                MainForm.AddCookie(http, this.cookie);
                while (true)
                {
                    string link = MainForm.uri + "query_hash=" + query_hash + "&variables=" + data;
                    string dataPost = http.Get(link).ToString();
                    ListFollowUser.Rootobject rootobject = JsonConvert.DeserializeObject<ListFollowUser.Rootobject>(dataPost);
                    if (rootobject.status.Equals("ok"))
                    {
                        foreach (ListFollowUser.Edge item in rootobject.data.user.edge_follow.edges)
                        {
                            follow.Add(new UserFollow(item.node.id, item.node.username, item.node.full_name, item.node.profile_pic_url));
                        }
                        if (rootobject.data.user.edge_follow.page_info.has_next_page)
                            this.data = "{\"id\":\"" + IDUser + "\",\"include_reel\":true,\"fetch_mutual\":false,\"first\":24,\"after\":\"" + rootobject.data.user.edge_follow.page_info.end_cursor + "\"}";
                        else
                            break;
                    }
                    else
                    {
                        MessageBox.Show("Error connet to instagram");
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void flowLayoutPanel1_Scroll(object sender, ScrollEventArgs e)
        {

        }

        private void ListFollow_Enter(object sender, EventArgs e)
        {

        }

        private void ListFollow_Activated(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void panel3_Click(object sender, EventArgs e)
        {
            if (idList.Count <= 0)
                return;
            listBox1.DataSource = idList.FindAll(x => x.Contains(textBox1.Text));
            flowLayoutPanel1.Controls.Clear();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (idList.Count <= 0)
                return;
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                string path = folderBrowserDialog1.SelectedPath;
                Thread t = new Thread(
                    () =>
                    {
                        count = 0;
                        DownLoadImage(path, userFind.username + "_", cookie, "", IDFollow);
                    });
                t.IsBackground = false;
                t.Start();
            }
        }

        public void DownLoadImage(string path, string name, string cookie, string after, string id)
        {
            string data = "{\"id\":\"" + id + "\",\"first\":24,\"after\":\"" + after + "\"}";
            string quety_hash = MainForm.query_hash["user_newfeed"].ToString();
            string link = MainForm.uri + "query_hash=" + quety_hash + "&variables=" + data;
            try
            {
                HttpRequest http = new HttpRequest();
                http.Cookies = new CookieDictionary();
                if (string.IsNullOrEmpty(cookie))
                    return;
                string json = http.Get(link).ToString();
                ImageOfUser.Rootobject root = JsonConvert.DeserializeObject<ImageOfUser.Rootobject>(json);
                foreach (ImageOfUser.Edge item in root.data.user.edge_owner_to_timeline_media.edges)
                {
                    if (item.node.__typename.Equals("GraphSidecar"))
                    {
                        foreach (ImageOfUser.Edge5 item2 in item.node.edge_sidecar_to_children.edges)
                        {
                            string src = "";
                            string end = ".jpg";
                            if (item2.node.is_video)
                            {
                                src = item2.node.video_url;
                                end = ".mp4";
                            }
                            else
                                src = item2.node.display_resources[2].src;
                            http.Get(src).ToFile(path + "/" + name + DateTime.Now.Ticks + end);
                        }
                    }
                    else
                    {
                        string src = "";
                        string end = ".jpg";
                        if (item.node.is_video)
                        {
                            src = item.node.video_url;
                            end = ".mp4";
                        }
                        else
                            src = item.node.display_resources[2].src;
                        http.Get(src).ToFile(path + "/" + name + DateTime.Now.Ticks + end);
                    }
                    count++;
                    Thread.Sleep(new Random().Next(500, 2000));
                    label5.Invoke(new MethodInvoker(
                        () =>
                        {
                            label5.Text = count + "/" + root.data.user.edge_owner_to_timeline_media.count;
                        }));
                    count++;
                }
                Thread.Sleep(new Random().Next(500, 2000));
                if (root.data.user.edge_owner_to_timeline_media.page_info.has_next_page)
                    DownLoadImage(path, name, cookie, root.data.user.edge_owner_to_timeline_media.page_info.end_cursor, id);
                else
                {
                    label5.Invoke(new MethodInvoker(
                        () =>
                        {
                            label5.Text = "Đã tải xong";
                        }));
                    return;
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                return;
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (idList.Count <= 0)
                return;
            Thread t = new Thread(
                () =>
                {
                    unFollow(userFind.ID, cookie);
                });
            t.IsBackground = false;
            t.Start();
        }
        public void unFollow(string id, string cookie)
        {
            try
            {
                HttpRequest http = new HttpRequest();
                http.Cookies = new CookieDictionary();
                if (string.IsNullOrEmpty(cookie))
                {
                    MessageBox.Show("Error: Cookie bị lỗi hoặc không có cookie\nvui lòng bỏ sung cookie");
                    return;
                }
                MainForm.AddCookie(http, this.cookie);
                http.AddHeader("x-csrftoken", "015oKylXREy1u467izcCFm54fX5nWVeK");
                http.AddHeader("x-ig-app-id", "936619743392459");
                http.AddHeader("x-ig-www-claim", "hmac.AR0PAxlT3VfrAFo0aTRTtKPdRxG9WhhgU3dPgM2Q_BTJG4OG");
                http.AddHeader("x-instagram-ajax", "da1b0f7a6e7c");
                string link = "https://www.instagram.com/web/friendships/" + id + "/unfollow/";
                string json = http.Post(link).ToString();
                if(json.Contains("\"status\": \"ok\""))
                {
                    MessageBox.Show("Đã hủy theo dõi thành công");
                }
                else
                {
                    MessageBox.Show("Lỗi không thể hủy theo dõi được:\nNguyên nhân có thể do Cookie\nVui lòng kiểm tra lại");
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi không thể hủy theo dõi được:\nNguyên nhân có thể do Cookie hoặc đường truyền internet\nVui lòng kiểm tra lại");
            }
        }
    }
}
class ListFollowUser
{

    public class Rootobject
    {
        public Data data { get; set; }
        public string status { get; set; }
    }

    public class Data
    {
        public User user { get; set; }
    }

    public class User
    {
        public Edge_Follow edge_follow { get; set; }
    }

    public class Edge_Follow
    {
        public int count { get; set; }
        public Page_Info page_info { get; set; }
        public Edge[] edges { get; set; }
    }

    public class Page_Info
    {
        public bool has_next_page { get; set; }
        public string end_cursor { get; set; }
    }

    public class Edge
    {
        public Node node { get; set; }
    }

    public class Node
    {
        public string id { get; set; }
        public string username { get; set; }
        public string full_name { get; set; }
        public string profile_pic_url { get; set; }
        public bool is_private { get; set; }
        public bool is_verified { get; set; }
        public bool followed_by_viewer { get; set; }
        public bool requested_by_viewer { get; set; }
        public Reel reel { get; set; }
    }

    public class Reel
    {
        public string id { get; set; }
        public int expiring_at { get; set; }
        public bool has_pride_media { get; set; }
        public int latest_reel_media { get; set; }
        public object seen { get; set; }
        public Owner owner { get; set; }
    }

    public class Owner
    {
        public string __typename { get; set; }
        public string id { get; set; }
        public string profile_pic_url { get; set; }
        public string username { get; set; }
    }

}
class ImageOfUser
{

    public class Rootobject
    {
        public Data data { get; set; }
        public string status { get; set; }
    }

    public class Data
    {
        public User user { get; set; }
    }

    public class User
    {
        public Edge_Owner_To_Timeline_Media edge_owner_to_timeline_media { get; set; }
    }

    public class Edge_Owner_To_Timeline_Media
    {
        public int count { get; set; }
        public Page_Info page_info { get; set; }
        public Edge[] edges { get; set; }
    }

    public class Page_Info
    {
        public bool has_next_page { get; set; }
        public string end_cursor { get; set; }
    }

    public class Edge
    {
        public Node node { get; set; }
    }

    public class Node
    {
        public string __typename { get; set; }
        public string id { get; set; }
        public Dimensions dimensions { get; set; }
        public string display_url { get; set; }
        public Display_Resources1[] display_resources { get; set; }
        public bool is_video { get; set; }
        public string video_url { get; set; }
        public string tracking_token { get; set; }
        public Edge_Media_To_Tagged_User edge_media_to_tagged_user { get; set; }
        public object accessibility_caption { get; set; }
        public Edge_Media_To_Caption edge_media_to_caption { get; set; }
        public string shortcode { get; set; }
        public Edge_Media_To_Comment edge_media_to_comment { get; set; }
        public Edge_Media_To_Sponsor_User edge_media_to_sponsor_user { get; set; }
        public bool comments_disabled { get; set; }
        public int taken_at_timestamp { get; set; }
        public Edge_Media_Preview_Like edge_media_preview_like { get; set; }
        public object gating_info { get; set; }
        public object fact_check_overall_rating { get; set; }
        public object fact_check_information { get; set; }
        public string media_preview { get; set; }
        public Owner1 owner { get; set; }
        public object location { get; set; }
        public bool viewer_has_liked { get; set; }
        public bool viewer_has_saved { get; set; }
        public bool viewer_has_saved_to_collection { get; set; }
        public bool viewer_in_photo_of_you { get; set; }
        public bool viewer_can_reshare { get; set; }
        public string thumbnail_src { get; set; }
        public Thumbnail_Resources[] thumbnail_resources { get; set; }
        public Edge_Sidecar_To_Children edge_sidecar_to_children { get; set; }
    }

    public class Dimensions
    {
        public int height { get; set; }
        public int width { get; set; }
    }

    public class Edge_Media_To_Tagged_User
    {
        public Edge1[] edges { get; set; }
    }

    public class Edge1
    {
        public Node1 node { get; set; }
    }

    public class Node1
    {
        public User1 user { get; set; }
        public float x { get; set; }
        public float y { get; set; }
    }

    public class User1
    {
        public string full_name { get; set; }
        public string id { get; set; }
        public bool is_verified { get; set; }
        public string profile_pic_url { get; set; }
        public string username { get; set; }
    }

    public class Edge_Media_To_Caption
    {
        public Edge2[] edges { get; set; }
    }

    public class Edge2
    {
        public Node2 node { get; set; }
    }

    public class Node2
    {
        public string text { get; set; }
    }

    public class Edge_Media_To_Comment
    {
        public int count { get; set; }
        public Page_Info1 page_info { get; set; }
        public Edge3[] edges { get; set; }
    }

    public class Page_Info1
    {
        public bool has_next_page { get; set; }
        public string end_cursor { get; set; }
    }

    public class Edge3
    {
        public Node3 node { get; set; }
    }

    public class Node3
    {
        public string id { get; set; }
        public string text { get; set; }
        public int created_at { get; set; }
        public bool did_report_as_spam { get; set; }
        public Owner owner { get; set; }
        public bool viewer_has_liked { get; set; }
    }

    public class Owner
    {
        public string id { get; set; }
        public bool is_verified { get; set; }
        public string profile_pic_url { get; set; }
        public string username { get; set; }
    }

    public class Edge_Media_To_Sponsor_User
    {
        public object[] edges { get; set; }
    }

    public class Edge_Media_Preview_Like
    {
        public int count { get; set; }
        public Edge4[] edges { get; set; }
    }

    public class Edge4
    {
        public Node4 node { get; set; }
    }

    public class Node4
    {
        public string id { get; set; }
        public string profile_pic_url { get; set; }
        public string username { get; set; }
    }

    public class Owner1
    {
        public string id { get; set; }
        public string username { get; set; }
    }

    public class Edge_Sidecar_To_Children
    {
        public Edge5[] edges { get; set; }
    }

    public class Edge5
    {
        public Node5 node { get; set; }
    }

    public class Node5
    {
        public string __typename { get; set; }
        public string id { get; set; }
        public Dimensions1 dimensions { get; set; }
        public string display_url { get; set; }
        public Display_Resources[] display_resources { get; set; }
        public bool is_video { get; set; }
        public string video_url { get; set; }
        public string tracking_token { get; set; }
        public Edge_Media_To_Tagged_User1 edge_media_to_tagged_user { get; set; }
        public object accessibility_caption { get; set; }
    }

    public class Dimensions1
    {
        public int height { get; set; }
        public int width { get; set; }
    }

    public class Edge_Media_To_Tagged_User1
    {
        public Edge6[] edges { get; set; }
    }

    public class Edge6
    {
        public Node6 node { get; set; }
    }

    public class Node6
    {
        public User2 user { get; set; }
        public float x { get; set; }
        public float y { get; set; }
    }

    public class User2
    {
        public string full_name { get; set; }
        public string id { get; set; }
        public bool is_verified { get; set; }
        public string profile_pic_url { get; set; }
        public string username { get; set; }
    }

    public class Display_Resources
    {
        public string src { get; set; }
        public int config_width { get; set; }
        public int config_height { get; set; }
    }

    public class Display_Resources1
    {
        public string src { get; set; }
        public int config_width { get; set; }
        public int config_height { get; set; }
    }

    public class Thumbnail_Resources
    {
        public string src { get; set; }
        public int config_width { get; set; }
        public int config_height { get; set; }
    }

}