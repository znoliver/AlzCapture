using AlzCapture.Models.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace AlzCapture.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty] private ViewModelBase _currentContentDataModel;


    public MainWindowViewModel()
    {
        this._currentContentDataModel = new ProcessListViewModel();

        WeakReferenceMessenger.Default.Register<ProcessMonitorMessage>(this,
            (r, m) => { this.CurrentContentDataModel = new ProcessMonitorViewModel(m.Value); });
    }
}