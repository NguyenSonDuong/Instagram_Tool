using System;
using System.Collections.Generic;
using System.Text;

namespace ModelInstagram.DataSend
{
    public class PostOfUser
    {
            public string id { get; set; }
            public int first { get; set; }
            public string after { get; set; }

            public PostOfUser(string a, int b, string c)
            {
                id = a;
                first = b;
                after = c;
            }
    }
}
