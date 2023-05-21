using Newtonsoft.Json;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;


namespace ALectureManager.Encoder;

[Serializable]
public partial class EncoderSettingsViewModel : ViewModelBase
{
    [ObservableProperty] private string _fFMpegPath = "/usr/bin/ffmpeg";
    [ObservableProperty] private string _newName = string.Empty;
    [ObservableProperty] private int _maxParallel = 1;
    [ObservableProperty] private string _newVideoCodec = string.Empty;
    [ObservableProperty] private string _newAudioCodec = string.Empty;
    [ObservableProperty] private string _newArguments = string.Empty;
    [ObservableProperty] private CodecOption? _defaultCodecOption; 
    public ObservableCollection<CodecOption> CodecOptions { get; set; } = new ObservableCollection<CodecOption>();
    public RelayCommand AddCodecOption { get; }
    public RelayCommand<CodecOption> DeleteCodecOption { get; } 


    [JsonConstructor]
    public EncoderSettingsViewModel()
    {
        AddCodecOption = new RelayCommand(AddCodecOptionExecute);
        DeleteCodecOption = new RelayCommand<CodecOption>(DefaultCodecOptionExecute);
    }


    private void AddCodecOptionExecute()
    {
        // Create a new CodecOptionViewModel and add it to the collection
        try
        {
            var codecOption = new CodecOption
            {
                Name = NewName,
                AudioCodec = NewAudioCodec == "" ? null : NewAudioCodec,
                VideoCodec = NewVideoCodec == "" ? null : NewVideoCodec,
                AdditionalArguments = NewArguments == "" ? null : NewArguments
            };
            if (CodecOptions.Select(x => x.Name).Contains(codecOption.Name))
            {
                throw new InvalidOperationException("Name already exists");
            }
            CodecOptions.Add(codecOption);

            // Clear the input fields
            NewName = string.Empty;
            NewVideoCodec = string.Empty;
            NewAudioCodec = string.Empty;
            NewArguments = string.Empty;
        }
        catch (InvalidOperationException e)
        {
            Utils.GetInfoBox("Warning", e.Message);
        }
    }

    private void DefaultCodecOptionExecute(CodecOption? codecOption)
    {
        if (codecOption != null)
        {
            CodecOptions.Remove(codecOption);
        }
    }
}