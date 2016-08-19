using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TraceSourceSharp.View
{
    class ViewElement : Source.SourceFootprint
    {
        public int x { get; set; }
        public int y { get; set; }
        public int hot { get; set; }
    }
}
