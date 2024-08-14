namespace AlzCapture.Businesses.Interfaces;

public interface IProcessManager
{
    bool IsProcessRequest(int processId, string requestIp, string requestPort);
}