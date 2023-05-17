using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ALectureManager.Encoder;

public partial class EncoderSettingsView : UserControl
{
    public EncoderSettingsView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}