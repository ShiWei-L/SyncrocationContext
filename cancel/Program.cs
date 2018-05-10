using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace cancel
{
    interface IAsyncCompletion
    {
        void Complete();

        Task Completion { get; }
    }

    class MyClass : IAsyncCompletion
    {
        public int Id 
        {
            get { return 1; }
        }
        private readonly TaskCompletionSource<object> _completion = new TaskCompletionSource<object>();

        private Task _comleting;
        public Task Completion 
        {
            get { return _completion.Task ;}
        }

        public void Complete()
        {
           if(_comleting!=null)
                return;
           _comleting = ComleteAsync();
        }

        private async Task ComleteAsync()
        {
            try
            {
               await Task.Delay(500);
            }
            catch (Exception ex)
            {
                _completion.TrySetException(ex);
            }
            finally{
                _completion.TrySetResult(null);
            }
            
        }
    }

    static class AsyncHelper
    {
        public static async Task<TResult> Using<TResource,TResult>(
            Func<TResource> construct,
            Func<TResource,Task<TResult>> process
        ) where TResource :MyClass, IAsyncCompletion
        {
                var resource  =  construct();

                Exception exception = null;

                TResult res = default(TResult);

                try
                {
                    res = await process(resource);
                }
                catch(Exception ex)
                {
                    exception = ex;
                }

                resource.Complete();

                try
                {   
                    await resource.Completion;
                    Console.WriteLine(resource.Id);
                }
                catch
                {
                    if(exception==null)
                        throw;
                }
                if(exception!=null)
                {
                    ExceptionDispatchInfo.Capture(exception).Throw();
                }

                return res;
        }
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            var result= await AsyncHelper.Using(()=>new MyClass(),async resource =>
            {
                await Task.Delay(5);
                return 1;
            });


            
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
