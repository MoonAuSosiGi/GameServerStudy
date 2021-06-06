using System;
using System.Collections.Generic;
using System.Text;
using ServerCore;

namespace Server
{
    struct JobTimerElem : IComparable<JobTimerElem>
    {
        public int execTick; // 실행시간
        public Action action;

        public int CompareTo(JobTimerElem other)
        {
            return other.execTick - execTick;
        }
    }
    class JobTimer
    {
        private PriorityQueue<JobTimerElem> _pq = new PriorityQueue<JobTimerElem>();
        private object _lock = new object();

        public static JobTimer Instance { get; } = new JobTimer();

        public void Push(Action action, int tickAfter = 0)
        {
            JobTimerElem job;
            job.execTick = System.Environment.TickCount + tickAfter;
            job.action = action;

            lock (_lock)
            {
                _pq.Push(job);
            }
        }

        public void Flush()
        {
            while (true)
            {
                int now = System.Environment.TickCount;
                JobTimerElem job;

                lock (_lock)
                {
                    if (_pq.Count == 0)
                        break; // lock 에 영향을 끼치는게 아님

                    job = _pq.Peek();

                    if (job.execTick > now)
                        break;
                    _pq.Pop();
                }
                job.action.Invoke();
            }
        }

    }
}
