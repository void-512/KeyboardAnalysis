using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace KeyboardAnalysis.Presentation.ViewModels;

/// <summary>Base class for MVVM view models.</summary>
public abstract class ViewModelBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
