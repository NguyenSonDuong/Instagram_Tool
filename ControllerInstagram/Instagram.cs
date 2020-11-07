using ModelInstagram.DataSend;
using ModelInstagram.DataRecive;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using ModelInstagram;
using InsstagramTool.ObjectData;

namespace ControllerInstagram
{
    public delegate void ProcessLoading(Object data, int process);
    public delegate void ErrorLoading(Object ex);
    public delegate void SuccessLoading(Object data);
    public class Instagram
    {
        #region Khai báo hành số
        public static string USERNAME_INFOR = "d4d88dc1500312af6f937f7b804c68c3";
        public static string FOLLLOW = "d04b0a864b4b54837c0d870b0e77e076";
        public static string USER_NEWFEED = "2c5d4d8b70cad329c4a6ebe3abb6eedd";
        public static string POST_INFOR = "fead941d698dc1160a298ba7bec277ac";
        public static string URI = "https://www.instagram.com/";
        public static string PARAGRAP_URI = "graphql/query/";
        public static string PARAGRAP_URI_USERINFOR = "/?__a=1";
        public static string VARIABLES = "&variables=";
        public static string QUERY_HASH = "?query_hash=";
        #endregion

        #region khai báo thuộc tính lớp
        private String cookie;
        private String id;
        private String username;
        private String CsrfToken;
        private UserInforRecive.User userInfor;
        private int proc = 0;
        private bool isStop = false;
        #endregion

        #region Khai báo sự kiện
        private event ProcessLoading process;
        private event ErrorLoading error;
        private event SuccessLoading success;
        #endregion

        #region Getter và Setter cho thuộc tính
        public string Cookie { get => cookie; set => cookie = value; }
        public string Id { get => id; set => id = value; }
        public string CsrfToken1 { get => CsrfToken; set => CsrfToken = value; }
        public UserInforRecive.User UserInfor { get => userInfor; set => userInfor = value; }
        public string Username { get => username; set => username = value; }
        public bool IsStop { get => isStop; set => isStop = value; }


        #endregion

        #region Getter Setter cho sự kiện
        public event ProcessLoading processLoading
        {
            add { this.process += value; }
            remove { this.process -= value; }
        }
        public event ErrorLoading errorLoading
        {
            add { this.error += value; }
            remove { this.error -= value; }
        }
        public event SuccessLoading successLoading
        {
            add { this.success += value; }
            remove { this.success -= value; }
        }
        #endregion
        public string getIDFromCookie(string cookie)
        {
            var temp = cookie.Split(';');
            foreach (var item in temp)
            {
                var temp2 = item.Trim().Split('=');
                if (temp2.Length > 1)
                {
                    if (temp2[0].Equals("ds_user_id"))
                    {
                        return temp2[1];
                    }
                }
            }
            return "";
        }
        public string getCsrfTokenFromCookie()
        {
            var temp = cookie.Split(';');
            foreach (var item in temp)
            {
                var temp2 = item.Trim().Split('=');
                if (temp2.Length > 1)
                {
                    if (temp2[0].Equals("csrftoken"))
                    {
                        return temp2[1];
                    }
                }
            }
            return "";
        }
        public void getUserNameOfCookie()
        {
            if (string.IsNullOrEmpty(cookie))
            {
                throw new Exception("Error: Cookie null");
            }
            try
            {
                string dataJsonSend = JsonConvert.SerializeObject(new UserInfor(id, true, true, true, false, false, true));
                string url = URI+ PARAGRAP_URI + QUERY_HASH + USERNAME_INFOR + VARIABLES + dataJsonSend;
                string jsonOutput = RequestCustom.GET(url,cookie,"","");
                UsernameRevcive.Rootobject root = JsonConvert.DeserializeObject<UsernameRevcive.Rootobject>(jsonOutput);
                if (root.status.Equals("ok"))
                {
                    UsernameRevcive.Reel data = root.data.user.reel;
                    id = data.id;
                    Username = data.user.username;
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
        public UserInforRecive.User getAllInforUser(string link)
        {
            if (string.IsNullOrEmpty(cookie))
            {
                throw new Exception("Error: Coookie null");
            }
            try
            {
                string url = URI + this.username + PARAGRAP_URI_USERINFOR;
                if (!String.IsNullOrEmpty(link))
                 url=  link + PARAGRAP_URI_USERINFOR;

                string jsonOutput = RequestCustom.GET(url,cookie,"","").ToString();
                UserInforRecive.Rootobject root = JsonConvert.DeserializeObject<UserInforRecive.Rootobject>(jsonOutput);
                if (String.IsNullOrEmpty(link))
                    userInfor = root.graphql.user;
                return root.graphql.user;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void Init()
        {
            this.id = getIDFromCookie(cookie);
            this.CsrfToken = getCsrfTokenFromCookie();
            getUserNameOfCookie();
            getAllInforUser("");
        }

        public void GetResourcePostUser(string id, string after)
        {
            try
            {
                string data = JsonConvert.SerializeObject(new PostOfUser(id, 30, after));
                string url = URI + PARAGRAP_URI + QUERY_HASH + USER_NEWFEED + VARIABLES + data;
                string json = RequestCustom.GET(url,cookie,"","").ToString();
                PostOfUserRecive.Rootobject root = JsonConvert.DeserializeObject<PostOfUserRecive.Rootobject>(json);

                foreach (PostOfUserRecive.Edge item in root.data.user.edge_owner_to_timeline_media.edges)
                {
                    if (isStop)
                    {
                        success("Đã dừng tải: IsStop = true");
                        return;
                    }
                        
                    if (item.node.__typename.Equals("GraphSidecar"))
                    {
                        foreach (PostOfUserRecive.Edge4 item2 in item.node.edge_sidecar_to_children.edges)
                        {
                            CustomResourcePostData dataPost = new CustomResourcePostData();
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
                            dataPost.id = item2.node.id;
                            process(dataPost,proc);
                            proc++;

                        }
                    }
                    else
                    {
                        CustomResourcePostData dataPost = new CustomResourcePostData();
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
                        dataPost.id = item.node.id;
                        process(dataPost, proc);
                        proc++;
                    }

                }
                if (root.data.user.edge_owner_to_timeline_media.page_info.has_next_page)
                    GetResourcePostUser(id,root.data.user.edge_owner_to_timeline_media.page_info.end_cursor);
                else
                {
                    success("");
                }
            }
            catch (Exception ex)
            {
                error(ex);
            }
        }

        public void ActionFollow(string id, bool isFollow, string cookie, ref int status)
        {
            try
            {
                String action = "/unfollow/";
                if (isFollow)
                    action = "/follow/";

                string link = "https://www.instagram.com/web/friendships/" + id + action;
                string json = RequestCustom.POST(link,"","",cookie, CsrfToken, "").ToString();
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
