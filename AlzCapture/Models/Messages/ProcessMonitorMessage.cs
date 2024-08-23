using System.Diagnostics;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace AlzCapture.Models.Messages;

public class ProcessMonitorMessage(ProcessMonitorModel value) : ValueChangedMessage<ProcessMonitorModel>(value);