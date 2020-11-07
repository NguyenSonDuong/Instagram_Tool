using System;
using System.Collections.Generic;
using System.Text;

namespace ModelInstagram.DataRecive
{
    public class PostOfUserRecive
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
}
