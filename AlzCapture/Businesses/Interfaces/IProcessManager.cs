using System.Threading.Tasks;

namespace AlzCapture.Businesses.Interfaces;

public interface IProcessManager
{
    Task<bool> IsProcessRequestAsync(int processId, string requestIp, string requestPort);
}