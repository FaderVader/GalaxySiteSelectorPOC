using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Reflection;
using System.ServiceProcess;
using System.Threading.Tasks;
using static GalaxyWeb.Tools.Enums;

namespace GalaxyWeb.Tools
{
    public class ServiceHandler : IServiceHandler
    {
        private TargetServiceState _processMode;
        private string _serviceName;
        private ServiceController serviceController;
        private ILogger logger;
        private bool handlerIsValid;

        public string ServiceName
        {
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    _serviceName = value;
                    handlerIsValid = true;
                }
            }
            get
            {
                return _serviceName;
            }
        }

        //public TargetServiceState ServiceState
        //{
        //    get { return GetStatus(); }
        //}

        public async Task<TargetServiceState> GetStatus()
        {
            TargetServiceState status = TargetServiceState.NotFound;
            if (!handlerIsValid) return status;

            try
            {
                using (serviceController = new ServiceController(_serviceName))
                {
                    switch (serviceController.Status)
                    {
                        case ServiceControllerStatus.Running:
                            status = TargetServiceState.StartService;
                            break;

                        case ServiceControllerStatus.StartPending:
                            status = TargetServiceState.StartService;
                            break;

                        case ServiceControllerStatus.Stopped:
                            status = TargetServiceState.StopService;
                            break;

                        case ServiceControllerStatus.StopPending:
                            status = TargetServiceState.StopService;
                            break;

                    }
                    logger.LogInformation("Found service {serviceName} in state:{serviceState}", _serviceName, status);
                    return status;
                }
            }
            catch (Exception e)
            {
                string method = MethodBase.GetCurrentMethod().ReflectedType.FullName + "." + MethodBase.GetCurrentMethod().Name;
                logger.LogError(method, e.Message, "Couldn't find service: ", _serviceName);

                return TargetServiceState.NotFound;
            }
        }

        public ServiceHandler(ILogger<ServiceHandler> logger)
        {            
            this.logger = logger;
            handlerIsValid = false;
        }

        // Overloaded method that takes both requested running-state and startup-type
        public async Task ServiceControlAsync(TargetServiceState state, ServiceStartMode startupType)
        {
            if (!handlerIsValid) return;
            _processMode = state;

            switch (state)
            {
                case TargetServiceState.StartService:
                    await StartServiceAsync(startupType);
                    break;

                case TargetServiceState.StopService:
                    await StopServiceAsync();
                    break;

                case TargetServiceState.RestartService:
                    await RestartServiceAsync();
                    break;
            }
        }


        // Overloaded method that takes only requested servicestate as parameter
        public async Task ServiceControlAsync(TargetServiceState state)
        {
            if (!handlerIsValid) return;
            ServiceStartMode currentStartupType;

            using (ServiceController sc = new ServiceController(_serviceName))
            {
                currentStartupType = sc.StartType;
            }

            await this.ServiceControlAsync(state, currentStartupType);
        }

        public async Task SetStartupTypeAsync(ServiceStartMode startUpType)
        {
            if (!handlerIsValid) return;
            try
            {
                string query = "SELECT * FROM Win32_Service";
                string name;

                ManagementBaseObject inParams;
                ManagementBaseObject outParams;
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(query); //scope, new ObjectQuery(query)
                ManagementObjectCollection objCol = searcher.Get();

                foreach (ManagementObject obj in objCol)
                {
                    name = obj.Properties["DisplayName"].Value.ToString().ToLower();
                    if (name == _serviceName.ToLower())
                    {
                        inParams = obj.GetMethodParameters("ChangeStartMode");
                        inParams["startmode"] = startUpType.ToString();
                        outParams = await Task.Run(() => obj.InvokeMethod("ChangeStartMode", inParams, null));

                        string result = await Task.Run(() => obj.Properties["StartMode"].Value.ToString().Trim()); // this line just for verifying result of operation
                        logger.LogInformation("Detected startUpType for service: " + name + ": " + result);
                        return;
                    }
                }
            }
            catch (Exception e)
            {
                string method = MethodBase.GetCurrentMethod().ReflectedType.FullName + "." + MethodBase.GetCurrentMethod().Name;
                logger.LogError(method, e.Message, "Couldn't set startUp mode for service: ", _serviceName + " " + startUpType.ToString());
            }
        }

        private async Task RestartServiceAsync()
        {
            if (!handlerIsValid) return;
            try
            {
                using (ServiceController serviceControllerRestart = new ServiceController(_serviceName))
                {
                    await StopServiceAsync();
                    await Task.Run(() => serviceControllerRestart.WaitForStatus(ServiceControllerStatus.Stopped));
                    await StartServiceAsync(ServiceStartMode.Automatic);
                    await Task.Run(() => serviceControllerRestart.WaitForStatus(ServiceControllerStatus.Running));
                }
            }
            catch (Exception e)
            {
                string method = MethodBase.GetCurrentMethod().ReflectedType.FullName + "." + MethodBase.GetCurrentMethod().Name;
                logger.LogError(method, e.Message, "Couldn't find service: ", _serviceName);
            }
        }

        private async Task StartServiceAsync(ServiceStartMode startupType)
        {
            if (!handlerIsValid) return;
            try
            {
                using (serviceController = new ServiceController(_serviceName))
                {
                    //serviceController.WaitForStatus();
                    if (serviceController.Status == ServiceControllerStatus.Stopped)  // || serviceController.Status == ServiceControllerStatus.StopPending
                    {

                        await SetStartupTypeAsync(startupType);
                        await Task.Run(() =>
                        {
                            serviceController.Start();
                            serviceController.WaitForStatus(ServiceControllerStatus.Running);
                            ServiceControllerStatus message = serviceController.Status;
                            logger.LogInformation("Service name and status: " + _serviceName + ": " + message);
                        });
                    }
                }
            }
            catch (Exception e)
            {
                string method = MethodBase.GetCurrentMethod().ReflectedType.FullName + "." + MethodBase.GetCurrentMethod().Name;
                logger.LogError(method, e.Message, "Couldn't find service: ", _serviceName);
            }
        }

        private async Task StopServiceAsync()
        {
            if (!handlerIsValid) return;
            try
            {
                using (serviceController = new ServiceController(_serviceName))
                {
                    if (serviceController.Status == ServiceControllerStatus.Running || serviceController.Status == ServiceControllerStatus.StartPending)
                    {
                        await Task.Run(() =>
                        {
                            serviceController.Stop();
                            serviceController.WaitForStatus(ServiceControllerStatus.Stopped);
                        });

                        var message = serviceController.Status;
                        logger.LogInformation("Service name and status: " + _serviceName + " : " + message);
                    }
                }
            }
            catch (Exception e)
            {
                string method = MethodBase.GetCurrentMethod().ReflectedType.FullName + "." + MethodBase.GetCurrentMethod().Name;
                logger.LogError(method, e.Message, "Couldn't find service: ", _serviceName);
            }
        }

      
    }
}
