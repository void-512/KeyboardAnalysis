using KeyboardAnalysis.Presentation.ViewModels;
using Microsoft.UI.Xaml;

namespace KeyboardAnalysis;

/// <summary>The application's main shell.</summary>
public sealed partial class MainWindow : Window
{
    public MainViewModel ViewModel { get; }

    public MainWindow(MainViewModel viewModel)
    {
        ViewModel = viewModel;
        InitializeComponent();
    }
}
