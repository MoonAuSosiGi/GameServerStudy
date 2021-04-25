using System;
using System.Threading;

namespace ServerCore
{
    internal class Program
    {
        static void MainThread(object state)
        {
            for (int i = 0; i < 5; i++)
                Console.WriteLine("Hello Thread!");
        }

        public static void Main(string[] args)
        {
            ThreadPool.SetMinThreads(1, 1);
            ThreadPool.SetMaxThreads(5, 5);

            for (int i = 0; i < 5; i++)
                ThreadPool.QueueUserWorkItem((obj) => { while (true) { } });

            // 아래는 생성되지 않음.
            ThreadPool.QueueUserWorkItem(MainThread);      
            while(true)
            {

            }
        }

    }
}
