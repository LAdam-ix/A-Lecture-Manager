using System;
using System.ComponentModel.DataAnnotations;

namespace ALectureManager.Encoder;

[Serializable]
public record CodecOption
{
    private string _name = string.Empty;
    private string? _videoCodec;
    private string? _audioCodec;
    private bool _oneNull;


    [Key] public Guid Id { get; set; } = Guid.NewGuid();
    public string Name
    {
        get => _name;
        init => _name = value ?? _name;
    }

    public string? VideoCodec
    {
        get => _videoCodec;
        init
        {
            if (value == null)
            {
                _oneNull = _oneNull == true
                    ? throw new InvalidOperationException(
                        "Only one of VideoCodec and AudioCodec can be non-null at a time.")
                    : true;
            }

            _videoCodec = value;
        }
    }

    public string? AudioCodec
    {
        get => _audioCodec;
        init
        {
            if (value == null)
            {
                _oneNull = _oneNull == true
                    ? throw new InvalidOperationException(
                        "Only one of VideoCodec and AudioCodec can be non-null at a time.")
                    : true;
            }

            _audioCodec = value;
        }
    }

    public string? AdditionalArguments { get; init; }

    public override string ToString()
    {
        return Name;
    }
}