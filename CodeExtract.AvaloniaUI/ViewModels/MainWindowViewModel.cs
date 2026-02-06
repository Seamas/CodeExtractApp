using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CodeExtract.AvaloniaUI.Services;
using CodeExtractTool.Comment;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MsBox.Avalonia;

namespace CodeExtract.AvaloniaUI.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private string? _source;
    [ObservableProperty]
    private string? _dest;
    [ObservableProperty]
    private string? _extension;
    [ObservableProperty]
    private bool _singleFile;

    public ObservableCollection<SelectedItem> Items { get; private set; } = new()
    {
        new(new EmptyLineComment(), true),
        new(new HtmlComment(), true),
        new(new SingleComment(), true),
        new(new MultiComment(), true)
    };

    public MainWindowViewModel()
    {
        Items.CollectionChanged += OnItemsCollectionChanged;
        foreach (var item in Items)
        {
            item.PropertyChanged += OnItemPropertyChanged;
        }
        WeakReferenceMessenger.Default.Register<MainWindowViewModel, ItemSelectionChangedMessage>(
            this, (recipient, message) =>
            {
                recipient.CleanCommand.NotifyCanExecuteChanged();
            });
    }
    
    
    
    // 清除注释方法
    [RelayCommand(CanExecute = nameof(CanClean))]
    private async Task Clean()
    {
        var regexComments = Items.Where(item => item.IsSelected)
            .Select(item => item.Comment).ToList();

        var extension = $".{Extension}";
        if (SingleFile)
        {
            var output = Path.Combine(Dest!, $"result{extension}");
            await CodeExtractTool.CodeExtract.ExtractSingleFileAsync(Source!, output, regexComments, extension!);
        }
        else
        {
            await CodeExtractTool.CodeExtract.ExtractMultipleFilesAsync(Source!, Dest!, regexComments, extension!);
        }

        await MessageBoxManager.GetMessageBoxStandard("提示", "操作成功").ShowWindowAsync();
    }

    private bool CanClean()
    {
        return !string.IsNullOrEmpty(Source) && !string.IsNullOrEmpty(Dest) && !string.IsNullOrEmpty(Extension)
               && Items.Any(item => item.IsSelected);
    }
    
    [RelayCommand]
    private async Task SelectSourceFolder()
    {
        Source =  await FileDialogHelper.OpenFolder(null, "选择源代码目录");
    }

    [RelayCommand]
    private async Task SelectDestFolder()
    {
        Dest =  await FileDialogHelper.OpenFolder(null, "选择目标代码目录");
    }
    
    private void OnItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems != null)
        {
            foreach (SelectedItem item in e.NewItems)
            {
                item.PropertyChanged += OnItemPropertyChanged;
            }
        }
        
        if (e.OldItems != null)
        {
            foreach (SelectedItem item in e.OldItems)
            {
                item.PropertyChanged -= OnItemPropertyChanged;
            }
        }
        
        // 发送消息通知命令状态更新
        WeakReferenceMessenger.Default.Send<ItemSelectionChangedMessage>();
    }
    
    private void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(SelectedItem.IsSelected))
        {
            // 发送消息通知命令状态更新
            WeakReferenceMessenger.Default.Send<ItemSelectionChangedMessage>();
        }
    }
    
    
    partial void OnSourceChanged(string? value)
    {
        CleanCommand.NotifyCanExecuteChanged();
    }

    partial void OnExtensionChanged(string? value)
    {
        CleanCommand.NotifyCanExecuteChanged();
    }

    partial void OnDestChanged(string? value)
    {
        CleanCommand.NotifyCanExecuteChanged();
    }
}