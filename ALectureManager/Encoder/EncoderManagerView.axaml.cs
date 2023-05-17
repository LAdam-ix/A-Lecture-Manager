using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ALectureManager.Encoder;

public partial class EncoderManagerView : UserControl
{
    public EncoderManagerView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}