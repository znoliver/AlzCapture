using System.Diagnostics;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace AlzCapture.Models.Messages;

public class ProcessMonitorMessage : ValueChangedMessage<int>
{
    public ProcessMonitorMessage(int value) : base(value)
    {
    }
}