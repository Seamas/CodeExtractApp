using CodeExtractTool.Comment;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CodeExtract.AvaloniaUI.ViewModels;

public partial class SelectedItem(IRegexComment comment, bool isSelected) : ObservableObject
{
    [ObservableProperty]
    private IRegexComment _comment = comment;
    
    [ObservableProperty]
    private bool _isSelected = isSelected;
    
}