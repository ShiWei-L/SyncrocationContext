using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using syncrocationContextWeb.Models;

namespace syncrocationContextWeb.Controllers
{
    public class HomeController : Controller
    {
        public async Task<IActionResult> Index()
        {
           var cx = HttpContext;
            var c= Thread.CurrentThread.ManagedThreadId;
            await Task.Factory.StartNew(()=>{
                var id = Thread.CurrentThread.ManagedThreadId;
            });
           var dx =HttpContext;
            var d = Thread.CurrentThread.ManagedThreadId;

            await Task.Factory.StartNew(()=>{
                var id = Thread.CurrentThread.ManagedThreadId;
            });
            var x = Thread.CurrentThread.ManagedThreadId;
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
