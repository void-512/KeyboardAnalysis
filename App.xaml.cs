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

    protected override async void OnLaunched(LaunchActivatedEventArgs args)
    {
        IKeyEventRepository repository = new SqliteKeyEventRepository();
        IKeyboardCaptureService keyboardCapture = new WindowsKeyboardCaptureService();
        var recorder = new KeyActivityRecorder(keyboardCapture, repository);

        var viewModel = new MainViewModel(recorder);
        _window = new MainWindow(viewModel);
        _window.Closed += async (_, _) => await recorder.StopAsync();
        _window.Activate();

        try
        {
            await recorder.StartAsync();
            viewModel.SetCaptureStatus("Keyboard capture is active. Events save after 20 seconds of inactivity.");
        }
        catch (Exception)
        {
            viewModel.SetCaptureStatus("Keyboard capture could not be started.");
        }
    }
}
