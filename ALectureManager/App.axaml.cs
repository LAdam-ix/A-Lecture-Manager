using ALectureManager.Main;
using ALectureManager.Models;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using ALectureManager.Views;

namespace ALectureManager;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        base.OnFrameworkInitializationCompleted();
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var settings = SettingsViewModel.Load("./config.json");
            var database = new AppDbContext(settings);

            var mainWindow = new MainWindow();
            var mainWindowViewModel = new MainWindowViewModel(settings, database, mainWindow);

            mainWindow.DataContext = mainWindowViewModel;

            desktop.MainWindow = mainWindow;

        }
    }
}