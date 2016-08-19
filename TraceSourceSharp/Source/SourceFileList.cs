using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TraceSourceSharp.Source
{
    class SourceFileList
    {
        /// <summary>
        /// key:full path
        /// value: SourceFile
        /// </summary>
        private Dictionary<string, SourceFile> sources =
                                new Dictionary<string, SourceFile>();

        public void PushFile(string fullname,DateTime writetime)
        {
            if (sources.ContainsKey(fullname))
            {
                var v = sources[fullname];
                v.writeTime = writetime;
            }
            else
            {
                var v = new SourceFile();
                var fi = new System.IO.FileInfo(fullname);
                v.name = fi.Name;
                v.fullName = fullname;
                v.writeTime = writetime;
                v.footPrint = 0;
                sources.Add(fullname, v);
            }
        }

        public  bool    CheckChanged(string fullname,DateTime writetime)
        {
            if(!sources.ContainsKey(fullname))
            {
                return true;
            }
            var v = sources[fullname];
            if(v.writeTime != writetime)
            {
                return true;
            }
            return false;
        }

        // the caller check the parameter.
        public  void    MarkPrint(string fullname)
        {
            ++sources[fullname].footPrint;
        }

        public int GetHot(string fullname)
        {
            if(sources.ContainsKey(fullname))
            {
                return sources[fullname].footPrint;
            }
            return 0;
        }
    }
}
