using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace TraceSourceSharp.Source
{
    class ParseFolder
    {

        private Thread parseThread = null;
        private string rootFolder; 

        public  bool    Parse(string folderName)
        {
            if (parseThread != null)
            {
                return false;
            }
            if(!Directory.Exists(folderName))
            {
                throw new System.FieldAccessException("cann't find folder");
            }
            parseThread = new Thread(ParseRoot);
            parseThread.IsBackground = true;
            return true;
        }

        public  bool    Startup()
        {
            return true;
        }

        void ParseRoot()
        {

        }
    }
}
