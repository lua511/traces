using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TraceSourceSharp.Source
{
    class SourceFile
    {
        /// <summary>
        /// the showing name 
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// the full name with path
        /// </summary>
        public string fullName { get;set;}

        /// <summary>
        /// last write time
        /// </summary>
        public DateTime writeTime;

        /// <summary>
        /// how many times do we focus this file
        /// </summary>
        public int footPrint { get; set; }
    }
}
