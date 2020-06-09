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
        private string serviceName;

        public ServiceController(ILogger<ServiceHandler> logger, IServiceHandler handler)
        {
            this.logger = logger;
            this.handler = handler;

            serviceName = "VizConfigHub";
            handler.ServiceName = serviceName;
        }

        [HttpGet] // Attribute routing with Http verb attributes
        public async Task<string> PeekService()
        {
            var state = await handler.GetState();
            string response = $"Service {serviceName} is currently in state {state.ToString()}";
            logger.LogInformation(response);

            return response;
        }

        [HttpGet("start")] // Attribute routing with Http verb attributes
        public async Task<string> StartService()
        {            
            await handler.ServiceControlAsync(Enums.ServiceState.StartService); //, System.ServiceProcess.ServiceStartMode.Automatic
            return await PeekService();
        }

        [HttpGet("stop")]
        public async Task<string> StopService()
        {
            await handler.ServiceControlAsync(Enums.ServiceState.StopService);
            return await PeekService();
        }

    }
}