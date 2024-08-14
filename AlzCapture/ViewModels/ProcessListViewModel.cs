using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

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
    
    private bool CanOpenNetMonitor() => CurrentSelectedProcess != null;

    [RelayCommand(CanExecute = nameof(CanOpenNetMonitor))]
    private void OpenNetMonitor()
    {
    }
}