using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using StorageCore.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace StorageCore.Controllers
{
    public class HomeController1 : Controller
    {
        private readonly ILogger<HomeController1> _logger;

        public readonly IWebHostEnvironment _webHostEnviroment;

        public string webRootPath;

        public HomeController1(ILogger<HomeController1> logger, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _webHostEnviroment = webHostEnvironment;
            webRootPath = _webHostEnviroment.WebRootPath;
        }

        //Muestra la vista del Index
        public IActionResult Index()
        {
            return View();
        }

        //Muestra la vista de Login
        public IActionResult Login()
        {
            return View();
        }

        //Muestra la vista de privacidad
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]

        //Muestra la vista en caso de un ERROR
        public IActionResult Error() {
            return View(new ErrorView { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier }); 
        }
    }
}
