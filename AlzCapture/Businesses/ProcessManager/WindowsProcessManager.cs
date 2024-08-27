using System;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AlzCapture.Businesses.Interfaces;

namespace AlzCapture.Businesses.ProcessManager;

public class WindowsProcessManager : IProcessManager
{
    [DllImport("iphlpapi.dll", SetLastError = true)]
    private static extern uint GetExtendedTcpTable(IntPtr pTcpTable, ref int dwOutBufLen, bool sort, int ipVersion, TcpTableClass tblClass, int reserved);

    [StructLayout(LayoutKind.Sequential)]
    private struct MIB_TCPROW_OWNER_PID
    {
        public uint state;
        public uint localAddr;
        public uint localPort;
        public uint remoteAddr;
        public uint remotePort;
        public uint owningPid;
    }

    private enum TcpTableClass
    {
        TCP_TABLE_BASIC_LISTENER,
        TCP_TABLE_BASIC_CONNECTIONS,
        TCP_TABLE_BASIC_ALL,
        TCP_TABLE_OWNER_PID_LISTENER,
        TCP_TABLE_OWNER_PID_CONNECTIONS,
        TCP_TABLE_OWNER_PID_ALL,
        TCP_TABLE_OWNER_MODULE_LISTENER,
        TCP_TABLE_OWNER_MODULE_CONNECTIONS,
        TCP_TABLE_OWNER_MODULE_ALL
    }
    
    public Task<bool> IsProcessRequestAsync(int processId, string requestIp, string requestPort)
    {
        int bufferSize = 0;
        IntPtr tcpTable = IntPtr.Zero;

        try
        {
            uint result = GetExtendedTcpTable(IntPtr.Zero, ref bufferSize, true, 2 /*AF_INET*/, TcpTableClass.TCP_TABLE_OWNER_PID_ALL, 0);

            tcpTable = Marshal.AllocHGlobal(bufferSize);
            result = GetExtendedTcpTable(tcpTable, ref bufferSize, true, 2 /*AF_INET*/, TcpTableClass.TCP_TABLE_OWNER_PID_ALL, 0);

            if (result == 0)
            {
                int rowSize = Marshal.SizeOf(typeof(MIB_TCPROW_OWNER_PID));
                int tableSize = Marshal.ReadInt32(tcpTable);

                for (int i = 0; i < tableSize; i++)
                {
                    IntPtr rowPtr = IntPtr.Add(tcpTable, 4 + i * rowSize);
                    MIB_TCPROW_OWNER_PID tcpRow = Marshal.PtrToStructure<MIB_TCPROW_OWNER_PID>(rowPtr);

                    IPAddress address = new IPAddress(tcpRow.localAddr);
                    int localPort = (ushort)IPAddress.NetworkToHostOrder((short)tcpRow.localPort);
                    int remotePort =(ushort) IPAddress.NetworkToHostOrder((short)tcpRow.remotePort);

                    // Console.WriteLine($"进程{tcpRow.owningPid}在地址{address.ToString()}以及端口{localPort}上发起请求");
                    // Console.WriteLine($"监控进程{processId}在地址{requestIp}以及端口{requestPort}上发起的请求");
                    
                    // 比较 IP 地址、端口和进程 ID
                    if (address.ToString() == requestIp && localPort.ToString() == requestPort && tcpRow.owningPid == processId)
                    {
                        return Task.FromResult(true);
                    }
                }
            }
        }
        finally
        {
            if (tcpTable != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(tcpTable);
            }
        }

        return Task.FromResult(false);
    }
}