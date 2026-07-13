using KeyboardAnalysis.Application.Services;

namespace KeyboardAnalysis.Presentation.ViewModels;

/// <summary>Presentation state for the application shell.</summary>
public sealed class MainViewModel : ViewModelBase
{
    private readonly KeyActivityRecorder _recorder;
    private string _statusMessage = "Keyboard capture is ready to be configured.";

    public MainViewModel(KeyActivityRecorder recorder) => _recorder = recorder;

    public string StatusMessage
    {
        get => _statusMessage;
        private set
        {
            _statusMessage = value;
            OnPropertyChanged();
        }
    }
}
