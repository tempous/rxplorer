using LiteExplorer.MVVM.ViewModels;
using LiteExplorer.MVVM.Views.Windows;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;

namespace LiteExplorer;

public partial class App
{
    public static IServiceProvider Container { get; private set; }

    public App() => RegisterServices();

    protected override void OnStartup(StartupEventArgs e) => Container.GetService<MainWindow>().Show();

    private static void RegisterServices()
    {
        var services = new ServiceCollection();

        services.AddSingleton<MainWindowViewModel>();
        services.AddSingleton<MainWindow>(provider => new()
        {
            DataContext = provider.GetService<MainWindowViewModel>()
        });

        services.AddTransient<TabContentViewModel>();

        Container = services.BuildServiceProvider();
    }
}