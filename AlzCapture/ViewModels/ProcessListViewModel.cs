using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using AlzCapture.Models.Messages;
using Avalonia.Metadata;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace AlzCapture.ViewModels;

public partial class ProcessListViewModel : ViewModelBase
{
    [ObservableProperty] private Process? _currentSelectedProcess;

    [ObservableProperty] private string _searchText = string.Empty;

    public ObservableCollection<Process> FilteredItems { get; }

    public ObservableCollection<Process> Processes { get; set; }

    public ProcessListViewModel()
    {
        Processes = new ObservableCollection<Process>(Process.GetProcesses()
            .Where(p => !string.IsNullOrWhiteSpace(p.ProcessName)).OrderBy(p => p.ProcessName));
        FilteredItems = new ObservableCollection<Process>(Processes);
    }

    [DependsOn(nameof(CurrentSelectedProcess))]
    public bool CanOpenNetMonitor() => CurrentSelectedProcess != null;

    public void OpenNetMonitor()
    {
        WeakReferenceMessenger.Default.Send(new ProcessMonitorMessage(CurrentSelectedProcess!.Id));
    }
    
    partial void OnSearchTextChanged(string value)
    {
        FilteredItems.Clear();
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            foreach (var process in Processes)
            {
                FilteredItems.Add(process);
            }
        }
        else
        {
            foreach (var process in Processes.Where(p =>
                         p.ProcessName.Contains(value, StringComparison.OrdinalIgnoreCase)))
            {
                FilteredItems.Add(process);
            }
        }
    }
}