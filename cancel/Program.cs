using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace cancel
{
    class Program
    {
        static async Task Main(string[] args)
        {
            
            var cancel = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            await DoSomething(cancel.Token);
            
            Console.WriteLine("Hello World!");
        }

        static async Task DoSomething(CancellationToken token)
        {
 
            for(var i = 0 ; i < 100 ; i++)
            {
                await Task.Delay(1000,token);
                Console.WriteLine(i);
            }
        }

        static void ParllelCancel(IEnumerable<int> list ,CancellationToken token)
        {
            Parallel.ForEach(list,new ParallelOptions{CancellationToken = token},x=>Console.WriteLine(x));
        }

        static void PlinqCancel(IEnumerable<int> list ,CancellationToken token)
        {
            list.AsParallel().WithCancellation(token).ForAll(Console.WriteLine);
        }
    }
}
