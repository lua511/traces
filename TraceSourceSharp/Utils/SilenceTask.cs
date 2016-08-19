using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace TraceSourceSharp.Utils
{
    class SilenceTask
    {
        private static SilenceTask _ins;
        public static SilenceTask main
        {
            get
            {
                if(_ins == null)
                {
                    _ins = new SilenceTask();                    
                }
                return _ins;
            }
        }

        public  void PostTask(Action act,bool bQueue)
        {
            Thread trd = new Thread(
                    () =>
                    {
                        if (bQueue)
                        {
                            lock (SilenceTask.main)
                            {
                                act();
                            }
                        }
                        else
                        {
                            act();
                        }
                    }
                );
            trd.IsBackground = true;
            trd.Start();
        }
    }
}
