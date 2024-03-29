using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ALectureManager.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
#if DEBUG
        this.AttachDevTools();
#endif
        AvaloniaXamlLoader.Load(this);
    }
}