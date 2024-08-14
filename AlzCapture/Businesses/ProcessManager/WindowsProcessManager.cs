using System.Diagnostics;
using AlzCapture.Businesses.Interfaces;

namespace AlzCapture.Businesses.ProcessManager;

public class WindowsProcessManager : IProcessManager
{
    public bool IsProcessRequest(int processId, string requestIp, string requestPort)
    {
        throw new System.NotImplementedException();
    }
}