using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using HUS.Model;

namespace HUS.ViewModel;

public class MainViewModel : ViewModelBase, INotifyPropertyChanged
{
    public MainViewModel()
    {
        // Initialize with DashboardViewModel
        CurrentViewModel = new DashboardViewModel(new ResultManager());
    }

    private ViewModelBase _currentViewModel;
    public ViewModelBase CurrentViewModel
    {
        get => _currentViewModel;
        set
        {
            _currentViewModel = value;
            OnPropertyChanged(nameof(CurrentViewModel)); // Implement INotifyPropertyChanged
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}