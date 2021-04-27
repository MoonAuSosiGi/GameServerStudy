using System;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{
    internal class Program
    {
        static ThreadLocal<string> ThreadName = new ThreadLocal<string>(() => { return $"My Name is {Thread.CurrentThread.ManagedThreadId}"; });
        static string ThreadNameNoTLS = "";

        static void WhoAmI()
        {
            bool repeat = ThreadName.IsValueCreated;
            if (repeat)
                Console.WriteLine(ThreadName.Value + " (repeat)");
            else
            {
                // 최초 생성시 value가 null이지만 사용시에 9번째 줄에서 넘긴 Func 무명함수를 실행해 받아오게 된다.
                Console.WriteLine(ThreadName.Value);
            }
            //ThreadName.Value = $"My Name is {Thread.CurrentThread.ManagedThreadId}";
            ThreadNameNoTLS = ThreadName.Value;           
            Thread.Sleep(1000);

            Console.WriteLine(ThreadName.Value);
            Console.WriteLine($"NoTLS {ThreadNameNoTLS}");
        }

        public static void Main(string[] args)
        {
            ThreadPool.SetMinThreads(1, 1);
            ThreadPool.SetMaxThreads(3, 3);
            Parallel.Invoke(WhoAmI, WhoAmI, WhoAmI, WhoAmI, WhoAmI, WhoAmI, WhoAmI, WhoAmI, WhoAmI);
        }
    }
}
