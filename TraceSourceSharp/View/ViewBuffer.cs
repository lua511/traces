using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TraceSourceSharp.View
{
    class ViewBuffer : IEnumerable
    {
        private List<ViewElement> elements = new List<ViewElement>();

        // a sample method for sort
        public void SortElement()
        {
            // first get hot
            foreach(var e in elements)
            {
                e.hot = Source.DataCenter.main.Sources.GetHot(e.fullName);
            }

            // we make the date one by one
            for(int i = 0;i < elements.Count; ++i)
            {
                var e = elements[i];
                e.x = 40 + i * 110;
                e.y = 50;
            }
        }

        public  void    Reset()
        {
            elements.Clear();
        }

        public  void    PushData(Source.SourceFootprint sfp)
        {
            ViewElement ve = new ViewElement();
            ve.fullName = sfp.fullName;
            ve.time = sfp.time;
            ve.description = sfp.description;
            elements.Add(ve);
        }

        public IEnumerator GetEnumerator()
        {
            foreach(var p in elements)
            {
                yield return p;
            }
        }
    }
}
