using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Styling;
using MessageBox.Avalonia;
using MessageBox.Avalonia.DTO;
using MessageBox.Avalonia.Enums;

namespace ALectureManager;

public static class Utils
{
    public static async Task<string?> OpenVideoFile()
    {
        var filters = new List<FileDialogFilter>
        {
            new FileDialogFilter
            {
                Name = "Video Files",
                Extensions = new List<string> { "mp4", "avi", "mkv", "mov", "m4p" }
            }
        };
        return await OpenFileExplorer(filters: filters);
    }


    public static async Task<string?> OpenFileExplorer(bool openDirectory = false,
        List<FileDialogFilter>? filters = null)
    {
        filters ??= new List<FileDialogFilter>();

        var mainWindow = (IClassicDesktopStyleApplicationLifetime)Application.Current?.ApplicationLifetime!;
        if (openDirectory)
        {
            var openDialog = new OpenFolderDialog();
            var result = await openDialog.ShowAsync(mainWindow.MainWindow);
            return result is { Length: > 0 } ? result : null;
        }
        else
        {
            var openDialog = new OpenFileDialog()
            {
                Filters = filters
            };
            var result = await openDialog.ShowAsync(mainWindow.MainWindow);
            return result is { Length: > 0 } ? result[0] : null;
        }


    }

    public static string GetAutoOutputPath(string filePath)
    {
        string extension = Path.GetExtension(filePath);
        string modifiedPath = filePath.Substring(0, filePath.Length - extension.Length)
                              + "_converted"
                              + extension;
        return modifiedPath;
    }

    public static string? TryGetDirectory(string path)
    {
        try
        {
            return Path.GetDirectoryName(path);
        }
        catch (ArgumentException)
        {
        }
        catch (PathTooLongException)
        {
        }

        return null;
    }

    public async static Task<ButtonResult> GetInfoBox(
        string title, string message, 
        ButtonEnum buttons = ButtonEnum.Ok)
    {

        var msBoxStandardWindow = MessageBox.Avalonia.MessageBoxManager
        .GetMessageBoxStandardWindow(new MessageBoxStandardParams
        {
            ButtonDefinitions = buttons,
            ContentTitle = title,
            ContentMessage = message,
            Icon = Icon.Plus,
        });
        return await msBoxStandardWindow.Show();
    }

}