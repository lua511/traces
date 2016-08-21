using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;

namespace TraceSourceSharp.View
{
    class ViewCenter
    {
        private static ViewCenter _ins;
        public static ViewCenter main
        {
            get
            {
                if(_ins == null)
                {
                    _ins = new ViewCenter();
                }
                return _ins;
            }
        }

        private object firstLocker = new object();
        private object secondLocker = new object();

        private ViewBuffer first = new ViewBuffer();
        private ViewBuffer second = new ViewBuffer();

        public ViewBuffer   AcquireFirst()
        {
            Monitor.Enter(firstLocker);
            return first;
        }
        public void ReleaseFirst()
        {
            Monitor.Exit(firstLocker);
        }

        public void SwapBuffer()
        {
            var p = first;
            Monitor.Enter(firstLocker);
            first = second;
            Monitor.Exit(firstLocker);
            second = p;
        }

        public  ViewBuffer  GetSecond()
        {
            return second;
        }

        public int XOff { get; set; }
        public int YOff { get;set; }

        public int LastMaxWidth { get; private set; }
        public int LastMaxHeight { get; private set; }

        public  void DrawBuffer(Graphics graphics,int w,int h)
        {
            var dataw = w;
            var datah = h;
            if(Monitor.TryEnter(firstLocker))
            {
                try
                {
                    graphics.Clear(Color.LightYellow);
                    foreach (var ele in first)
                    {
                        var p = ele as View.ViewElement;
                        if (p.x + 200 > dataw) dataw = p.x + 200;
                        if (p.y + 200 > datah) datah = p.y + 200;

                        if (p.x >= XOff && p.y >= YOff && p.x <= XOff + w
                            && p.y <= YOff + h)
                        {
                            graphics.FillRectangle(Brushes.LightPink
                                            , p.x - XOff, p.y - YOff
                                            , 90, 30);
                            //graphics.DrawString(p.fullName.Substring(1, 20));
                        }
                    }
                }
                finally
                {
                    Monitor.Exit(firstLocker);
                }
            }
            LastMaxWidth = dataw;
            LastMaxHeight = datah;
        }
    }
}
