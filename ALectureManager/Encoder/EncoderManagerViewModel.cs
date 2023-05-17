using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
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
    public ObservableCollection<EncodeProcessViewModel> EncodeProcesses { get; set; }

    public RelayCommand AddNew { get; }
    public RelayCommand SaveProcesses { get; }

    private List<Action<EncodeProcessViewModel>> atDelete;

    // maybe will be public in future for more functionality
    private readonly EncoderSettingsViewModel _settings;
    private readonly AppDbContext _db;
    private IMediaEncoder _encoder;
    private TaskFactory _factory = new TaskFactory();

    public EncoderManagerViewModel(EncoderSettingsViewModel settings, AppDbContext database)
    {
        _settings = settings;
        _db = database;
        atDelete = new List<Action<EncodeProcessViewModel>>()
        {
            OnProcessDeleteRequested,
        };

        var options = Options.Create(new AppPaths()
        {
            FFmpeg = settings.FFMpegPath
        });

        var service = new EncoderService(new ProcessManager(options));
        _encoder = service.GetMediaEncoder(_factory);


        EncodeProcesses = new ObservableCollection<EncodeProcessViewModel>();
        foreach (var data in database.EncoderProcessesData)
        {
            EncodeProcesses.Add(new EncodeProcessViewModel(_settings, _encoder, atDelete, data));
        }


        AddNew = new RelayCommand(AddNewExecute);
        SaveProcesses = new RelayCommand(SaveCommandExecute);

        // // TESTING DATA LEAVING IT HERE
        //     var x = new EncoderProcessData()
        //     {
        //         InputPath = "/home/ch_x/Downloads/redditsave.com_one_of_the_coolest_steins_gate_edits-8bqejnx6yfo71.mp4"
        //     };
        //     AddNewWData(x);
        //     AddNewExecute();
    }

    // this return value is for future reference to LibraryManager
    public EncodeProcessViewModel AddNewWData(EncoderProcessData data)
    {
        var process = new EncodeProcessViewModel(_settings, _encoder, atDelete, data);
        EncodeProcesses.Insert(0,process);
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

    public async void OnClosing(object? sender, CancelEventArgs e)
    {
        if (EncodeProcesses.Any(x => x.IsRunning))
        {

            e.Cancel = true;
            if (await Utils.GetInfoBox(
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