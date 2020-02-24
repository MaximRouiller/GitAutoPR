using System;
using System.Collections.Generic;
using System.Text;

namespace GitAutoPR
{
    public class Ref
    {
        public string @ref { get; set; }
        public string node_id { get; set; }
        public string url { get; set; }
        public Object @object { get; set; }
    }

    public class Object
    {
        public string sha { get; set; }
        public string type { get; set; }
        public string url { get; set; }
    }

}
