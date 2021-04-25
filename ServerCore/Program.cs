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

            for (int i = 0; i < 4; i++)
                ThreadPool.QueueUserWorkItem((obj) => { while (true) { } });

            // 위에서 최대 개수를 넘지 않게 했기 때문에, 아래는 생성됨
            ThreadPool.QueueUserWorkItem(MainThread);      
            while(true)
            {

            }
        }

    }
}
