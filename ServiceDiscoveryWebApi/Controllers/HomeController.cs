using Consul;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ServiceDiscoveryWebApi.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;


namespace ServiceDiscoveryWebApi.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IOptions<ApplicationSettings> _appSettings;

        public HomeController(ILogger<HomeController> logger, IOptions<ApplicationSettings> appSettings)
        {
            _logger = logger;
            _appSettings = appSettings;
        }

        public IActionResult Index()
        {
            IList<Uri> serverUrls = new List<Uri>();
            var consuleClient = new ConsulClient(c => c.Address = new Uri(_appSettings.Value.ConsulHost));
            var services = consuleClient.Agent.Services().Result.Response;
            foreach (var service in services)
            {
                var serviceUri = new Uri($"{service.Value.Address}:{service.Value.Port}");
                serverUrls.Add(serviceUri);
            }

            return View(serverUrls);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
