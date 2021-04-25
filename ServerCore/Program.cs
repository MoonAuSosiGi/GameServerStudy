using System;
using System.Threading;

namespace ServerCore
{
    internal class Program
    {
        static void MainThread()
        {
            Console.WriteLine("Hello Thread!");
        }

        public static void Main(string[] args)
        {
            Thread t = new Thread(MainThread);
            t.Start();
        }

    }
}
