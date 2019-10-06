using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Shared;
using mischievousripley_mvc.Models;

namespace mischievousripley_mvc.Controllers
{
    public class HomeController : Controller
    {
        // e.g. HostName=youriothub.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=****
        private const string connectionString = "HostName=Group8.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=81BJOU1EJXowRuNZ8q9YqB+TyYcgTzJegNK1gQOgJs8=";
        // e.g. myDevice
        private const string deviceId = "2DDF6E32A7DEA4ECD617B1075386F8F6B1C981708E3B69762724E99A8E9AD31B3DC7B2334B5EA93140D5F86C704CC4C2D2A7348FF94C27FC800007D7D3901F43";

        private readonly RegistryManager registryManager;
        private readonly Twin deviceTwin;

        public HomeController()
        {
            registryManager = RegistryManager.CreateFromConnectionString(connectionString);
            deviceTwin = registryManager.GetTwinAsync(deviceId).Result;
        }

        public IActionResult Index()
        {
            OvenState state = new OvenState()
            {
                StatusDesired = "n/a",
                StatusReported = "n/a",
                TempVal = "60", 
                HumidityVal = "30", 
                TheData = "0, 32, 35"
            };
            try
            {
                bool status = deviceTwin.Properties.Desired["StatusLED"]
                    .ToString()
                    .Contains("true", StringComparison.OrdinalIgnoreCase);
                state.TempVal = deviceTwin.Properties.Desired["TemperatureStatus"]["value"];
                state.HumidityVal = deviceTwin.Properties.Desired["Humidity"]["value"];
                state.TheData = deviceTwin.Properties.Desired["TheData"]["value"];
                state.StatusDesired = status ? "On" : "Off";
                state.TheData = deviceTwin.Properties.Desired["StatusLED"]["value"]
                    .ToString();
                    
            }
            catch (ArgumentException ex)
            {
                Debug.WriteLine(ex);
            }

            try
            {
                bool status = deviceTwin.Properties.Reported["StatusLED"]
                    .ToString()
                    .Contains("true", StringComparison.OrdinalIgnoreCase);
                
                state.StatusReported = status ? "On" : "Off";
            }
            catch (ArgumentException ex)
            {
                Debug.WriteLine(ex);
            }

            return View(state);
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

        public IActionResult TurnOvenOn()
        {
            RequestOvenState(true);
            return Redirect("/");
        }

        public IActionResult TurnOvenOff()
        {
            RequestOvenState(false);
            return Redirect("/");
        }

        private void RequestOvenState(bool isOn)
        {
            deviceTwin.Properties.Desired["StatusLED"]["value"] = isOn;
            registryManager.UpdateTwins2Async(new[] { deviceTwin }, true).Wait();
        }
    }
}
