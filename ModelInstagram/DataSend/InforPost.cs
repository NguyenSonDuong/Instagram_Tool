using System;
using System.Collections.Generic;
using System.Text;

namespace ModelInstagram.DataSend
{
    public class InforPost
    {
        public string shortcode { get; set; }
        public int child_comment_count { get; set; }
        public int fetch_comment_count { get; set; }
        public int parent_comment_count { get; set; }
        public bool has_threaded_comments { get; set; }
        public InforPost(string shortcode, int child_comment_count, int fetch_comment_count, int parent_comment_count, bool has_threaded_comments)
        {
            this.child_comment_count = child_comment_count;
            this.fetch_comment_count = fetch_comment_count;
            this.shortcode = shortcode;
            this.parent_comment_count = parent_comment_count;
            this.has_threaded_comments = has_threaded_comments;
        }
    }
}
