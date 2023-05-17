using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using ALectureManager.Encoder;
using ALectureManager.Main;
using ALectureManager.Models;
using System.Linq;

namespace ALectureManager.Library;

public partial class LibraryManagerViewModel : ViewModelBase
{
    [ObservableProperty] private bool _isSaving = false;
    public RelayCommand AddNew { get; }
    public RelayCommand<LibraryEntry?> DeleteEntry { get; }
    public RelayCommand Scan { get; }
    private AppDbContext _db;
    public ObservableCollection<LibraryEntry> Enteries { get; }
    private EncoderManagerViewModel _encoderManager;
    private SettingsViewModel _settings;

    public LibraryManagerViewModel(SettingsViewModel settings, EncoderManagerViewModel encoderManager,
        AppDbContext database)
    {
        _settings = settings;

        _encoderManager = encoderManager;
        _db = database;
        Enteries = new ObservableCollection<LibraryEntry>();
        foreach (var entry in database.LibraryEntries)
        {
            Enteries.Add(entry);
        }

        Scan = new RelayCommand(ScanExecute);
        AddNew = new RelayCommand(AddNewExecute);
        DeleteEntry = new RelayCommand<LibraryEntry?>(DeleteEntryExecute);
        foreach (var data in database.EncoderProcessesData)
        {
        }
    }

    private void ScanExecute()
    {
        var newEncodeData = new List<EncoderProcessData>();
        foreach (var entry in Enteries)
        {
            newEncodeData.AddRange(entry.Scan());
        }
        CreateEncodeProcess(newEncodeData);
    }



    private async void AddNewExecute()
    {
        var directoryPath = await Utils.OpenFileExplorer(true);
        if (!string.IsNullOrEmpty(directoryPath))
        {
            var newEntry = new LibraryEntry(directoryPath);
            newEntry.EnteryDefaultCodec = _settings.EncoderSettings.DefaultCodecOption;
            Enteries.Add(newEntry);
            await _db.LibraryEntries.AddAsync(newEntry);
            await _db.SaveChangesAsync();
            CreateEncodeProcess(newEntry.Scan());
        }
    }
    private void CreateEncodeProcess(List<EncoderProcessData> EncodeData)
    {

        var allreadyIn = _encoderManager.EncodeProcesses.Select(x => x.Data).ToList();

        foreach (var data in EncodeData)
        {
            if (allreadyIn.Any(x => (x.InputPath == data.InputPath && x.OutputPath == data.OutputPath)))
            {
                continue;
            }
            _encoderManager.AddNewWData(data);
        }
    }

    private async void DeleteEntryExecute(LibraryEntry? entry)
    {
        if (entry != null)
        {
            Enteries.Remove(entry);
            _db.LibraryEntries.Remove(entry);
            await _db.SaveChangesAsync();
        }
    }
}