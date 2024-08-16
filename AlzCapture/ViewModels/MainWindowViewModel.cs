using AlzCapture.Models.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace AlzCapture.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty] private ViewModelBase _currentDataModel;

    public MainWindowViewModel()
    {
        this.CurrentDataModel = new ProcessListViewModel();

        WeakReferenceMessenger.Default.Register<ProcessMonitorMessage>(this,
            (r, m) => { this.CurrentDataModel = new ProcessMonitorViewModel(m.Value); });
    }
}