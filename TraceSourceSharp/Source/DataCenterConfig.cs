using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TraceSourceSharp.Source
{
    class DataCenterConfig
    {
        public DataCenterConfig()
        {
            files = new List<string>();
        }
        public string root { get; set; }
        public List<string> files { get; set; }
    }
}
