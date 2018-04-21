using System;
using System.Net;
using System.Threading.Tasks;

namespace combine
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine((await new Program().ApmToTap()).ContentLength);
        }


        async Task<WebResponse> ApmToTap()
        {
           var client =  WebRequest.Create("http://www.baidu.com");
           return await Task<WebResponse>.Factory.FromAsync(client.BeginGetResponse,client.EndGetResponse,null);
        }

    }
}
