using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using xNet;

namespace InsstagramTool
{
    public partial class MainForm : Form
    {
        public Dictionary<string, object> query_hash = new Dictionary<string, object>();
        public string uri = "https://www.instagram.com/graphql/query/?";
        public MainForm()
        {
            query_hash.Add("userInfor", "c9100bf9110dd6361671f113dd02e7d6");
            InitializeComponent();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            loadUser();
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
                        
                        try
                        {
                            HttpRequest http = new HttpRequest();
                            http.Cookies = new CookieDictionary();
                            AddCookie(http, cookie);
                            string myID = getMyIDFromCookie(cookie).Trim();
                            if (!string.IsNullOrEmpty(myID))
                            {
                                string dataPost = "{\"user_id\":\""+myID+"\",\"include_chaining\":true,\"include_reel\":true,\"include_suggested_users\":true,\"include_logged_out_extras\":false,\"include_highlight_reels\":false,\"include_related_profiles\":false}";
                                string getUserName = http.Get(uri+ "query_hash=" + query_hash["userInfor"] + "&variables=" + dataPost).ToString();
                                string userNameFormID = getUserNameFromID(getUserName).Trim();
                                string linkGetInfor = "https://www.instagram.com/"+userNameFormID+"/?__a=1";
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
                                    lbFollow.Text = "Follow: "+ root.graphql.user.edge_follow.count + "";
                                    lbFollower.Text = "Follower: " + root.graphql.user.edge_followed_by.count + "";
                                    lbID.Text = "ID: "+ myID;
                                    lbName.Text ="Tên: "+ root.graphql.user.full_name;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Errorr: Error connect to instagram ( Load data user )"+ ex.Message);
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
        static string getMyIDFromCookie(string cookie)
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
        static void AddCookie(HttpRequest http, string cookie)
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
            if(string.IsNullOrEmpty(tbCookie.Text))
                return;
            try
            {
                File.WriteAllText("cookie.ini", tbCookie.Text);
                loadUser();
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
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