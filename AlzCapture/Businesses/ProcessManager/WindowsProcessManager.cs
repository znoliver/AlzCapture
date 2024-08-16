using System.Diagnostics;
using System.Threading.Tasks;
using AlzCapture.Businesses.Interfaces;

namespace AlzCapture.Businesses.ProcessManager;

public class WindowsProcessManager : IProcessManager
{
    public Task<bool> IsProcessRequestAsync(int processId, string requestIp, string requestPort)
    {
        throw new System.NotImplementedException();
    }
}