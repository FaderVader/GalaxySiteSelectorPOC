using System.ServiceProcess;
using System.Threading.Tasks;
using static GalaxyWeb.Tools.Enums;

namespace GalaxyWeb.Tools
{
    public interface IServiceHandler
    {
        string ServiceName { set;  get; }

        //TargetServiceState ServiceState { get; }

        Task<TargetServiceState> GetStatus();

        Task ServiceControlAsync(TargetServiceState state, ServiceStartMode startupType);

        Task ServiceControlAsync(TargetServiceState state);

        Task SetStartupTypeAsync(ServiceStartMode startUpType);

    }
}