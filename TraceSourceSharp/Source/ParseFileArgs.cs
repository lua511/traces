using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TraceSourceSharp.Source
{
    class ParseFileArgs
    {
        public bool IsCompleted { get; set; }

        private List<string> elements = new List<string>();

        public   List<string>   Elements
        {
            get
            {
                return elements;
            }
        }
    }
}
