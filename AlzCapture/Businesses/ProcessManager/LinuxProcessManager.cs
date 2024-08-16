using System.Diagnostics;
using System.Threading.Tasks;
using AlzCapture.Businesses.Interfaces;

namespace AlzCapture.Businesses.ProcessManager;

internal class LinuxProcessManager : IProcessManager
{
    public Task<bool> IsProcessRequestAsync(int processId, string requestIp, string requestPort)
    {
        throw new System.NotImplementedException();
    }
}