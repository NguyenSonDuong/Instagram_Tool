using System;
using System.Collections.Generic;
using System.Text;

namespace ModelInstagram.DataRecive
{
    public class UsernameRevcive
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
}
