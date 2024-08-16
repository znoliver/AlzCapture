using System.Threading.Tasks;
using AlzCapture.Businesses.Interfaces;

namespace AlzCapture.Businesses.ProcessManager;

public class MacProcessManager : IProcessManager
{
    public async Task<bool> IsProcessRequestAsync(int processId, string requestIp, string requestPort)
    {
        var command = $"lsof -Pan -p {processId} -i @{requestIp}:{requestPort}";
        var (output, errors) = await CommandLineHelper.RunCommandAsync(command);
        return !string.IsNullOrWhiteSpace(output);
    }
}