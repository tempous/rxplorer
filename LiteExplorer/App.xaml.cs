using LiteExplorer.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace LiteExplorer
{
    public partial class App
    {
        private readonly ServiceProvider serviceProvider;

        public App()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<MainWindow>(/*provider => new()
            {
                DataContext = provider.GetRequiredService<MainWindowViewModel>()
            }*/);

            serviceProvider = services.BuildServiceProvider();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            var mainWindow = serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.DataContext = serviceProvider.GetRequiredService<MainWindowViewModel>();
            mainWindow.Show();

            base.OnStartup(e);
        }
    }
}