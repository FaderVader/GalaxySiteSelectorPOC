using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GalaxyWeb.Tools;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GalaxyWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private ILogger<ServiceHandler> logger;
        public ServiceController(ILogger<ServiceHandler> logger)
        {
            this.logger = logger;
        }

        [HttpGet]
        public string PeekService()
        {
            string serviceName = "VizConfigHub";

            ServiceHandler handler = new ServiceHandler(serviceName, logger); //
            var state = handler.ServiceState;

            string response = $"Service {serviceName} is currently in state {state.ToString()}";

            return response;
        }
    }
}