using System.IO;
using ALectureManager.Encoder;
using ALectureManager.Models;
using Newtonsoft.Json;
using CommunityToolkit.Mvvm.Input;


namespace ALectureManager.Main;

[Serializable]
public partial class SettingsViewModel : ViewModelBase
{
    public LibraryMangerSettings LibraryMangerSettings { get; set; } = new LibraryMangerSettings();
    public EncoderSettingsViewModel EncoderSettings { get; set; } = new EncoderSettingsViewModel();
    [ObservableProperty] private string _databaseConectionString = "Data Source=./Database/AppDB.db";
    [JsonIgnore] public RelayCommand SaveSettings { get; }
    public string SavePath { get; init; } = "./../config.json";

    [JsonConstructor]
    public SettingsViewModel()
    {
        SaveSettings = new RelayCommand(SaveExecute);
    }
    private SettingsViewModel(string saveFilePath) : this()
    {
        SavePath = saveFilePath;
        EncoderSettings = new EncoderSettingsViewModel();
        LibraryMangerSettings = new LibraryMangerSettings();
        SetDefaults();
    }

    private void SetDefaults()
    {
        var defaultCodecs = new List<CodecOption>()
        {
            new CodecOption
            {
                Name = "Cuda for Lectures",
                VideoCodec = "libx265",
                AudioCodec = null,
                AdditionalArguments = "-crf 28"
            },
            new CodecOption
            {
                Name = "ssa720",
                VideoCodec = "libx265",
                AudioCodec = "aac",
                AdditionalArguments =
                    "-map 0:v -map 0:a -b:a 192k -color_primaries 1 -color_range 1 -color_trc 1 -colorspace 1 -crf 24.2 -map 0:s? -pix_fmt yuv420p -preset slow -profile:v main -vf smartblur=1.5:-0.35:-3.5:0.65:0.25:2.0,scale=1280:720:spline16+accurate_rnd+full_chroma_int -x265-params me=2:rd=4:subme=7:aq-mode=3:aq-strength=1:deblock=1,1:psy-rd=1:psy-rdoq=1:rdoq-level=2:merange=57:bframes=8:b-adapt=2:limit-sao=1:frame-threads=3:no-info=1 -y"
            },
            new CodecOption
            {
                Name = "ssa1080",
                VideoCodec = "libx265",
                AudioCodec = "aac",
                AdditionalArguments =
                    "-map 0:v -map 0:a -b:a 192k -color_primaries 1 -color_range 1 -color_trc 1 -colorspace 1 -crf 24.2 -map 0:s? -pix_fmt yuv420p -preset slow -profile:v main -vf smartblur=1.5:-0.35:-3.5:0.65:0.25:2.0,scale=1920:1080:spline16+accurate_rnd+full_chroma_int -x265-params me=2:rd=4:subme=7:aq-mode=3:aq-strength=1:deblock=1,1:psy-rd=1:psy-rdoq=1:rdoq-level=2:merange=57:bframes=8:b-adapt=2:limit-sao=1:frame-threads=3:no-info=1 -y"
            },
        };
        foreach (var codec in defaultCodecs)
        {
            EncoderSettings.CodecOptions.Add(codec);
        }
        EncoderSettings.DefaultCodecOption = EncoderSettings.CodecOptions[0];

        //here would be default setting for library manager if it had any
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