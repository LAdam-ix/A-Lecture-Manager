using ALectureManager.Models;
using ALectureManager.Encoder;
using ALectureManager.Library;
using ALectureManager.Views;

namespace ALectureManager.Main;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty] private EncoderManagerViewModel _encoderManager;
    [ObservableProperty] private LibraryManagerViewModel _libraryManager;
    [ObservableProperty] private SettingsViewModel _settings;
    private MainWindow _mainWindow;

    public MainWindowViewModel(SettingsViewModel settings, AppDbContext db, MainWindow mainWindow)
    {
        _settings = settings;
        _mainWindow = mainWindow;
        
        EncoderManager = new EncoderManagerViewModel(settings.EncoderSettings, db);
        LibraryManager = new LibraryManagerViewModel(settings, EncoderManager, db);
        _mainWindow.Closing += EncoderManager.OnClosing;
    }
}