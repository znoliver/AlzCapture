using CommunityToolkit.Mvvm.ComponentModel;

namespace AlzCapture.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty] private ViewModelBase _currentDataModel;
    public MainWindowViewModel()
    {
        this.CurrentDataModel = new ProcessMonitorViewModel(null);
    }
}