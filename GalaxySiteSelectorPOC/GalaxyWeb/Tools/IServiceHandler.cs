using System.ServiceProcess;
using System.Threading.Tasks;
using static GalaxyWeb.Tools.Enums;

namespace GalaxyWeb.Tools
{
    public interface IServiceHandler
    {
        string ServiceName { set;  get; }

        Task<ServiceState> GetState();

        Task ServiceControlAsync(ServiceState state, ServiceStartMode startupType);

        Task ServiceControlAsync(ServiceState state);

        Task SetStartupTypeAsync(ServiceStartMode startUpType);

    }
}