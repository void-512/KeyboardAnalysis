using Microsoft.UI.Xaml;
using KeyboardAnalysis.Application.Interfaces;
using KeyboardAnalysis.Application.Services;
using KeyboardAnalysis.Infrastructure.KeyboardCapture;
using KeyboardAnalysis.Infrastructure.Persistence;
using KeyboardAnalysis.Presentation.ViewModels;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace KeyboardAnalysis;

/// <summary>Provides application-specific behavior and composes application services.</summary>
public partial class App : Microsoft.UI.Xaml.Application
{
    private Window? _window;

    public App()
    {
        InitializeComponent();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        IKeyEventRepository repository = new SqliteKeyEventRepository();
        IKeyboardCaptureService keyboardCapture = new WindowsKeyboardCaptureService();
        var recorder = new KeyActivityRecorder(keyboardCapture, repository);

        _window = new MainWindow(new MainViewModel(recorder));
        _window.Activate();
    }
}
