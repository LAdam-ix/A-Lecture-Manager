using System.IO;
using ALectureManager.Encoder;
using System.ComponentModel.DataAnnotations;

namespace ALectureManager.Library;

public partial class LibraryEntry : ObservableObject
{
    [Key] public Guid Id { get; set; } = Guid.NewGuid();

    [ObservableProperty] private string _name = string.Empty;
    [ObservableProperty] private string _directoryPath;
    [ObservableProperty] private string _outputDirectoryPath;

    public CodecOption? EnteryDefaultCodec { get; set; }

    public LibraryEntry(string directoryPath, string? outputDirectoryPath = null)
    {
        DirectoryPath = directoryPath;
        Name = Path.GetFileName(DirectoryPath) ?? "Directory not found";
        OutputDirectoryPath = outputDirectoryPath ?? Path.Combine(directoryPath, Name + " Encoded");
    }


    public List<EncoderProcessData> Scan()
    {
        if (!Directory.Exists(OutputDirectoryPath))
            Directory.CreateDirectory(OutputDirectoryPath);
        string[] supportedFormats = { "*.mp4", "*.mkv", "*.avi", "*.m4p" };
        List<EncoderProcessData> newProcessDataList = new List<EncoderProcessData>();

        foreach (var format in supportedFormats)
        {
            var videoFiles = Directory.GetFiles(DirectoryPath, format, SearchOption.TopDirectoryOnly);

            foreach (var videoFile in videoFiles)
            {
                var videoFileName = Path.GetFileName(videoFile);
                var outputFilePath = Path.Combine(OutputDirectoryPath, videoFileName);

                if (File.Exists(outputFilePath)) continue;
                var processData = new EncoderProcessData()
                {
                    InputPath = videoFile,
                    OutputPath = outputFilePath,
                    CodecOption = EnteryDefaultCodec
                };
                newProcessDataList.Add(processData);
            }
        }

        //make a reverse sort by name?
        return newProcessDataList.OrderByDescending(x => x.InputPath).ToList();
    }

    public override string ToString()
    {
        return Name;
    }
}