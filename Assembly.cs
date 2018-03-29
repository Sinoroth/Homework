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
        public static Semaphore semaphore = new Semaphore(3, 3);

        private static AssemblyLine al;

        public static object modifyList = new object();

        public Assembly()
        {
            al = new AssemblyLine(10);
        }

        public static void consumer(int consumerNumber) {
            while (true)
            {
                semaphore.WaitOne();

                if (al.isEmpty())
                {
                    semaphore.Release();
                    continue;
                }

                lock (modifyList)
                {
                    if (al.isEmpty())
                    {
                        semaphore.Release();
                        continue;
                    }

                    al.consume(consumerNumber);

                }
                semaphore.Release();

            }
        }

        public static void producer(int producerNumber) {
            while (true)
            {
                semaphore.WaitOne();
                if (al.isThereRoomLeft() == false)
                {
                    semaphore.Release();
                    continue;
                }

                lock (modifyList)
                {
                    if(al.isThereRoomLeft() == false)
                    {
                        semaphore.Release();
                        continue;
                    }
                    al.addOne(producerNumber);
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

        class AssemblyLine
        {
            int currentNumberOfObjects, totalNumberOfObjects;

            public AssemblyLine(int totalObjects)
            {
                totalNumberOfObjects = totalObjects;
                currentNumberOfObjects = 0;
            }

            public void addOne(int producerN)
            {
                currentNumberOfObjects++;
                Console.WriteLine("Producer " + producerN+" produced item " + currentNumberOfObjects);
            }

            public void consume(int consumerN)
            {
                Console.WriteLine("Consumer " + consumerN + " consumed item " + currentNumberOfObjects);
                currentNumberOfObjects--;
            }

            public bool isThereRoomLeft()
            {
                return currentNumberOfObjects < totalNumberOfObjects;
            }

            public bool isEmpty()
            {
                if (currentNumberOfObjects == 0) return true;
                return false;
            }


        }
    }
}
