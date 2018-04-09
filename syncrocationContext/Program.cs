using System;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace syncrocationContext {
  class Program {
    static async Task Main (string[] args) {
      // Console.WriteLine (Thread.CurrentThread.ManagedThreadId);

      // await Task.Factory.StartNew (() => Console.WriteLine (Thread.CurrentThread.ManagedThreadId));
      // Console.WriteLine (Thread.CurrentThread.ManagedThreadId);

      // Console.WriteLine (Thread.CurrentThread.ManagedThreadId);
      // await Task.Factory.StartNew (() => Console.WriteLine (Thread.CurrentThread.ManagedThreadId));
      // Console.WriteLine (Thread.CurrentThread.ManagedThreadId);
      // Console.WriteLine (Thread.CurrentThread.ManagedThreadId);
      // Console.WriteLine (Thread.CurrentThread.ManagedThreadId);
      await Task.Delay (10);
      // Observable.Interval (TimeSpan.FromSeconds (1)).Timestamp ()
      //   .Where (x => x.Value % 2 == 0).Select (x => x.Timestamp).Subscribe (x => {
      //     Console.WriteLine(x);
      //   });

      //Console.ReadLine();

      #region 数据流
      // var tf = new TransformBlock<int, int> (item => item * 2);
      // var sb = new TransformBlock<int, int> (item => item - 2);

      // tf.LinkTo (sb);
      // // tf.Complete ();
      // // await sb.Completion;

      // //要实现完成情况需要在链接中设置propagteCompletion属性
      // var options = new DataflowLinkOptions {PropagateCompletion = true};
      // tf.LinkTo(sb,options);

      // //第一个块完成的情况下自动传递给第二个块
      // tf.Complete();
      // await sb.Completion;

      //传递出错信息
      var block = new TransformBlock<int, int> (item => {
          if (item == 1)
            throw new InvalidOperationException ("xxx");
          return item * 2;
        });
      block.Post (1); //出错,block进行故障状态,删除所有数据并停止接收数据
      block.Post (2);

      try {
        await block.Completion;
      } catch (InvalidOperationException ex) {
        Console.WriteLine (ex.Message);
      }

      //断开连接
      var link = block.LinkTo (new TransformBlock<int, int> (item => item - 2));
      link.Dispose ();

      //限制流量，使数据公平分发
      var sourceBlock = new BufferBlock<int> ();
      var options = new DataflowBlockOptions { BoundedCapacity = 1 };
      var targetB = new BufferBlock<int> (options);
      var targetA = new BufferBlock<int> (options);
      sourceBlock.LinkTo (targetA);
      sourceBlock.LinkTo (targetB);

      //数据流块并行处理 设置ataflowBlockOptions.Unbounded或大于零的数值
      var multplyblock = new TransformBlock<int,int>(item=>item*2,new ExecutionDataflowBlockOptions{
        MaxDegreeOfParallelism = DataflowBlockOptions.Unbounded
      });

      //自定义数据流块 使用Envapsulate方法取出数据流网络中任何具有单一输入块和输出块的部分,利用两个端点创建单独的数据流块
      IPropagatorBlock<int,int> createMyCustomBlock()
      {
        var mb = new TransformBlock<int,int>(item=>item*2);
        var addblock = new TransformBlock<int,int>(item=>item+2);
        var divideBlock = new TransformBlock<int,int>(item=>item/2);

        var flowCompletion = new  DataflowLinkOptions { PropagateCompletion = true };
        mb.LinkTo(addblock,flowCompletion);
        divideBlock.LinkTo(divideBlock,flowCompletion);
        
        return DataflowBlock.Encapsulate(mb,divideBlock);
      }
      #endregion

    }
  }
}