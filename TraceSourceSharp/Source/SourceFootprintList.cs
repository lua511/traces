using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TraceSourceSharp.Source
{
    class SourceFootprintList : IEnumerable
    {
        private SortedList<DateTime, SourceFootprint> footprints =
                            new SortedList<DateTime, SourceFootprint>();


        public void PushPrint(string fullname,DateTime time)
        {
            if(footprints.ContainsKey(time))
            {
               if(footprints[time].fullName != fullname)
               {
                   // i cann't believe you can complete in same second
                   throw new System.IO.InvalidDataException("same time second");
               }
            }
            else
            {
                var v = new SourceFootprint();
                v.fullName = fullname;
                v.time = time;
                footprints.Add(time, v);
            }
            isDirty = true;
        }

        private bool isDirty = false;
        public void ClearDirty()
        {
            isDirty = false;
        }

        public bool IsDirty()
        {
            return isDirty;
        }
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (var v in footprints)
            {
                yield return v.Value;
            }
        }
    }
}
