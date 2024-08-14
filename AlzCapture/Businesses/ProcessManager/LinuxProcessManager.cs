using System.Diagnostics;
using AlzCapture.Businesses.Interfaces;

namespace AlzCapture.Businesses.ProcessManager;

internal class LinuxProcessManager : IProcessManager
{
    public bool IsProcessRequest(int processId, string requestIp, string requestPort)
    {
        throw new System.NotImplementedException();
    }
}