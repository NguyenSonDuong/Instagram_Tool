using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsstagramTool.ObjectData
{
    class User
    {
        public string id { get; set; }
        public string username { get; set; }
        public string full_name { get; set; }
        public string profile_pic_url { get; set; }
        public int follow { get; set; }
        public int following { get; set; }
        public bool is_private { get; set; }
        public List<PostData> listPost { get; set; }
        

    }
}
