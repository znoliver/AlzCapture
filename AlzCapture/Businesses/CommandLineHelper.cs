using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Avalonia.Collections;

namespace AlzCapture.Businesses;

public static class CommandLineHelper
{
    public static Task<(string output, string errors)> RunCommandAsync(string command)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return OsxRunCommandAsync(command);
        }

        throw new NotImplementedException();
    }

    private static async Task<(string output, string errors)> OsxRunCommandAsync(string command)
    {
        var psi = new ProcessStartInfo
        {
            FileName = "/bin/bash", // 使用bash解释器
            Arguments = $"-c \"{command}\"", // 命令行参数
            RedirectStandardOutput = true, // 重定向输出
            RedirectStandardError = true, // 重定向错误输出
            UseShellExecute = false, // 不使用系统外壳启动
            CreateNoWindow = true // 不创建新窗口
        };

        try
        {
            using var process = Process.Start(psi)!;
            // 读取标准输出
            var output = process.StandardOutput.ReadToEnd();
            // 读取错误输出
            var errors = process.StandardError.ReadToEnd();

            await process.WaitForExitAsync();

            return (output, errors);
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred:");
            Console.WriteLine(ex.Message);

            return ("", "");
        }
    }
}