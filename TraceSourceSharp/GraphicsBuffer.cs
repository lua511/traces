using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;

namespace TraceSourceSharp
{
    class GraphicsBuffer
    {
        private static GraphicsBuffer _ins;
        public static GraphicsBuffer main
        {
            get
            {
                if(_ins == null)
                {
                    _ins = new GraphicsBuffer();
                }
                return _ins;
            }
        }

        public int width{get;private set;}
        public int height{get;private set;}

        private Bitmap buffer;
        private Bitmap backbuffer;
        private object bufferLocker;
        private object backbufferLocker;

        public GraphicsBuffer()
        {
            width = 0;
            height = 0;
            bufferLocker = new object();
            backbufferLocker = new object();
        }

        public  void    FastDrawBuffer(int w,int h,Graphics g)
        {
            width = w;
            height = h;
            if (Monitor.TryEnter(bufferLocker))
            {
                if (buffer != null && buffer.Width == w && buffer.Height == h)
                {
                    g.DrawImage(buffer, 0, 0);
                }
                Monitor.Exit(bufferLocker);
            }
        }

        public void Update()
        {
            int w = width;
            int h = height;
            if(w <= 0 || h <= 0)
            {
                return;
            }
            if(backbuffer == null)
            {
                backbuffer = new Bitmap(w, h);
            }
            if(backbuffer.Size.Width != w 
                || backbuffer.Size.Height != h)
            {
                backbuffer.Dispose();
                backbuffer = new Bitmap(w, h);
            }
            // draw back buffer
            using(Graphics g = Graphics.FromImage(backbuffer))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighSpeed;
                View.ViewCenter.main.DrawBuffer(g,w,h);
            }
            //swap
            Monitor.Enter(bufferLocker);
            var p = buffer;
            buffer = backbuffer;
            backbuffer = p;
            Monitor.Exit(bufferLocker);
        }
    }
}
