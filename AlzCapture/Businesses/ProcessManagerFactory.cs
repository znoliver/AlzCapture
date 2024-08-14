using System.Runtime.InteropServices;
using AlzCapture.Businesses.Interfaces;
using AlzCapture.Businesses.ProcessManager;

namespace AlzCapture.Businesses;

public static class ProcessManagerFactory
{
    public static IProcessManager Create() => RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
        ? new MacProcessManager()
        : RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? new WindowsProcessManager()
            : new LinuxProcessManager();
}