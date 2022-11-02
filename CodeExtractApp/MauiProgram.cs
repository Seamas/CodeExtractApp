namespace CodeExtractApp;

using CodeExtractApp.ViewModel;
using CodeExtractTool.Comment;
using MAUILib;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if WINDOWS
		builder.Services.AddTransient<IFolderPicker, MAUILib.Platforms.Windows.FolderPicker>();
#elif MACCATALYST
		builder.Services.AddTransient<IFolderPicker, MAUILib.Platforms.MacCatalyst.FolderPicker>();
#endif
		builder.Services.AddSingleton<MainPageModel>();
		builder.Services.AddSingleton<List<RegexSource>>((provider) =>
		{
			var assemblies = AppDomain.CurrentDomain.GetAssemblies();
			var list = assemblies
				.Select(item => item.GetTypes().Where(x => x.IsClass && x.IsAssignableTo(typeof(IRegexComment)))
									.Select(x => Activator.CreateInstance(x) as IRegexComment)
									.ToList()
				).Aggregate((a, b) => { a.AddRange(b); return a; })
				.OrderBy(item => item.Order)
				.Select(item => new RegexSource { Value = item, IsSelected = true })
				.ToList();
			return list;
		});
		builder.Services.AddSingleton<MainPage>();
		builder.Services.AddSingleton<App>();
        return builder.Build();
	}
}
