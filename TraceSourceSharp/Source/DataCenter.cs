﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace TraceSourceSharp.Source
{
    class DataCenter
    {
        #region Instance
        private static DataCenter _ins;
        public static DataCenter main
        {
            get
            {
                if (_ins == null)
                {
                    _ins = new DataCenter();
                }
                return _ins;
            }
        }        
        private DataCenter()
        {
            islocked = false;
            Prints = new SourceFootprintList();
            Sources = new SourceFileList();
            config = new DataCenterConfig();
        }
        #endregion
        public SourceFootprintList Prints { get; private set; }
        public SourceFileList Sources { get; private set; }
        public DataCenterConfig config { get; private set; }
        #region Parse
        private bool islocked { get; set; }
       
        public void Reload()
        {
            if(islocked)
            {
                return;
            }
            islocked = true;
            FireStatusInformation("Processing");
            ThreadPool.QueueUserWorkItem(ParseRoot);
        }
        void ParseRoot(object param)
        {
            try
            {
                if(string.IsNullOrEmpty(config.root))
                {
                    throw new FileNotFoundException("goto");
                }
                if(!Directory.Exists(config.root))
                {
                    throw new FileNotFoundException("goto");
                }
                config.files.Clear();
                ParseFiles(config.root);
                //10.3s@40
                //10.0s@20
                //10.0s@10
                //10.0s@5
                //10.0s@2
                //16.0s@1
                int SECTION_COUNT = 5;
                ParseFileArgs[] pargs = new ParseFileArgs[SECTION_COUNT];
                for(int i = 0;i < SECTION_COUNT; ++i)
                {
                    pargs[i] = new ParseFileArgs();
                    pargs[i].IsCompleted = false;
                }
                int fi = 0;
                foreach(var f in config.files)
                {
                    pargs[fi].Elements.Add(f);
                    ++fi;
                    if(fi >= SECTION_COUNT)
                    {
                        fi = 0;
                    }
                }
                for (int i = 0; i < SECTION_COUNT; ++i)
                {
                    ThreadPool.QueueUserWorkItem(FileArgsWorker,pargs[i]);
                }
                while(true)
                {
                    bool bcompleted = true;
                    for(int i = 0;i < SECTION_COUNT;++i)
                    {
                        if(!pargs[i].IsCompleted)
                        {
                            bcompleted = false;
                            break;
                        }
                    }
                    if(bcompleted)
                    {
                        break;
                    }
                    Thread.Sleep(500);
                }
                FireStatusInformation("Completed");
            }
            catch(FileNotFoundException e)
            {
                FireStatusInformation("NoWorkingDirectory");
            }
            finally
            {
                islocked = false;
            }
            FireStatusParseComplete();

            if(Prints.IsDirty())
            {
                var vb = View.ViewCenter.main.GetSecond();
                vb.Reset();
                foreach(var v in Prints)
                {
                    vb.PushData(v as SourceFootprint);
                }
                vb.SortElement();
                View.ViewCenter.main.SwapBuffer();
            }
        }
        void ParseFiles(string root)
        {
            string[] files = Directory.GetFiles(root);
            foreach(var f in files)
            {
                config.files.Add(f);
            }
            string[] folders = Directory.GetDirectories(root);
            foreach(var f in folders)
            {
                ParseFiles(f);
            }
        }

        void    FileArgsWorker(object param)
        {
            ParseFileArgs pa = param as ParseFileArgs;
            foreach(var f in pa.Elements)
            {
                ParseFileInfo(f);
            }
            pa.IsCompleted = true;
        }

        bool ParseFileInfo(string fname)
        {
            var fi = new FileInfo(fname);
            if(Sources.CheckChanged(fname,fi.LastWriteTime))
            {
                Sources.PushFile(fname, fi.LastWriteTime);
                ParseFileContent(fname);
            }
            return true;
        }

        private const string DATE_PATTERN = 
                                @"\d{4}年\d{1,2}月\d{1,2}日\d{2}\:\d{2}\:\d{2}";
        private const string DATE_FORMAT = "yyyy年MM月dd日HH:mm:ss";
        bool ParseFileContent(string fname)
        {
            if(fname.EndsWith("gladevcp_panel.ini"))
            {
                System.Console.WriteLine("here it is");
            }
            using(var fs = new FileStream(fname
                ,FileMode.Open,FileAccess.Read,FileShare.ReadWrite))
            using (var sr = new StreamReader(fs,Encoding.Default))
            {
                do
                {
                    var str = sr.ReadLine();
                    if(str == null)
                    {
                        break;
                    }
                    Match m = Regex.Match(str, DATE_PATTERN);
                    if(m.Success)
                    {
                        var dtFormat = new DateTimeFormatInfo();
                        dtFormat.ShortDatePattern = DATE_FORMAT;
                        DateTime dt = Convert.ToDateTime(m.Value, dtFormat);
                        Sources.MarkPrint(fname);
                        Prints.PushPrint(fname, dt);
                    }
                }
                while (true);
            }
            return true;
        }
        #endregion
        #region status
        public event Action<string> StatusEvent;
        private void FireStatusParseComplete()
        {
            if(StatusEvent != null)
            {
                StatusEvent("(2)" + DateTime.Now.ToString("HH:mm:ss"));
            }
        }
        private void FireStatusInformation(string p)
        {
            if(StatusEvent != null)
            {
                StatusEvent("(1)" + p);
            }
        }
        #endregion
    }
}
