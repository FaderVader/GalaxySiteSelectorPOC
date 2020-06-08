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
        private IServiceHandler handler;

        public ServiceController(ILogger<ServiceHandler> logger, IServiceHandler handler)
        {
            this.logger = logger;
            this.handler = handler;
        }

        [HttpGet]
        public async Task<string> PeekService()
        {
            string serviceName = "VizConfigHub";
            handler.ServiceName = serviceName;
            await handler.ServiceControlAsync(Enums.TargetServiceState.StartService); //, System.ServiceProcess.ServiceStartMode.Automatic

            var state = await handler.GetStatus();

            string response = $"Service {serviceName} is currently in state {state.ToString()}";
            logger.LogInformation(response);

            return response;
        }
    }
}