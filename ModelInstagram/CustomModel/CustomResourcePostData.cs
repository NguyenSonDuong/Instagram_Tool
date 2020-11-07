using ModelInstagram.DataRecive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsstagramTool.ObjectData
{
    public class CustomResourcePostData
    {
        public String id { get; set; }
        public bool is_video { get; set; }
        public string video_url { get; set; }
        public PostOfUserRecive.Display_Resources[] display_desources { get; set; }
        
    }
}
