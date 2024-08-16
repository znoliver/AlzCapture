using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using AlzCapture.Models.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace AlzCapture.ViewModels;

public partial class ProcessListViewModel : ViewModelBase
{
    [ObservableProperty] [NotifyCanExecuteChangedFor(nameof(OpenNetMonitorCommand))]
    private Process? _currentSelectedProcess;

    public ObservableCollection<Process> Processes { get; set; }

    public ProcessListViewModel()
    {
        Processes = new ObservableCollection<Process>(Process.GetProcesses()
            .Where(p => !string.IsNullOrWhiteSpace(p.ProcessName)).OrderBy(p => p.ProcessName));
    }

    public bool CanOpenNetMonitor() => CurrentSelectedProcess != null;

    [RelayCommand(CanExecute = nameof(CanOpenNetMonitor))]
    public void OpenNetMonitor()
    {
        WeakReferenceMessenger.Default.Send(new ProcessMonitorMessage(CurrentSelectedProcess.Id!));
    }
}