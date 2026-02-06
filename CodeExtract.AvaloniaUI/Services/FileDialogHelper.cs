using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;

namespace CodeExtract.AvaloniaUI.Services;

public static class FileDialogHelper
{
    /// <summary>
    /// 获取存储提供者
    /// </summary>
    private static IStorageProvider? GetStorageProvider(Control? control = null)
    {
        var topLevel = control != null 
            ? TopLevel.GetTopLevel(control) 
            : GetActiveWindow();
        
        return topLevel?.StorageProvider;
    }
    
    /// <summary>
    /// 获取活动窗口
    /// </summary>
    private static Window? GetActiveWindow()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            return desktop.Windows.FirstOrDefault(w => w.IsActive) ?? desktop.MainWindow;
        }
        return null;
    }
    
    
    /// <summary>
    /// 打开单个文件
    /// </summary>
    public static async Task<string?> OpenFile(
        Control? owner = null,
        string title = "选择文件",
        string filterName = "所有文件",
        string[]? filterExtensions = null,
        string? initialDirectory = null)
    {
        var storageProvider = GetStorageProvider(owner);
        if (storageProvider == null) 
            return null;
        
        var options = new FilePickerOpenOptions
        {
            Title = title,
            AllowMultiple = false
        };
        
        // 设置过滤器
        if (filterExtensions != null && filterExtensions.Length > 0)
        {
            options.FileTypeFilter =
            [
                new FilePickerFileType(filterName)
                {
                    Patterns = filterExtensions.Select(ext => 
                        ext.StartsWith("*") ? ext : $"*.{ext}").ToArray()
                }
            ];
        }
        
        // 设置初始目录
        if (!string.IsNullOrEmpty(initialDirectory) && Directory.Exists(initialDirectory))
        {
            options.SuggestedStartLocation = await storageProvider.TryGetFolderFromPathAsync(initialDirectory);
        }
        
        var files = await storageProvider.OpenFilePickerAsync(options);
        
        return files.FirstOrDefault()?.Path.LocalPath;
    }
    
    /// <summary>
    /// 打开多个文件
    /// </summary>
    public static async Task<List<string>> OpenMultipleFiles(
        Control? owner = null,
        string title = "选择多个文件",
        Dictionary<string, string[]>? filters = null)
    {
        var storageProvider = GetStorageProvider(owner);
        if (storageProvider == null) return new List<string>();
        
        var options = new FilePickerOpenOptions
        {
            Title = title,
            AllowMultiple = true
        };
        
        // 设置过滤器
        if (filters != null && filters.Count > 0)
        {
            options.FileTypeFilter = filters.Select(f => 
                new FilePickerFileType(f.Key)
                {
                    Patterns = f.Value.Select(ext => 
                        ext.StartsWith("*") ? ext : $"*.{ext}").ToArray()
                }).ToList();
        }
        
        var files = await storageProvider.OpenFilePickerAsync(options);
        
        return files.Select(f => f.Path.LocalPath).ToList() ?? [];
    }
    
    /// <summary>
    /// 保存文件
    /// </summary>
    public static async Task<string?> SaveFile(
        Control? owner = null,
        string title = "保存文件",
        string? defaultFileName = null,
        string filterName = "所有文件",
        string[]? filterExtensions = null)
    {
        var storageProvider = GetStorageProvider(owner);
        if (storageProvider == null) 
            return null;
        
        var options = new FilePickerSaveOptions
        {
            Title = title,
            ShowOverwritePrompt = true,
            SuggestedFileName = defaultFileName
        };
        
        // 设置过滤器
        if (filterExtensions != null && filterExtensions.Length > 0)
        {
            options.FileTypeChoices = new[]
            {
                new FilePickerFileType(filterName)
                {
                    Patterns = filterExtensions.Select(ext => 
                        ext.StartsWith("*") ? ext : $"*.{ext}").ToArray()
                }
            };
            options.DefaultExtension = filterExtensions.FirstOrDefault()?.TrimStart('*', '.');
        }
        
        var file = await storageProvider.SaveFilePickerAsync(options);
        
        return file?.Path.LocalPath;
    }
    
    /// <summary>
    /// 选择文件夹
    /// </summary>
    public static async Task<string?> OpenFolder(
        Control? owner = null,
        string title = "选择文件夹")
    {
        var storageProvider = GetStorageProvider(owner);
        if (storageProvider == null) return null;
        
        var options = new FolderPickerOpenOptions
        {
            Title = title,
            AllowMultiple = false
        };
        
        var folders = await storageProvider.OpenFolderPickerAsync(options);
        
        return folders.FirstOrDefault()?.Path.LocalPath;
    }
}