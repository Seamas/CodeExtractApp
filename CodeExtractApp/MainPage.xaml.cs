
using CodeExtractApp.ViewModel;
using MAUILib;

namespace CodeExtractApp;

public partial class MainPage : ContentPage
{
	private readonly IFolderPicker folderPicker;

	public MainPage(IFolderPicker folderPicker, MainPageModel model)
	{
		InitializeComponent();
		this.folderPicker = folderPicker;
		this.BindingContext = model;
	}

	private async void btnSource_Clicked(object sender, EventArgs e)
	{
		var result = await folderPicker.PickFolderAsync();
		if (result != null)
		{
			var model = this.BindingContext as MainPageModel;
            model.Input = result;
        }
    }

	private async void btnDest_Clicked(object sender, EventArgs e)
	{
		var options = new PickOptions()
		{
			FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
			{
				{ DevicePlatform.iOS, new[]{ ".txt" }},
                { DevicePlatform.Android, new[]{ ".txt" }},
                { DevicePlatform.WinUI, new[]{ ".txt" }},
                { DevicePlatform.Tizen, new[]{ ".txt" }},
                { DevicePlatform.macOS, new[]{ ".txt" }},
            })
		};
		var result = await FilePicker.Default.PickAsync(options);
		if (result != null)
		{
            var model = this.BindingContext as MainPageModel;
			model.Output = result.FullPath;
		}
	}

	private void btnOk_Clicked(object sender, EventArgs e)
	{
        var model = this.BindingContext as MainPageModel;

        btnOk.IsEnabled = false;

		Task.Run(() =>
		{
			CodeExtractTool.CodeExtract.Extract(model.Input, model.Output, model.GetSelectedItems(), model.Extension);
            this.Dispatcher.Dispatch(() =>
            {
                this.btnOk.IsEnabled = true;
            });
        });
		
	}
}

