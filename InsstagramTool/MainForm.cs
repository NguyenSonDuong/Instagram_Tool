using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using xNet;

namespace InsstagramTool
{
    public partial class MainForm : Form
    {
        #region
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        enum KeyModifier
        {
            None = 0,
            Alt = 1,
            Control = 2,
            Shift = 4,
            WinKey = 8
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == 0x0312)
            {
                Keys key = (Keys)(((int)m.LParam >> 16) & 0xFFFF);                  // The key of the hotkey that was pressed.
                KeyModifier modifier = (KeyModifier)((int)m.LParam & 0xFFFF);       // The modifier of the hotkey that was pressed.
                int id = m.WParam.ToInt32();
                // The id of the hotkey that was pressed.
                if (id == 1999)
                {
                    runDownloadPost();
                }
                else if (id == 2000)
                {
                    runDownloadUser();
                }else if (id == 2012)
                {

                }

            }
        }

        public void runDownloadPost()
        {
            try
            {
                string link = Clipboard.GetText();
                if (link.StartsWith("https://www.instagram.com/") || link.StartsWith("https://instagram.com/") || link.StartsWith("instagram.com/"))
                {
                    if (!File.Exists("path.ini"))
                        return;
                    string path = File.ReadAllText("path.ini");
                    if (string.IsNullOrEmpty(path))
                        return;
                    Thread t = new Thread(
                    () =>
                    {
                        string idm = link.Split('/')[4];
                        if (!Directory.Exists(path + "\\" + idm))
                            Directory.CreateDirectory(path + "\\" + idm);
                        DownloadImageOfPost(path + "\\" + idm, idm, cookie, idm);
                    });
                    t.IsBackground = false;
                    t.Start();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public int count = 0;
        public void runDownloadUser()
        {
            try
            {
                string link = Clipboard.GetText();
                if (link.StartsWith("https://www.instagram.com/") || link.StartsWith("https://instagram.com/") || link.StartsWith("instagram.com/"))
                {
                    if (!File.Exists("path.ini"))
                        return;

                    string path = File.ReadAllText("path.ini");

                    if (string.IsNullOrEmpty(path))
                        return;

                    Thread t = new Thread(
                    () =>
                    {
                        String name = "";
                        string idm = getIDOfUser(ref name, link, cookie);
                        if (!Directory.Exists(path + "\\" + name))
                            Directory.CreateDirectory(path + "\\" + name);
                        DownLoadImage(path + "\\" + name, idm, cookie, "", idm);
                    });
                    t.IsBackground = false;
                    t.Start();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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
                MainForm.AddCookie(http, cookie);
                string json = http.Get(link).ToString();
                ImageOfUser.Rootobject root = JsonConvert.DeserializeObject<ImageOfUser.Rootobject>(json);
                foreach (ImageOfUser.Edge item in root.data.user.edge_owner_to_timeline_media.edges)
                {
                    if (item.node.__typename.Equals("GraphSidecar"))
                    {
                        foreach(ImageOfUser.Edge5 item2 in item.node.edge_sidecar_to_children.edges)
                        {
                            string src = "";
                            string end = ".jpg";
                            if (item.node.is_video)
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

                    #region comment
                    //string src = "";
                    //string end = ".jpg";
                    //if (item.node.is_video)
                    //{
                    //    src = item.node.video_url;
                    //    end = ".mp4";
                    //}
                    //else
                    //    src = item.node.display_resources[2].src;
                    //http.Get(src).ToFile(path + "/" + name + DateTime.Now.Ticks + "_" + count + end);
                    //count++;
                    #endregion

                }
                Thread.Sleep(new Random().Next(500, 2000));
                if (root.data.user.edge_owner_to_timeline_media.page_info.has_next_page)
                    DownLoadImage(path, name, cookie, root.data.user.edge_owner_to_timeline_media.page_info.end_cursor, id);
                else
                {
                    MessageBox.Show("Đãm tải xong: " + count);
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                return;
            }
        }
        public string getIDOfUser(ref String userName,string link,string cookie)
        {
            link = link.Trim();
            if (link.EndsWith("/"))
            {
                link += "?__a=1";
            }else
            {
                link += "/?__a=1";
            }
            try
            {
                HttpRequest http = new HttpRequest();
                http.Cookies = new CookieDictionary();
                if(!string.IsNullOrEmpty(cookie))
                    AddCookie(http, cookie);
                string json = http.Get(link).ToString();
                InforUser.Rootobject root = JsonConvert.DeserializeObject<InforUser.Rootobject>(json);
                userName = root.graphql.user.username;
                return root.graphql.user.id;
            }catch(Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            return "-1";
        }

        #endregion

        public static Dictionary<string, object> query_hash = new Dictionary<string, object>();
        public string IDUser = "";
        public static string uri = "https://www.instagram.com/graphql/query/?";
        public string cookie = "";
        public MainForm()
        {
            query_hash.Add("userInfor", "c9100bf9110dd6361671f113dd02e7d6");
            query_hash.Add("follow", "d04b0a864b4b54837c0d870b0e77e076");
            query_hash.Add("user_newfeed", "2c5d4d8b70cad329c4a6ebe3abb6eedd");
            query_hash.Add("post", "fead941d698dc1160a298ba7bec277ac");
            InitializeComponent();

        }
        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

            loadUser();
            int id = 1999;     // The id of the hotkey. 
            int id2 = 2000;
            int id3 = 2012;
            RegisterHotKey(this.Handle, id, (int)KeyModifier.Shift, Keys.F7.GetHashCode());
            RegisterHotKey(this.Handle, id2, (int)KeyModifier.Shift, Keys.F8.GetHashCode());
            RegisterHotKey(this.Handle, id3, (int)KeyModifier.Shift, Keys.F12.GetHashCode());
            new DiChuyenForm(this, panel1);
        }
        public void loadUser()
        {
            if (!File.Exists("cookie.ini"))
            {
                lbID.Text = "";
                lbFollower.Text = "";
                lbFollow.Text = "";
                lbName.Text = "";
            }
            else
            {
                try
                {
                    String cookie = File.ReadAllText("cookie.ini");
                    if (!string.IsNullOrEmpty(cookie))
                    {
                        tbCookie.Text = cookie;
                        this.cookie = cookie;
                        try
                        {
                            HttpRequest http = new HttpRequest();
                            http.Cookies = new CookieDictionary();
                            AddCookie(http, cookie);
                            string myID = getMyIDFromCookie(cookie).Trim();
                            this.IDUser = myID;
                            if (!string.IsNullOrEmpty(myID))
                            {
                                string dataPost = "{\"user_id\":\"" + myID + "\",\"include_chaining\":true,\"include_reel\":true,\"include_suggested_users\":true,\"include_logged_out_extras\":false,\"include_highlight_reels\":false,\"include_related_profiles\":false}";
                                string getUserName = http.Get(uri + "query_hash=" + query_hash["userInfor"] + "&variables=" + dataPost).ToString();
                                string userNameFormID = getUserNameFromID(getUserName).Trim();
                                string linkGetInfor = "https://www.instagram.com/" + userNameFormID + "/?__a=1";
                                InforUser.Rootobject root = null;
                                try
                                {
                                    root = JsonConvert.DeserializeObject<InforUser.Rootobject>(http.Get(linkGetInfor).ToString());
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show("Errorr: Error cover json");
                                }

                                if (root != null)
                                {
                                    picAvatar.ImageLocation = root.graphql.user.profile_pic_url_hd;
                                    lbFollow.Text = "Follow: " + root.graphql.user.edge_follow.count + "";
                                    lbFollower.Text = "Follower: " + root.graphql.user.edge_followed_by.count + "";
                                    lbID.Text = "ID: " + myID;
                                    lbName.Text = "Tên: " + root.graphql.user.full_name;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Errorr: Error connect to instagram ( Load data user )" + ex.Message);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Error: Cookie null");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error:" + ex.Message);
                }

            }
        }
        public string getUserNameFromID(String data)
        {
            if (!string.IsNullOrEmpty(data))
            {
                DataGetInforUser dataGet = new DataGetInforUser();
                DataGetInforUser.Rootobject root = JsonConvert.DeserializeObject<DataGetInforUser.Rootobject>(data);
                if (root.status.Equals("ok"))
                {
                    return root.data.user.reel.user.username;
                }
            }
            return "";
        }

        public static string getMyIDFromCookie(string cookie)
        {
            var temp = cookie.Split(';');
            foreach (var item in temp)
            {
                var temp2 = item.Trim().Split('=');
                if (temp2.Count() > 1)
                {
                    if (temp2[0].Equals("ds_user_id"))
                    {
                        return temp2[1];
                    }
                }
            }
            return "";
        }
        public static void AddCookie(HttpRequest http, string cookie)
        {
            var temp = cookie.Split(';');
            foreach (var item in temp)
            {
                var temp2 = item.Trim().Split('=');
                if (temp2.Count() > 1)
                {
                    http.Cookies.Add(temp2[0], temp2[1]);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tbCookie.Text))
                return;
            try
            {
                File.WriteAllText("cookie.ini", tbCookie.Text);
                loadUser();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ListFollow li = new ListFollow(cookie, IDUser, query_hash["follow"].ToString());
            label5.Text = "Đang cập nhật dữ liệu";
            li.ShowDialog();
            label5.Text = "";

        }

        private void button3_Click(object sender, EventArgs e)
        {
            string link = textBox3.Text;
            if (!string.IsNullOrEmpty(link))
            {
                if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                {
                    new Thread(
                    () =>
                        {
                            if (link.StartsWith("https://www.instagram.com/") || link.StartsWith("https://instagram.com/"))
                            {
                                string id = link.Split('/')[4];

                                DownloadImageOfPost(folderBrowserDialog1.SelectedPath, id, cookie, id);

                            }
                        }).Start();
                }
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Visible = false;
        }
        public void DownloadImageOfPost(string path, string name, string cookie, string id)
        {
            string data = "{\"shortcode\":\"" + id + "\",\"child_comment_count\":3,\"fetch_comment_count\":40,\"parent_comment_count\":24,\"has_threaded_comments\":true}";
            string link = uri + "query_hash=" + query_hash["post"] + "&variables=" + data;
            try
            {
                HttpRequest http = new HttpRequest();
                http.Cookies = new CookieDictionary();
                AddCookie(http, cookie);
                string json = http.Get(link).ToString();
                PostData.Rootobject root = JsonConvert.DeserializeObject<PostData.Rootobject>(json);
                if (!root.status.Equals("ok"))
                    return;
                if (root.data.shortcode_media.__typename.Equals("GraphSidecar"))
                {
                    foreach (PostData.Edge6 item in root.data.shortcode_media.edge_sidecar_to_children.edges)
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
                }
                else
                {
                    PostData.Shortcode_Media item = root.data.shortcode_media;
                    string src = "";
                    string end = ".jpg";
                    if (item.is_video)
                    {
                        src = item.video_url;
                        end = ".mp4";
                    }
                    else
                        src = item.display_resources[2].src;
                    http.Get(src).ToFile(path + "/" + name + DateTime.Now.Ticks + end);
                }
                MessageBox.Show("Đã tải xong");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            UnregisterHotKey(this.Handle, 1999);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            new Save().ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            this.Visible = true;
        }

        private void mởPhầnMềmToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Visible = true;
        }

        private void đóngPhầnMềmToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
class DataGetInforUser
{

    public class Rootobject
    {
        public Data data { get; set; }
        public string status { get; set; }
    }

    public class Data
    {
        public Viewer viewer { get; set; }
        public User user { get; set; }
    }

    public class Viewer
    {
        public Edge_Suggested_Users edge_suggested_users { get; set; }
    }

    public class Edge_Suggested_Users
    {
        public int count { get; set; }
    }

    public class User
    {
        public Reel reel { get; set; }
        public Edge_Chaining edge_chaining { get; set; }
    }

    public class Reel
    {
        public string __typename { get; set; }
        public string id { get; set; }
        public int expiring_at { get; set; }
        public bool has_pride_media { get; set; }
        public int latest_reel_media { get; set; }
        public object seen { get; set; }
        public User1 user { get; set; }
        public Owner owner { get; set; }
    }

    public class User1
    {
        public string id { get; set; }
        public string profile_pic_url { get; set; }
        public string username { get; set; }
    }

    public class Owner
    {
        public string __typename { get; set; }
        public string id { get; set; }
        public string profile_pic_url { get; set; }
        public string username { get; set; }
    }

    public class Edge_Chaining
    {
        public Edge[] edges { get; set; }
    }

    public class Edge
    {
        public Node node { get; set; }
    }

    public class Node
    {
        public string id { get; set; }
        public bool blocked_by_viewer { get; set; }
        public bool followed_by_viewer { get; set; }
        public bool follows_viewer { get; set; }
        public string full_name { get; set; }
        public bool has_blocked_viewer { get; set; }
        public bool has_requested_viewer { get; set; }
        public bool is_private { get; set; }
        public bool is_verified { get; set; }
        public string profile_pic_url { get; set; }
        public bool requested_by_viewer { get; set; }
        public string username { get; set; }
    }

}

class InforUser
{

    public class Rootobject
    {
        public string logging_page_id { get; set; }
        public bool show_suggested_profiles { get; set; }
        public bool show_follow_dialog { get; set; }
        public Graphql graphql { get; set; }
        public object toast_content_on_load { get; set; }
    }

    public class Graphql
    {
        public User user { get; set; }
    }

    public class User
    {
        public string biography { get; set; }
        public bool blocked_by_viewer { get; set; }
        public bool country_block { get; set; }
        public object external_url { get; set; }
        public object external_url_linkshimmed { get; set; }
        public Edge_Followed_By edge_followed_by { get; set; }
        public bool followed_by_viewer { get; set; }
        public Edge_Follow edge_follow { get; set; }
        public bool follows_viewer { get; set; }
        public string full_name { get; set; }
        public bool has_channel { get; set; }
        public bool has_blocked_viewer { get; set; }
        public int highlight_reel_count { get; set; }
        public bool has_requested_viewer { get; set; }
        public string id { get; set; }
        public bool is_business_account { get; set; }
        public bool is_joined_recently { get; set; }
        public object business_category_name { get; set; }
        public bool is_private { get; set; }
        public bool is_verified { get; set; }
        public Edge_Mutual_Followed_By edge_mutual_followed_by { get; set; }
        public string profile_pic_url { get; set; }
        public string profile_pic_url_hd { get; set; }
        public bool requested_by_viewer { get; set; }
        public string username { get; set; }
        public object connected_fb_page { get; set; }
        public Edge_Felix_Combined_Post_Uploads edge_felix_combined_post_uploads { get; set; }
        public Edge_Felix_Combined_Draft_Uploads edge_felix_combined_draft_uploads { get; set; }
        public Edge_Felix_Video_Timeline edge_felix_video_timeline { get; set; }
        public Edge_Felix_Drafts edge_felix_drafts { get; set; }
        public Edge_Felix_Pending_Post_Uploads edge_felix_pending_post_uploads { get; set; }
        public Edge_Felix_Pending_Draft_Uploads edge_felix_pending_draft_uploads { get; set; }
        public Edge_Owner_To_Timeline_Media edge_owner_to_timeline_media { get; set; }
        public Edge_Saved_Media edge_saved_media { get; set; }
        public Edge_Media_Collections edge_media_collections { get; set; }
    }

    public class Edge_Followed_By
    {
        public int count { get; set; }
    }

    public class Edge_Follow
    {
        public int count { get; set; }
    }

    public class Edge_Mutual_Followed_By
    {
        public int count { get; set; }
        public object[] edges { get; set; }
    }

    public class Edge_Felix_Combined_Post_Uploads
    {
        public int count { get; set; }
        public Page_Info page_info { get; set; }
        public object[] edges { get; set; }
    }

    public class Page_Info
    {
        public bool has_next_page { get; set; }
        public object end_cursor { get; set; }
    }

    public class Edge_Felix_Combined_Draft_Uploads
    {
        public int count { get; set; }
        public Page_Info1 page_info { get; set; }
        public object[] edges { get; set; }
    }

    public class Page_Info1
    {
        public bool has_next_page { get; set; }
        public object end_cursor { get; set; }
    }

    public class Edge_Felix_Video_Timeline
    {
        public int count { get; set; }
        public Page_Info2 page_info { get; set; }
        public object[] edges { get; set; }
    }

    public class Page_Info2
    {
        public bool has_next_page { get; set; }
        public object end_cursor { get; set; }
    }

    public class Edge_Felix_Drafts
    {
        public int count { get; set; }
        public Page_Info3 page_info { get; set; }
        public object[] edges { get; set; }
    }

    public class Page_Info3
    {
        public bool has_next_page { get; set; }
        public object end_cursor { get; set; }
    }

    public class Edge_Felix_Pending_Post_Uploads
    {
        public int count { get; set; }
        public Page_Info4 page_info { get; set; }
        public object[] edges { get; set; }
    }

    public class Page_Info4
    {
        public bool has_next_page { get; set; }
        public object end_cursor { get; set; }
    }

    public class Edge_Felix_Pending_Draft_Uploads
    {
        public int count { get; set; }
        public Page_Info5 page_info { get; set; }
        public object[] edges { get; set; }
    }

    public class Page_Info5
    {
        public bool has_next_page { get; set; }
        public object end_cursor { get; set; }
    }

    public class Edge_Owner_To_Timeline_Media
    {
        public int count { get; set; }
        public Page_Info6 page_info { get; set; }
        public Edge[] edges { get; set; }
    }

    public class Page_Info6
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
        public Edge_Media_To_Caption edge_media_to_caption { get; set; }
        public string shortcode { get; set; }
        public Edge_Media_To_Comment edge_media_to_comment { get; set; }
        public bool comments_disabled { get; set; }
        public int taken_at_timestamp { get; set; }
        public Dimensions dimensions { get; set; }
        public string display_url { get; set; }
        public Edge_Liked_By edge_liked_by { get; set; }
        public Edge_Media_Preview_Like edge_media_preview_like { get; set; }
        public object location { get; set; }
        public object gating_info { get; set; }
        public object fact_check_overall_rating { get; set; }
        public object fact_check_information { get; set; }
        public string media_preview { get; set; }
        public Owner owner { get; set; }
        public string thumbnail_src { get; set; }
        public Thumbnail_Resources[] thumbnail_resources { get; set; }
        public bool is_video { get; set; }
        public string accessibility_caption { get; set; }
    }

    public class Edge_Media_To_Caption
    {
        public Edge1[] edges { get; set; }
    }

    public class Edge1
    {
        public Node1 node { get; set; }
    }

    public class Node1
    {
        public string text { get; set; }
    }

    public class Edge_Media_To_Comment
    {
        public int count { get; set; }
    }

    public class Dimensions
    {
        public int height { get; set; }
        public int width { get; set; }
    }

    public class Edge_Liked_By
    {
        public int count { get; set; }
    }

    public class Edge_Media_Preview_Like
    {
        public int count { get; set; }
    }

    public class Owner
    {
        public string id { get; set; }
        public string username { get; set; }
    }

    public class Thumbnail_Resources
    {
        public string src { get; set; }
        public int config_width { get; set; }
        public int config_height { get; set; }
    }

    public class Edge_Saved_Media
    {
        public int count { get; set; }
        public Page_Info7 page_info { get; set; }
        public Edge2[] edges { get; set; }
    }

    public class Page_Info7
    {
        public bool has_next_page { get; set; }
        public string end_cursor { get; set; }
    }

    public class Edge2
    {
        public Node2 node { get; set; }
    }

    public class Node2
    {
        public string __typename { get; set; }
        public string id { get; set; }
        public Edge_Media_To_Caption1 edge_media_to_caption { get; set; }
        public string shortcode { get; set; }
        public Edge_Media_To_Comment1 edge_media_to_comment { get; set; }
        public bool comments_disabled { get; set; }
        public int taken_at_timestamp { get; set; }
        public Dimensions1 dimensions { get; set; }
        public string display_url { get; set; }
        public Edge_Liked_By1 edge_liked_by { get; set; }
        public Edge_Media_Preview_Like1 edge_media_preview_like { get; set; }
        public object location { get; set; }
        public object gating_info { get; set; }
        public object fact_check_overall_rating { get; set; }
        public object fact_check_information { get; set; }
        public string media_preview { get; set; }
        public Owner1 owner { get; set; }
        public string thumbnail_src { get; set; }
        public Thumbnail_Resources1[] thumbnail_resources { get; set; }
        public bool is_video { get; set; }
        public string accessibility_caption { get; set; }
    }

    public class Edge_Media_To_Caption1
    {
        public Edge3[] edges { get; set; }
    }

    public class Edge3
    {
        public Node3 node { get; set; }
    }

    public class Node3
    {
        public string text { get; set; }
    }

    public class Edge_Media_To_Comment1
    {
        public int count { get; set; }
    }

    public class Dimensions1
    {
        public int height { get; set; }
        public int width { get; set; }
    }

    public class Edge_Liked_By1
    {
        public int count { get; set; }
    }

    public class Edge_Media_Preview_Like1
    {
        public int count { get; set; }
    }

    public class Owner1
    {
        public string id { get; set; }
        public string username { get; set; }
    }

    public class Thumbnail_Resources1
    {
        public string src { get; set; }
        public int config_width { get; set; }
        public int config_height { get; set; }
    }

    public class Edge_Media_Collections
    {
        public int count { get; set; }
        public Page_Info8 page_info { get; set; }
        public object[] edges { get; set; }
    }

    public class Page_Info8
    {
        public bool has_next_page { get; set; }
        public object end_cursor { get; set; }
    }

}

class DataFollow
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

class PostData
{
    public class Rootobject
    {
        public Data data { get; set; }
        public string status { get; set; }
    }

    public class Data
    {
        public Shortcode_Media shortcode_media { get; set; }
    }

    public class Shortcode_Media
    {
        public string __typename { get; set; }
        public string id { get; set; }
        public string shortcode { get; set; }
        public Dimensions dimensions { get; set; }
        public object gating_info { get; set; }
        public object fact_check_overall_rating { get; set; }
        public object fact_check_information { get; set; }
        public object media_preview { get; set; }
        public string display_url { get; set; }
        public Display_Resources1[] display_resources { get; set; }
        public bool is_video { get; set; }
        public string tracking_token { get; set; }
        public Edge_Media_To_Tagged_User edge_media_to_tagged_user { get; set; }
        public Edge_Media_To_Caption edge_media_to_caption { get; set; }
        public bool caption_is_edited { get; set; }
        public bool has_ranked_comments { get; set; }
        public string video_url { get; set; }
        public Edge_Media_To_Parent_Comment edge_media_to_parent_comment { get; set; }
        public Edge_Media_Preview_Comment edge_media_preview_comment { get; set; }
        public bool comments_disabled { get; set; }
        public int taken_at_timestamp { get; set; }
        public Edge_Media_Preview_Like edge_media_preview_like { get; set; }
        public Edge_Media_To_Sponsor_User edge_media_to_sponsor_user { get; set; }
        public object location { get; set; }
        public bool viewer_has_liked { get; set; }
        public bool viewer_has_saved { get; set; }
        public bool viewer_has_saved_to_collection { get; set; }
        public bool viewer_in_photo_of_you { get; set; }
        public bool viewer_can_reshare { get; set; }
        public Owner3 owner { get; set; }
        public bool is_ad { get; set; }
        public Edge_Web_Media_To_Related_Media edge_web_media_to_related_media { get; set; }
        public Edge_Sidecar_To_Children edge_sidecar_to_children { get; set; }
    }

    public class Dimensions
    {
        public int height { get; set; }
        public int width { get; set; }
    }

    public class Edge_Media_To_Tagged_User
    {
        public Edge[] edges { get; set; }
    }

    public class Edge
    {
        public Node node { get; set; }
    }

    public class Node
    {
        public User user { get; set; }
        public float x { get; set; }
        public float y { get; set; }
    }

    public class User
    {
        public string full_name { get; set; }
        public string id { get; set; }
        public bool is_verified { get; set; }
        public string profile_pic_url { get; set; }
        public string username { get; set; }
    }

    public class Edge_Media_To_Caption
    {
        public Edge1[] edges { get; set; }
    }

    public class Edge1
    {
        public Node1 node { get; set; }
    }

    public class Node1
    {
        public string text { get; set; }
    }

    public class Edge_Media_To_Parent_Comment
    {
        public int count { get; set; }
        public Page_Info page_info { get; set; }
        public Edge2[] edges { get; set; }
    }

    public class Page_Info
    {
        public bool has_next_page { get; set; }
        public string end_cursor { get; set; }
    }

    public class Edge2
    {
        public Node2 node { get; set; }
    }

    public class Node2
    {
        public string id { get; set; }
        public string text { get; set; }
        public int created_at { get; set; }
        public bool did_report_as_spam { get; set; }
        public Owner owner { get; set; }
        public bool viewer_has_liked { get; set; }
        public Edge_Liked_By edge_liked_by { get; set; }
        public Edge_Threaded_Comments edge_threaded_comments { get; set; }
    }

    public class Owner
    {
        public string id { get; set; }
        public bool is_verified { get; set; }
        public string profile_pic_url { get; set; }
        public string username { get; set; }
    }

    public class Edge_Liked_By
    {
        public int count { get; set; }
    }

    public class Edge_Threaded_Comments
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
        public Owner1 owner { get; set; }
        public bool viewer_has_liked { get; set; }
        public Edge_Liked_By1 edge_liked_by { get; set; }
    }

    public class Owner1
    {
        public string id { get; set; }
        public bool is_verified { get; set; }
        public string profile_pic_url { get; set; }
        public string username { get; set; }
    }

    public class Edge_Liked_By1
    {
        public int count { get; set; }
    }

    public class Edge_Media_Preview_Comment
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
        public string text { get; set; }
        public int created_at { get; set; }
        public bool did_report_as_spam { get; set; }
        public Owner2 owner { get; set; }
        public bool viewer_has_liked { get; set; }
        public Edge_Liked_By2 edge_liked_by { get; set; }
    }

    public class Owner2
    {
        public string id { get; set; }
        public bool is_verified { get; set; }
        public string profile_pic_url { get; set; }
        public string username { get; set; }
    }

    public class Edge_Liked_By2
    {
        public int count { get; set; }
    }

    public class Edge_Media_Preview_Like
    {
        public int count { get; set; }
        public Edge5[] edges { get; set; }
    }

    public class Edge5
    {
        public Node5 node { get; set; }
    }

    public class Node5
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

    public class Owner3
    {
        public string id { get; set; }
        public bool is_verified { get; set; }
        public string profile_pic_url { get; set; }
        public string username { get; set; }
        public bool blocked_by_viewer { get; set; }
        public bool followed_by_viewer { get; set; }
        public string full_name { get; set; }
        public bool has_blocked_viewer { get; set; }
        public bool is_private { get; set; }
        public bool is_unpublished { get; set; }
        public bool requested_by_viewer { get; set; }
    }

    public class Edge_Web_Media_To_Related_Media
    {
        public object[] edges { get; set; }
    }

    public class Edge_Sidecar_To_Children
    {
        public Edge6[] edges { get; set; }
    }

    public class Edge6
    {
        public Node6 node { get; set; }
    }

    public class Node6
    {
        public string __typename { get; set; }
        public string id { get; set; }
        public string shortcode { get; set; }
        public Dimensions1 dimensions { get; set; }
        public object gating_info { get; set; }
        public object fact_check_overall_rating { get; set; }
        public object fact_check_information { get; set; }
        public string media_preview { get; set; }
        public string display_url { get; set; }
        public Display_Resources[] display_resources { get; set; }
        public string accessibility_caption { get; set; }
        public bool is_video { get; set; }
        public string video_url { get; set; }
        public string tracking_token { get; set; }
        public Edge_Media_To_Tagged_User1 edge_media_to_tagged_user { get; set; }
    }

    public class Dimensions1
    {
        public int height { get; set; }
        public int width { get; set; }
    }

    public class Edge_Media_To_Tagged_User1
    {
        public Edge7[] edges { get; set; }
    }

    public class Edge7
    {
        public Node7 node { get; set; }
    }

    public class Node7
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


}

