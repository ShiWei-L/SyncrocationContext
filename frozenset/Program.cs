using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace frozenset {
    class Program {
        static void Main (string[] args) {

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
           Task.Factory.StartNew(()=>
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


            Console.WriteLine ("Hello World!");
        }
    }
}