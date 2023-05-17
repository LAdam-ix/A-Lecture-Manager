using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ALectureManager.Encoder;

public partial class EncodeProcessView : UserControl
{
    public EncodeProcessView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}