using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ProjectLibrary.Models
{
    public class WorkContent
    {

        private Thread _thread;

        private ThreadWork _threadWork;

        private Work _work;

        private EventWaitHandle _individualWaitHandle;

        public WorkContent(Work work)
        {
            this._work = work;
            this._individualWaitHandle = new ManualResetEvent(initialState: true);
        }

        public Thread Thread { get => _thread; set => _thread = value; }
        public ThreadWork ThreadWork { get => _threadWork; set => _threadWork = value; }
        public Work Work { get => _work; set => _work = value; }
        public EventWaitHandle IndividualWaitHandle { get => _individualWaitHandle; set => _individualWaitHandle = value; }
    }
}
