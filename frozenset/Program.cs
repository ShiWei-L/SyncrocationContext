using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Nito.AsyncEx;

namespace frozenset {
    class Program {
        async Task Main (string[] args) {

            //不可变集合是永远不会改变的集合,写入操作会返回新实例，不可变集合之间通常共享了大部分存储空间，浪费不大。多个线程访问安全
            
            ///不可变栈
            var list = ImmutableStack<int>.Empty;
            list = list.Push (13);
            list = list.Push (15);

            foreach (var item in list) {
                Console.WriteLine (item);
            }
            ///不可变队列 ImmutableQueue
            
            //不可变列表
            //支持索引、不经常修改、可以被多个线程安全访问
            var immutlist = ImmutableList<int>.Empty;
            immutlist = immutlist.Insert(0,13);
            immutlist = immutlist.Insert(0,7);

            //不可变set集合
            //不需要存放重复内容，不经常修改，可以被多个线程安全访问
            //ImmutableHashSet 不含重复元素的集合
            //ImmutableSortedSet 已排序不含重复元素的集合


            //不可变字典
            //ImmutableSortedDictionary
            //ImmutableDictionary



            //线程安全集合是可同时被多个线程修改的可变集合，线程安全集合混合使用了细粒度锁定和无锁技术,优点是多个线程可安全地对其进行访问
            //线程安全字典
            //需要有一个键/值集合，多个线程同时读写时仍能保持同步
            //ConcurrentDictionary
            var dictionary = new ConcurrentDictionary<int,string>();
            //第一个委托把本来的键0转换成值zero,第二个委托把键0和原来的值转换成字典中修改后的值 zero，只有字典中已民存在这个键时，最后一个委托才会运行
            dictionary[0]="zero";
            var newValue = dictionary.AddOrUpdate(0,key=>"Zero",(key,oldValue)=>"Zero1");
            dictionary.TryGetValue(0,out string currentalue);
            Console.WriteLine(currentalue);
            dictionary.TryRemove(0,out string removeValue);
            
            
            //生产费消费者模型
            //阻塞队列
            //需要有一个管道,在进行之间传递消息或数据，例如一个线程下大装载数据，装载的同时把数据压进管道，与此同时，另一个线程在管道的接收端接收处理数据
            //BlockingCollection 类可当做这种管道，阻塞队列，先进先出 限流 bounedCapacity属性
            //不过如果用到这个的话，更推荐数据流
            var blockqueue = new BlockingCollection<int>();
           var blockqueueTask= Task.Factory.StartNew(()=>
           {
              blockqueue.Add(7);
              blockqueue.Add(8);
              blockqueue.Add(9);
            blockqueue.CompleteAdding();
           });

            foreach(var item in blockqueue.GetConsumingEnumerable())
            {
                Console.WriteLine(item);
            }
            await blockqueueTask;

            //阻塞栈和包
            //首先有一个管道，在线程之间传递消息或数据，但不想（不需要）这个管道使用先进先出的语义
            //blockingCollection 可以在创建时选择规则
            var _blockingStack = new BlockingCollection<int>(new ConcurrentBag<int>());


            //异步队列
            //在代码的各个部分之间以选进先出的方式传递消息或数据 多个消费者时需要注意捕获InvalidOperationException异常
            var _syncQueue = new BufferBlock<int>();
            await _syncQueue.SendAsync(7);
            await _syncQueue.SendAsync(13);

            _syncQueue.Complete();


            while(await _syncQueue.OutputAvailableAsync())
            {
                Console.WriteLine(await _syncQueue.ReceiveAsync());
            }


            //异步栈和包
            //需要有一个管道，在程序的各个部分传递数据，但不希望先进先出
            var _asyncStack = new AsyncCollection<int>(new ConcurrentBag<int>(),maxCount:1);
            //这个添加操作会立即完成，下一个添加会等待7被移除后
            await _asyncStack.AddAsync(7);
            _asyncStack.CompleteAdding();
            while(await _asyncStack.OutputAvailableAsync())
            {
                var taskReuslt = await _asyncStack.TryTakeAsync();
                if(taskReuslt.Success)
                {
                    Console.WriteLine(taskReuslt.Item);
                }
            }


            //阻塞/异步队列
            //先进先出 足够灵活 同步或异步方式处理
            var queue = new  BufferBlock<int>();
            await queue.SendAsync(1);
            queue.Complete(); 

            while(await queue.OutputAvailableAsync())
            {
                Console.WriteLine(await queue.ReceiveAsync());
            }



            
            Console.WriteLine ("Hello World!");
        }
    }
}