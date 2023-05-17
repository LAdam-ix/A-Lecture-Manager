using System;
using HanumanInstitute.FFmpeg;
using System.ComponentModel.DataAnnotations;


namespace ALectureManager.Encoder;

public partial class EncoderProcessData : ObservableObject
{
    [Key] public Guid Id { get; set; } = Guid.NewGuid();

    #region AvaloniaAutoGeneratedProps
    [ObservableProperty] private string _name = string.Empty;
    [ObservableProperty] private string _inputPath = string.Empty;
    [ObservableProperty] private string _outputPath = string.Empty;
    [ObservableProperty] private long _progressBarMax = 100;
    [ObservableProperty] private long _progressBarValue;

    [ObservableProperty] private long _resumeFrame = 0;
    [ObservableProperty] private long _totalFrames = 0;
    [ObservableProperty] private string _status = string.Empty;
    [ObservableProperty] private TimeSpan _timeLeft = TimeSpan.Zero;
    [ObservableProperty] private bool _started;
    [ObservableProperty] private bool _paused;

    [ObservableProperty] private bool _finished;


    [ObservableProperty] private CodecOption? _codecOption;
    [ObservableProperty] private SavableProgressStatus _progress = new SavableProgressStatus();

    #endregion
}

// this was for the resume ignore for now
public partial class SavableProgressStatus : ViewModelBase
{
    public Guid Id { get; set; } = Guid.NewGuid();
    #region AvaloniaAutoGeneratedProps

    [ObservableProperty] private long _frame = 0;
    [ObservableProperty] private float _fps = 0;
    [ObservableProperty] private float _quantizer = 0;
    [ObservableProperty] private string _size = string.Empty;
    [ObservableProperty] private TimeSpan _time = TimeSpan.Zero;
    [ObservableProperty] private string _bitrate = string.Empty;
    [ObservableProperty] private float _speed = 0;
    #endregion


    public void Update(ProgressStatusFFmpeg progressStatus)
    {
        Frame = progressStatus.Frame;
        Fps = progressStatus.Fps;
        Quantizer = progressStatus.Quantizer;
        Size = progressStatus.Size;
        Time = progressStatus.Time;
        Bitrate = progressStatus.Bitrate;
        Speed = progressStatus.Speed;
    }

    public ProgressStatusFFmpeg ToProgressStatusFFmpeg()
    {
        return new ProgressStatusFFmpeg
        {
            Frame = Frame,
            Fps = Fps,
            Quantizer = Quantizer,
            Size = Size,
            Time = Time,
            Bitrate = Bitrate,
            Speed = Speed
        };
    }
}
