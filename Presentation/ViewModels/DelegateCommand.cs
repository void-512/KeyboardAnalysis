using System.Windows.Input;

namespace KeyboardAnalysis.Presentation.ViewModels;

public sealed class DelegateCommand : ICommand
{
    private readonly Action _execute;
    public DelegateCommand(Action execute) => _execute = execute;
    public event EventHandler? CanExecuteChanged;
    public bool CanExecute(object? parameter) => true;
    public void Execute(object? parameter) => _execute();
    private void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}
