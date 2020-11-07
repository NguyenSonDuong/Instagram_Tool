using System;
using System.Collections.Generic;
using System.Text;

namespace ModelInstagram.DataRecive
{
    public class UserInforRecive
    {
        public class Rootobject
        {
            public string logging_page_id { get; set; }
            public bool show_suggested_profiles { get; set; }
            public bool show_follow_dialog { get; set; }
            public Graphql graphql { get; set; }
            public object toast_content_on_load { get; set; }
            public bool show_view_shop { get; set; }
            public object profile_pic_edit_sync_props { get; set; }
        }

        public class Graphql
        {
            public User user { get; set; }
        }

        public class User
        {
            public string biography { get; set; }
            public bool blocked_by_viewer { get; set; }
            public object business_email { get; set; }
            public bool restricted_by_viewer { get; set; }
            public bool country_block { get; set; }
            public string external_url { get; set; }
            public string external_url_linkshimmed { get; set; }
            public Edge_Followed_By edge_followed_by { get; set; }
            public bool followed_by_viewer { get; set; }
            public Edge_Follow edge_follow { get; set; }
            public bool follows_viewer { get; set; }
            public string full_name { get; set; }
            public bool has_ar_effects { get; set; }
            public bool has_clips { get; set; }
            public bool has_guides { get; set; }
            public bool has_channel { get; set; }
            public bool has_blocked_viewer { get; set; }
            public int highlight_reel_count { get; set; }
            public bool has_requested_viewer { get; set; }
            public string id { get; set; }
            public bool is_business_account { get; set; }
            public bool is_joined_recently { get; set; }
            public object business_category_name { get; set; }
            public object overall_category_name { get; set; }
            public object category_enum { get; set; }
            public bool is_private { get; set; }
            public bool is_verified { get; set; }
            public Edge_Mutual_Followed_By edge_mutual_followed_by { get; set; }
            public string profile_pic_url { get; set; }
            public string profile_pic_url_hd { get; set; }
            public bool requested_by_viewer { get; set; }
            public string username { get; set; }
            public object connected_fb_page { get; set; }
            public Edge_Felix_Video_Timeline edge_felix_video_timeline { get; set; }
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
            public Edge[] edges { get; set; }
        }

        public class Edge
        {
            public Node node { get; set; }
        }

        public class Node
        {
            public string username { get; set; }
        }

        public class Edge_Felix_Video_Timeline
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

        public class Edge_Owner_To_Timeline_Media
        {
            public int count { get; set; }
            public Page_Info1 page_info { get; set; }
            public Edge1[] edges { get; set; }
        }

        public class Page_Info1
        {
            public bool has_next_page { get; set; }
            public string end_cursor { get; set; }
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
            public Dimensions dimensions { get; set; }
            public string display_url { get; set; }
            public Edge_Media_To_Tagged_User edge_media_to_tagged_user { get; set; }
            public object fact_check_overall_rating { get; set; }
            public object fact_check_information { get; set; }
            public object gating_info { get; set; }
            public object sharing_friction_info { get; set; }
            public object media_overlay_info { get; set; }
            public string media_preview { get; set; }
            public Owner owner { get; set; }
            public bool is_video { get; set; }
            public string accessibility_caption { get; set; }
            public Edge_Media_To_Caption edge_media_to_caption { get; set; }
            public Edge_Media_To_Comment edge_media_to_comment { get; set; }
            public bool comments_disabled { get; set; }
            public int taken_at_timestamp { get; set; }
            public Edge_Liked_By edge_liked_by { get; set; }
            public Edge_Media_Preview_Like edge_media_preview_like { get; set; }
            public object location { get; set; }
            public string thumbnail_src { get; set; }
            public Thumbnail_Resources[] thumbnail_resources { get; set; }
        }

        public class Dimensions
        {
            public int height { get; set; }
            public int width { get; set; }
        }

        public class Edge_Media_To_Tagged_User
        {
            public Edge2[] edges { get; set; }
        }

        public class Edge2
        {
            public Node2 node { get; set; }
        }

        public class Node2
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

        public class Owner
        {
            public string id { get; set; }
            public string username { get; set; }
        }

        public class Edge_Media_To_Caption
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

        public class Edge_Media_To_Comment
        {
            public int count { get; set; }
        }

        public class Edge_Liked_By
        {
            public int count { get; set; }
        }

        public class Edge_Media_Preview_Like
        {
            public int count { get; set; }
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
            public Page_Info2 page_info { get; set; }
            public object[] edges { get; set; }
        }

        public class Page_Info2
        {
            public bool has_next_page { get; set; }
            public object end_cursor { get; set; }
        }

        public class Edge_Media_Collections
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

    }
}
