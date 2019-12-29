using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsstagramTool.ObjectData
{
    class DataPost
    {
        public bool is_video { get; set; }
        public string video_url { get; set; }
        public DataPostOfUser.Display_Resources[] display_desources { get; set; }
        
    }
}
