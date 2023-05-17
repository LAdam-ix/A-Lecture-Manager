using System;
using System.IO;
using ALectureManager.Encoder;
using ALectureManager.Models;
using Newtonsoft.Json;
using CommunityToolkit.Mvvm.Input;


namespace ALectureManager.Main;

[Serializable]
public partial class SettingsViewModel : ViewModelBase
{
    public LibraryMangerSettings LibraryMangerSettings { get; set; }
    public EncoderSettingsViewModel EncoderSettings { get; set; }
    [ObservableProperty] private string _databaseConectionString;
    [JsonIgnore] public RelayCommand SaveSettings { get; }
    public string SavePath { get; init; } = "./../config.json";

    private SettingsViewModel(string saveFilePath) : this()
    {
        SavePath = saveFilePath;
        LibraryMangerSettings = new LibraryMangerSettings();
        EncoderSettings = new EncoderSettingsViewModel("/usr/bin/ffmpeg");
        DatabaseConectionString = "Data Source=./Database/AppDB.db";
    }

    [JsonConstructor]
    public SettingsViewModel()
    {
        
        SaveSettings = new RelayCommand(SaveExecute);
    }

    public static SettingsViewModel Load(string filePath)
    {
        SettingsViewModel? loadedSettings = null;
        if (File.Exists(filePath))
        {
            var json = File.ReadAllText(filePath);
            loadedSettings = JsonConvert.DeserializeObject<SettingsViewModel>(json);
        }

        return loadedSettings ?? new SettingsViewModel(filePath);
    }

    private void SaveExecute()
    {
        string json = JsonConvert.SerializeObject(this);
        File.WriteAllText(SavePath, json);
    }
}