using System;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{
    class Lock
    {
        // 커널에 있는 bool 변수라고 보면 됨
        ManualResetEvent _available = new ManualResetEvent(true);

        public void Acquire()
        {
            _available.WaitOne(); // 입장 시도
            // 자동으로 닫지 않으므로 수동으로 처리
            _available.Reset();
        }

        public void Release()
        {
            _available.Set(); // 이벤트 상태 변경
        }
    }
    
    internal class Program
    {
        static int _num = 0;
        static Lock _lock = new Lock();

        static void Thread_1()
        {
            for(int i = 0; i < 100000; i++)
            {
                _lock.Acquire();
                _num++;
                _lock.Release();
            }
        }

        static void Thread_2()
        {
            for (int i = 0; i < 100000; i++)
            {
                _lock.Acquire();
                _num--;
                _lock.Release();
            }
        }

        public static void Main(string[] args)
        {
            Task t1 = new Task(Thread_1);
            Task t2 = new Task(Thread_2);

            t1.Start();
            t2.Start();

            Task.WaitAll(t1, t2);

            Console.WriteLine($"{_num}");
        }
    }
}
