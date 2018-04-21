using System;
using System.Diagnostics;
using System.Net;
using System.Reactive.Linq;
using System.Threading;
using System.Timers;

namespace rx {
    class Program {
        static void Main (string[] args) {
            // var progress = new Progress<int> ();
            // var progressReports = Observable.FromEventPattern<int> (
            //     handler => progress.ProgressChanged += handler,
            //     handler => progress.ProgressChanged -= handler
            // );
            // progressReports.Subscribe (data => Trace.WriteLine ("OnNext:" + data.EventArgs));

            // //对于非EventHandler<T>事件的转换,构造参数第一个lamdba是转换器
            // // var timer = new System.Timers.Timer (interval: 1000) { Enabled = true };
            // // var ticks = Observable.FromEventPattern<ElapsedEventHandler, ElapsedEventArgs> (
            // //         handler => (s, a) => handler (s, a),
            // //         handler => timer.Elapsed += handler,
            // //         handler => timer.Elapsed -= handler
            // //     );

            // //较为简单的处理方式,但是data.eventargs为object类型
            // var timer = new System.Timers.Timer (interval: 1000) { Enabled = true };
            // var ticks = Observable.FromEventPattern (timer, "Elapsed");
            // ticks.Subscribe (data => Trace.WriteLine ("OnNext:" + ((ElapsedEventArgs) data.EventArgs).SignalTime));

            //标准事件模式 第一个参数是事件发送者 第二个参数是事件的类型参数
            //事件封装到observable后,引发事件会调用OnNext,处理asyncCompletedEventArgs异常信息会通过OnNext而不是OnError
            var client = new WebClient ();
            var download = Observable.FromEventPattern (client, "DownloadStringCompleted");
            download.Subscribe (
                data => {
                    var eventArgs = (DownloadStringCompletedEventArgs) data.EventArgs;
                    if (eventArgs.Error != null)
                        Console.WriteLine ("OnNext: (Error)" + eventArgs.Error);
                    else
                        Console.WriteLine ("OnNext: " + eventArgs.Result);
                },
                ex => Console.WriteLine ("OnError: " + ex.ToString ()),
                () => Console.WriteLine ("OnCpmled")
            );
            client.DownloadStringAsync(new Uri("http://xxx.com"));

        

            //在特定线程发送上下文
            var currentContext = SynchronizationContext.Current;
            Observable.Interval(TimeSpan.FromSeconds(1)).ObserveOn(currentContext).Subscribe(x=>Console.WriteLine(Environment.CurrentManagedThreadId));
            

            //事件分组
            Observable.Interval(TimeSpan.FromSeconds(1)).Buffer(2).Subscribe(x=>Console.WriteLine($"{Environment.CurrentManagedThreadId} {x[0]} {x[1]}"));

            Observable.Interval(TimeSpan.FromSeconds(1)).Window(2).Subscribe(group=>
            {
                group.Subscribe(x=>Console.WriteLine(x));
            });

            //事件限流
            //每当事件到达重新计时.发送最后的事件
            Observable.Interval(TimeSpan.FromMilliseconds(200)).Throttle(TimeSpan.FromSeconds(1)).Subscribe(x=>Console.WriteLine(x));
            //不重新计时,发送最后的事件
            Observable.Interval(TimeSpan.FromMilliseconds(200)).Sample(TimeSpan.FromSeconds(1)).Subscribe(x=>Console.WriteLine(x));
            //条件限流
            Observable.Interval(TimeSpan.FromSeconds(1)).Where(x=>x%2==0).Subscribe(x=>Console.WriteLine(x));


            //超时
            Observable.Interval(TimeSpan.FromSeconds(2)).Timeout(TimeSpan.FromSeconds(1)).Subscribe(x=>Console.WriteLine(x),ex=>Console.WriteLine($"超时 {ex.Message}"));
            

            Console.ReadLine();
        }
    }
}