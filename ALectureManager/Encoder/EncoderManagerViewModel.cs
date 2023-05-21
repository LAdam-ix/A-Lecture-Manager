using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ALectureManager.Models;
using ALectureManager.Views;

using CommunityToolkit.Mvvm.Input;
using HanumanInstitute.FFmpeg;
using MessageBox.Avalonia.Enums;
using Microsoft.Extensions.Options;



namespace ALectureManager.Encoder;

public partial class EncoderManagerViewModel : ViewModelBase
{
    [ObservableProperty] private bool _isSaving;
    [ObservableProperty] private bool _allIsRunning;
    [ObservableProperty] private bool _isInitialized = true;
    public ObservableCollection<EncodeProcessViewModel> EncodeProcesses { get; set; }

    public RelayCommand AddNew { get; }
    public RelayCommand SaveProcesses { get; }
    public RelayCommand RunAll { get; }

    private List<Action<EncodeProcessViewModel>> atDelete;

    // maybe will be public in future for more functionality
    private readonly EncoderSettingsViewModel _settings;
    private readonly AppDbContext _db;
    private IMediaEncoder _encoder;
    private TaskFactory _factory = new TaskFactory();
    private readonly SemaphoreSlim _semaphore;

    public EncoderManagerViewModel(EncoderSettingsViewModel settings, AppDbContext database)
    {
        _settings = settings;
        _db = database;
        atDelete = new List<Action<EncodeProcessViewModel>>()
        {
            OnProcessDeleteRequested,
        };

        if (!File.Exists(_settings.FFMpegPath))
        {
            Utils.GetInfoBox("Error", "FFmpeg not found, please install it and set the path in settings");
            //DUMMY change to inicialize even withou ffmpeg
            IsInitialized = false;
        }

        var options = Options.Create(new AppPaths()
        {
            FFmpeg = _settings.FFMpegPath
        });


        var service = new EncoderService(new ProcessManager(options));
        _encoder = service.GetMediaEncoder(_factory);

        _semaphore = new SemaphoreSlim(_settings.MaxParallel);

        EncodeProcesses = new ObservableCollection<EncodeProcessViewModel>();
        foreach (var data in database.EncoderProcessesData)
        {
            EncodeProcesses.Add(new EncodeProcessViewModel(_settings, _encoder, atDelete, data));
        }


        AddNew = new RelayCommand(AddNewExecute);
        SaveProcesses = new RelayCommand(SaveCommandExecute);
        RunAll = new RelayCommand(RunAllExecute);

        // // TESTING DATA LEAVING IT HERE
        // var DUMMY = new EncoderProcessData()
        // {
        //     InputPath = "/home/ch_x/Downloads/redditsave.com_one_of_the_coolest_steins_gate_edits-8bqejnx6yfo71.mp4"
        // };
        // AddNewWData(DUMMY);
        // AddNewExecute();
    }

    // this return value is for future reference to LibraryManager
    public EncodeProcessViewModel AddNewWData(EncoderProcessData data)
    {
        var process = new EncodeProcessViewModel(_settings, _encoder, atDelete, data);
        EncodeProcesses.Insert(0, process);
        return process;
    }


    private void AddNewExecute()
    {
        AddNewWData(new EncoderProcessData());
    }

    private async void SaveCommandExecute()
    {
        IsSaving = true;
        await _db.ReplaceEncodeProcesses(EncodeProcesses.Select(x => x.Data).ToList());
        IsSaving = false;
    }

    public async void RunAllExecute()
    {

        // it will do only the ones that are alreeady in list, newly added will be ignored
        var counter = _settings.MaxParallel;
        var encodeProcesses = EncodeProcesses.ToList();
        var tasks = new Queue<Task>();
        AllIsRunning = true;
        foreach (var encodeProcess in encodeProcesses)
        {

            if (counter == 0)
            {
                await tasks.Dequeue();
                counter++;
            }
            if (encodeProcess.IsRunning) continue;
            encodeProcess.Start.Execute(null);
            // this is to so there a litlle time to inicialize ffmpeg task
            // and so that it wont slip pass this and run multiple at once
            await Task.Delay(250);
            if (encodeProcess.EncodingTask != null)
            {
                tasks.Enqueue(encodeProcess.EncodingTask);
                counter--;
            }

        };
        AllIsRunning = false;
    }

    public async void OnClosing(object? sender, CancelEventArgs e)
    {
        if (EncodeProcesses.Any(x => x.IsRunning))
        {

            e.Cancel = true;
            if (await Utils.GetConfirmBox(
                "Warning",
                "There are still some encoding in progress. Do you still wish to exit?",
                ButtonEnum.YesNo
            ) == ButtonResult.Yes)
            {
                EncodeProcesses.ToList().ForEach(x => x.Pause());
                // it need to be done like this because there bug in awalonia that doesnt allow await in closing event
                if (sender == null) return;
                var mainWindow = (MainWindow)sender;
                mainWindow.Closing -= OnClosing;
                mainWindow.Close();
            }
        }
    }

    private void OnProcessDeleteRequested(EncodeProcessViewModel viewModel)
    {
        EncodeProcesses.Remove(viewModel);
    }

}