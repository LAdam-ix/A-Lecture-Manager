using System.Linq;
using System;
using System.IO;
using Newtonsoft.Json;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;


namespace ALectureManager.Encoder;

[Serializable]
public partial class EncoderSettingsViewModel : ViewModelBase
{
    [ObservableProperty] private string _fFMpegPath = string.Empty;
    [ObservableProperty] private string _newName = string.Empty;
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

    public EncoderSettingsViewModel(string path) : this()
    {
        FFMpegPath = path; // exclude .exe to work on Linux/MacOS
        CodecOptions = new ObservableCollection<CodecOption>()
        {
            new CodecOption
            {
                Name = "Cuda for Lectures",
                VideoCodec = "libx265",
                AudioCodec = null,
                AdditionalArguments = "-hwaccel cuda -crf 28"
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

        DefaultCodecOption = CodecOptions[1];
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
            NewArguments = e.Message;
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