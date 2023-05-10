using LiteExplorer.MVVM.ViewModels;
using LiteExplorer.MVVM.Views.Windows;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace LiteExplorer;

public partial class App
{
    private readonly ServiceProvider serviceProvider;

    public App()
    {
        IServiceCollection services = new ServiceCollection();
        services.AddSingleton<MainWindow>();

        serviceProvider = services.BuildServiceProvider();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        var mainWindow = serviceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();

        base.OnStartup(e);
    }
}