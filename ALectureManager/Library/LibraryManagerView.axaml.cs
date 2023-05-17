using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ALectureManager.Library;

public partial class LibraryManagerView : UserControl
{
    public LibraryManagerView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}