using CodeExtractTool.Comment;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace CodeExtractApp.ViewModel
{
    public partial class MainPageModel : ObservableObject
    {

        public MainPageModel(List<RegexSource> items)
        {
            this.items = new ObservableCollection<RegexSource>(items);
        }


        [ObservableProperty]
        ObservableCollection<RegexSource> items;

        [ObservableProperty]
        private string input;

        [ObservableProperty]
        private string output;

        [ObservableProperty]
        private string extension = "*.java";

        public IEnumerable<IRegexComment> GetSelectedItems()
        {
            var result = items.Where(item => item.IsSelected)
                .Select(item => item.Value);
            return result;
        }
    }
}
