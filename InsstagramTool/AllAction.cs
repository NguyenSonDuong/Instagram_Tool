using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InsstagramTool.ObjectData;
using xNet;
using Newtonsoft.Json;
using System.IO;
using System.Diagnostics;

namespace InsstagramTool
{
    class AllAction
    {
        public static string USERNAME_INFOR = "c9100bf9110dd6361671f113dd02e7d6";
        public static string FOLLLOW = "d04b0a864b4b54837c0d870b0e77e076";
        public static string USER_NEWFEED = "2c5d4d8b70cad329c4a6ebe3abb6eedd";
        public static string POST_INFOR = "fead941d698dc1160a298ba7bec277ac";
        public static string URI = "https://www.instagram.com/";
        public static string PARAGRAP_URI = "graphql/query/";
        public static string PARAGRAP_URI_USERINFOR = "/?_a=1";
        public static string VARIABLES = "&variables=";
        public static string QUERY_HASH = "?query_hash=";

        /// <summary>
        /// Lấy ID của người dùng từ cookie
        /// </summary>
        /// <param name="cookie">Cookie của bạn</param>
        /// <returns>Chuỗi ID</returns>
        public static string getIDFromCookie(string cookie)
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

        /// <summary>
        /// Lấy CsrfToken của người dùng từ cookie
        /// </summary>
        /// <param name="cookie">Cookie của bạn</param>
        /// <returns>Chuỗi CsrfToken</returns>
        public static string getCsrfTokenFromCookie(string cookie)
        {
            var temp = cookie.Split(';');
            foreach (var item in temp)
            {
                var temp2 = item.Trim().Split('=');
                if (temp2.Count() > 1)
                {
                    if (temp2[0].Equals("csrftoken"))
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
        public static HttpRequest getConnect(string cookie)
        {
            if (string.IsNullOrEmpty(cookie))
            {
                HttpRequest http = new HttpRequest();
                return http;
            }
            else
            {
                HttpRequest http = new HttpRequest();
                http.Cookies = new CookieDictionary();
                AddCookie(http, cookie);
                return http;
            }
        }

        /// <summary>
        /// Lấy tên người dùng và id của người dùng từ cookie
        /// </summary>
        /// <param name="cookie">Cookie của bạn</param>
        /// <returns>Đối tượng của lớp User</returns>
        public static string getUserNameOfCookie(string cookie)
        {
            if (string.IsNullOrEmpty(cookie))
            {
                return null;
            }
            try
            {
                HttpRequest http = getConnect(cookie);
                string id = getIDFromCookie(cookie);
                string dataJsonSend = JsonConvert.SerializeObject(new DataSendToGetUserName.Data(id, true, true, true, true, true, true));
                string url = URI + QUERY_HASH + USERNAME_INFOR + VARIABLES + dataJsonSend;
                string jsonOutput = http.Get(url).ToString();
                DataUsernameOfUser.Rootobject root = JsonConvert.DeserializeObject<DataUsernameOfUser.Rootobject>(jsonOutput);
                if (root.status.Equals("ok"))
                {
                    DataUsernameOfUser.Reel data = root.data.user.reel;
                    return data.owner.username;
                }
                else
                {
                    throw new Exception("Error: Cookie");
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        /// <summary>
        /// Lấy thông tin chi tiết người dùng
        /// 
        /// </summary>
        /// <param name="userName">Tên người dùng muốn lấy</param>
        /// <param name="cookie">Cookie</param>
        /// <returns></returns>
        public static User getAllInforUser(string userName, string cookie)
        {
            if (string.IsNullOrEmpty(cookie))
            {
                return null;
            }
            try
            {
                HttpRequest http = new HttpRequest();
                http.Cookies = new CookieDictionary();
                AddCookie(http, cookie);
                string url = URI + userName + PARAGRAP_URI_USERINFOR;
                string jsonOutput = http.Get(url).ToString();
                DataInforUser.Rootobject root = JsonConvert.DeserializeObject<DataInforUser.Rootobject>(jsonOutput);
                DataInforUser.User tembUser = root.graphql.user;
                User user = new User();
                user.id = tembUser.id;
                user.username = tembUser.username;
                user.full_name = tembUser.full_name;
                user.profile_pic_url = tembUser.profile_pic_url_hd;
                user.following = tembUser.edge_follow.count;
                user.follow = tembUser.edge_followed_by.count;
                return user;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Lưu trữ dữ liệu dạng json
        /// </summary>
        /// <param name="path">Đường dẫn file trả ra</param>
        /// <param name="obj">Đối tượng đưa vào</param>
        public static void saveDataObject(string path,Object obj)
        {
            try
            {
                String a = JsonConvert.SerializeObject(obj);
                File.AppendAllText(path, a);
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Lấy danh sách các tài nguyên của toàn bộ bài viết thuộc
        /// về người dùng nào đó
        /// </summary>
        /// <param name="list">Danh sách sau khu trả ra ( List<DataPost> )</param>
        /// <param name="id">ID của người muốn tải</param>
        /// <param name="cookie">Cookie của bạn</param>
        /// <param name="after">Để rỗng nếu bạn muốn tải từ đầu đây là mã lấy các bài viết tiết theo</param>
        /// <param name="isStop">Trạng thái nếu là true sẽ tải nếu là false sẽ dừng tải
        ///  (sử dụng nó để dừng tải 1 tiến trình nào đó )</param>
        public static void GetListLinkResourch(List<DataPost> list, string id, string cookie, string after, bool isStop)
        {
            try
            {

                HttpRequest http = getConnect(cookie);
                string data = JsonConvert.SerializeObject(new DataSendToGetPostOfUser.Data(id, 15, after));
                string url = URI + PARAGRAP_URI + QUERY_HASH + USER_NEWFEED + VARIABLES + data;
                string json = http.Get(url).ToString();
                DataPostOfUser.Rootobject root = JsonConvert.DeserializeObject<DataPostOfUser.Rootobject>(json);

                foreach (DataPostOfUser.Edge item in root.data.user.edge_owner_to_timeline_media.edges)
                {
                    if (isStop)
                        return;
                    if (item.node.__typename.Equals("GraphSidecar"))
                    {
                        foreach (DataPostOfUser.Edge4 item2 in item.node.edge_sidecar_to_children.edges)
                        {
                            DataPost dataPost = new DataPost();
                            string src = "";
                            string end = ".jpg";
                            if (item2.node.is_video)
                            {
                                dataPost.is_video = true;
                                src = item2.node.video_url;
                                dataPost.video_url = src;
                                dataPost.display_desources = null;
                                end = ".mp4";
                            }
                            else
                            {
                                dataPost.is_video = false;
                                dataPost.video_url = "";
                                dataPost.display_desources = item2.node.display_resources;
                                src = item2.node.display_resources[2].src;
                            }
                            list.Add(dataPost);
                            
                        }
                    }
                    else
                    {
                        DataPost dataPost = new DataPost();
                        string src = "";
                        string end = ".jpg";
                        if (item.node.is_video)
                        {
                            dataPost.is_video = true;
                            src = item.node.video_url;
                            dataPost.video_url = src;
                            dataPost.display_desources = null;
                            end = ".mp4";
                        }
                        else
                        {
                            dataPost.is_video = false;
                            dataPost.video_url = "";
                            dataPost.display_desources = item.node.display_resources;
                            src = item.node.display_resources[2].src;
                        }
                        list.Add(dataPost);
                    }

                }
                if (root.data.user.edge_owner_to_timeline_media.page_info.has_next_page)
                    GetListLinkResourch( list, id, cookie,root.data.user.edge_owner_to_timeline_media.page_info.end_cursor,isStop);
            }
            catch(Exception ex)
            {
                throw new Exception("Error", ex);
            }
        }

        /// <summary>
        /// Tải toàn bộ bài viết của người dùng 
        /// bất kỳ về với người dùng để riêng tư phải có cookie
        /// </summary>
        /// <param name="id">ID của người dùng muốn tải</param>
        /// <param name="cookie">Cookie của bạn</param>
        /// <param name="after">Để rỗng nếu bạn muốn tải từ đầu đây là mã lấy các bài viết tiết theo</param>
        /// <param name="path">Đường dẫn thưc mục tải về</param>
        /// <param name="isStop">Trạng thái nếu là true sẽ tải nếu là false sẽ dừng tải
        ///  (sử dụng nó để dừng tải 1 tiến trình nào đó )</param>
        public static void DownloadPostOfProfileUser(string id,string cookie,string after,string path, bool isStop)
        {
            try
            {
                HttpRequest http = getConnect(cookie);
                string data = JsonConvert.SerializeObject(new DataSendToGetPostOfUser.Data(id,15,after));
                string url = URI + PARAGRAP_URI + QUERY_HASH + USER_NEWFEED + VARIABLES + data;
                string json = http.Get(url).ToString();
                DataPostOfUser.Rootobject root = JsonConvert.DeserializeObject<DataPostOfUser.Rootobject>(json);

                foreach (DataPostOfUser.Edge item in root.data.user.edge_owner_to_timeline_media.edges)
                {
                    if (isStop)
                        return;
                    if (item.node.__typename.Equals("GraphSidecar"))
                    {
                        foreach (DataPostOfUser.Edge4 item2 in item.node.edge_sidecar_to_children.edges)
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
                            http.Get(src).ToFile(path + "/" + DateTime.Now.Ticks + end);
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
                        http.Get(src).ToFile(path + "/" + DateTime.Now.Ticks + end);
                        
                    }
                    
                }
                if (root.data.user.edge_owner_to_timeline_media.page_info.has_next_page)
                    DownloadPostOfProfileUser(id, cookie, root.data.user.edge_owner_to_timeline_media.page_info.end_cursor,path,isStop);

            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }


        }

        /// <summary>
        /// Lấy thông tin của 1 bài viết nào đó thông qua shortcode
        /// </summary>
        /// <param name="path">Đường thử mục tải về</param>
        /// <param name="shortcode">shortcode của bài viết</param>
        /// <param name="cookie">Cookie của người dùng</param>
        public static void DownloadInforOfPost(string path, string shortcode, string cookie)
        {
            try
            {
                string data = JsonConvert.SerializeObject(new DataSendToGetInforPost(shortcode, 1, 1, 1, true));
                string link = URI + PARAGRAP_URI_USERINFOR + QUERY_HASH + POST_INFOR + VARIABLES + data;
                HttpRequest http = new HttpRequest();
                http.Cookies = new CookieDictionary();
                if (!string.IsNullOrEmpty(cookie))
                    AddCookie(http, cookie);
                string json = http.Get(link).ToString();
                PostData.Rootobject root = JsonConvert.DeserializeObject<PostData.Rootobject>(json);
                if (!root.status.Equals("ok"))
                {
                    throw new Exception("Lỗi cookie, hoặc người dùng không tồn tại\nvui lòng kiểm tra lại cookie");
                }

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
                        http.Get(src).ToFile(path+"/" + DateTime.Now.Ticks + end);
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
                    http.Get(src).ToFile(path + "/" + DateTime.Now.Ticks + end);
                }
                try
                {
                    Process.Start(path);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error", ex);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error",ex);
            }
        }

        /// <summary>
        /// Hủy theo dõi hoặc theo dõi người dùng
        /// </summary>
        /// <param name="id">ID của người dùng</param>
        /// <param name="isFollow">True nếu bạn muốn theo dõi và ngược lại</param>
        /// <param name="cookie">Cookie của bạn</param>
        /// <param name="status">Trạng thái thực liện: -1 thực hiện không thành công</param>
        public static void ActionFollow(string id,bool isFollow ,string cookie, ref int status)
        {
            try
            {
                String f = "/unfollow/";
                if (isFollow)
                    f = "/follow/";

                HttpRequest http = getConnect(cookie);
                http.AddHeader("x-csrftoken", getCsrfTokenFromCookie(cookie));
                string link = "https://www.instagram.com/web/friendships/" + id + f;
                string json = http.Post(link).ToString();
                if (json.Contains("\"status\": \"ok\""))
                {
                    status++;
                }
                else
                {
                    status = -1;
                    throw new Exception("Lỗi không thể hủy theo dõi được:\nNguyên nhân có thể do Cookie\nVui lòng kiểm tra lại");
                }

            }
            catch (Exception ex)
            {
                status = -1;
                throw new Exception("Error: ", ex);
            }
        }



    }
}



#region Dữ liệu được gửi đi
/// <summary>
/// Dữ liệu được gửi đi và nhận kết quả trả về là 
/// thông tin cá nhân của người dùng hiện tại
/// </summary>
public class DataSendToGetUserName
{
    public class Data
    {
        public string user_id { get; set; }
        public bool include_chaining { get; set; }
        public bool include_reel { get; set; }
        public bool include_suggested_users { get; set; }
        public bool include_logged_out_extras { get; set; }
        public bool include_highlight_reels { get; set; }
        public bool include_related_profiles { get; set; }

        public Data(string a, bool b, bool c, bool d, bool e, bool f, bool g)
        {
            user_id = a;
            include_chaining = b;
            include_reel = c;
            include_suggested_users = d;
            include_logged_out_extras = e;
            include_highlight_reels = f;
            include_related_profiles = g;
        }
    }
}

/// <summary>
/// Dữ liệu được gửi đi và nhận kết quả trả về là 
/// danh sách các bài viết của người dùng
/// </summary>
public class DataSendToGetPostOfUser
{
    public class Data
    {
        public string id { get; set; }
        public int first { get; set; }
        public string after { get; set; }

        public Data(string a, int b, string c)
        {
            id = a;
            first = b;
            after = c;
        }
    }
}

/// <summary>
/// Dữ liệu được gửi đi và nhận kết quả trả về là 
/// thông tin của 1 bài viết cụ thể
/// </summary>
public class DataSendToGetInforPost
{
        public string shortcode { get; set; }
        public int child_comment_count { get; set; }
        public int fetch_comment_count { get; set; }
        public int parent_comment_count { get; set; }
        public bool has_threaded_comments { get; set; }
        public DataSendToGetInforPost(string shortcode, int child_comment_count, int fetch_comment_count, int parent_comment_count, bool has_threaded_comments)
        {
            this.child_comment_count = child_comment_count;
            this.fetch_comment_count = fetch_comment_count;
            this.shortcode = shortcode;
            this.parent_comment_count = parent_comment_count;
            this.has_threaded_comments = has_threaded_comments;
        }

}
#endregion

#region Dữ liệu trả về
/// <summary>
/// Mục đích của class là để lấy tên người dùng (Username)
/// của người dùng hiện tại ( thông qua cookie )
/// </summary>
public class DataUsernameOfUser
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

}

/// <summary>
/// Mục đích của class là để lấy thông tin người dùng 
///  hiện tại ( thông qua cookie )
/// </summary>
public class DataInforUser
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
        public object felix_profile_grid_crop { get; set; }
        public int video_view_count { get; set; }
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

/// <summary>
/// Mục đích của class là để lấy dang sách các bài viết
/// của người dùng nào đó( thông qua cookie hoặc không)
/// </summary>
public class DataPostOfUser
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
        public Display_Resources[] display_resources { get; set; }
        public bool is_video { get; set; }
        public string video_url { get; set; }
        public string tracking_token { get; set; }
        public Edge_Media_To_Tagged_User edge_media_to_tagged_user { get; set; }
        public string accessibility_caption { get; set; }
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
        public object[] edges { get; set; }
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
        public Page_Info1 page_info { get; set; }
        public Edge2[] edges { get; set; }
    }

    public class Page_Info1
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
        public Edge3[] edges { get; set; }
    }

    public class Edge3
    {
        public Node3 node { get; set; }
    }

    public class Node3
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
        public Edge4[] edges { get; set; }
    }

    public class Edge4
    {
        public Node4 node { get; set; }
    }

    public class Node4
    {
        public string __typename { get; set; }
        public string id { get; set; }
        public Dimensions1 dimensions { get; set; }
        public string display_url { get; set; }
        public Display_Resources[] display_resources { get; set; }
        public bool is_video { get; set; }
        public string tracking_token { get; set; }
        public Edge_Media_To_Tagged_User1 edge_media_to_tagged_user { get; set; }
        public string accessibility_caption { get; set; }
        public Dash_Info dash_info { get; set; }
        public string video_url { get; set; }
        public int video_view_count { get; set; }
    }

    public class Dimensions1
    {
        public int height { get; set; }
        public int width { get; set; }
    }

    public class Edge_Media_To_Tagged_User1
    {
        public object[] edges { get; set; }
    }

    public class Dash_Info
    {
        public bool is_dash_eligible { get; set; }
        public string video_dash_manifest { get; set; }
        public int number_of_qualities { get; set; }
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

/// <summary>
/// Mục đích của class là để lấy thông tin
/// của bài viết nào đó( thông qua cookie hoặc không)
/// </summary>
public class DataInforPost
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
        public Display_Resources[] display_resources { get; set; }
        public bool is_video { get; set; }
        public string video_url { get; set; }
        public string tracking_token { get; set; }
        public Edge_Media_To_Tagged_User edge_media_to_tagged_user { get; set; }
        public Edge_Media_To_Caption edge_media_to_caption { get; set; }
        public bool caption_is_edited { get; set; }
        public bool has_ranked_comments { get; set; }
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
        public Owner owner { get; set; }
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
        public object[] edges { get; set; }
    }

    public class Edge_Media_To_Caption
    {
        public Edge[] edges { get; set; }
    }

    public class Edge
    {
        public Node node { get; set; }
    }

    public class Node
    {
        public string text { get; set; }
    }

    public class Edge_Media_To_Parent_Comment
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

    public class Edge_Media_Preview_Comment
    {
        public int count { get; set; }
        public object[] edges { get; set; }
    }

    public class Edge_Media_Preview_Like
    {
        public int count { get; set; }
        public object[] edges { get; set; }
    }

    public class Edge_Media_To_Sponsor_User
    {
        public object[] edges { get; set; }
    }

    public class Owner
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
        public Edge1[] edges { get; set; }
    }

    public class Edge1
    {
        public Node1 node { get; set; }
    }

    public class Node1
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
        public string tracking_token { get; set; }
        public Edge_Media_To_Tagged_User1 edge_media_to_tagged_user { get; set; }
        public Dash_Info dash_info { get; set; }
        public string video_url { get; set; }
        public int video_view_count { get; set; }
    }

    public class Dimensions1
    {
        public int height { get; set; }
        public int width { get; set; }
    }

    public class Edge_Media_To_Tagged_User1
    {
        public object[] edges { get; set; }
    }

    public class Dash_Info
    {
        public bool is_dash_eligible { get; set; }
        public string video_dash_manifest { get; set; }
        public int number_of_qualities { get; set; }
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

#endregion