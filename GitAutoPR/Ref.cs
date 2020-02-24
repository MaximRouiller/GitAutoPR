using System;
using System.Collections.Generic;
using System.Text;

namespace GitAutoPR
{
    public class Ref
    {
        public Object @object { get; set; }
    }

    public class Object
    {
        public string sha { get; set; }
    }

}
