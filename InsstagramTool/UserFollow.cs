using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsstagramTool
{
    public class UserFollow
    {
        public string ID { get; set; }
        public string username { get; set; }
        public string full_name { get; set; }
        public string profile_pic_url { get; set; }

        public UserFollow(string id,string username,string full_name,string profile_pic_url)
        {
            this.ID = id;
            this.username = username;
            this.full_name = full_name;
            this.profile_pic_url = profile_pic_url;
        }
    }
}
