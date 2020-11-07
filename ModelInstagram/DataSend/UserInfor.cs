using System;
using System.Collections.Generic;
using System.Text;

namespace ModelInstagram
{
    public class UserInfor
    {
            public string user_id { get; set; }
            public bool include_chaining { get; set; }
            public bool include_reel { get; set; }
            public bool include_suggested_users { get; set; }
            public bool include_logged_out_extras { get; set; }
            public bool include_highlight_reels { get; set; }
            public bool include_related_profiles { get; set; }

            public UserInfor(string a, bool b, bool c, bool d, bool e, bool f, bool g)
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
