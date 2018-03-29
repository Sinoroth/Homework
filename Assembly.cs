using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;

namespace Homework2ProducerConsumer
{
    class Assembly
    {
        public static Semaphore semaphore = new Semaphore(1, 1);

        public static ConcurrentBag<int> bag = new ConcurrentBag<int>();

        public static int number = 0;

        public static object modifyList = new object();

        public static void consumer(int consumerNumber) {
            while (true)
            {
                semaphore.WaitOne();

                if (bag.IsEmpty)
                {
                    semaphore.Release();
                    continue;
                }

                int takenNumber;

                lock (modifyList)
                {
                    Interlocked.Decrement(ref number);

                    bag.TryTake(out takenNumber);

                    Console.WriteLine("Consumer number " + consumerNumber + " has taken element " + takenNumber);
                }
                semaphore.Release();

            }
        }

        public static void producer(int producerNumber) {
            while (true)
            {
                semaphore.WaitOne();
                if (bag.Count >= 10)
                {
                    semaphore.Release();
                    continue;
                }

                lock (modifyList)
                {
                    Interlocked.Increment(ref number);

                    bag.Add(number);

                    Console.WriteLine("Producer number " + producerNumber + " has produced element " + number);
                }
                semaphore.Release();
            }
        }

        public void run()
        {
            Thread[] threads = new Thread[10];

            for(int i = 0; i < 5; ++i)
            {
                threads[i] = new Thread(() => producer(i+1));

            }

            for (int i = 0; i < 5; ++i)
            {
                threads[i + 5] = new Thread(() => consumer(i + 1));
            }

            for(int i = 0; i < 10; ++i)
            {
                threads[i].Start();
            }

            for (int i = 0; i < 10; ++i)
            {
                threads[i].Join();
            }

        }
    }
}
