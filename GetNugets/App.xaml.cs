using GetNugets.Views;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace GetNugets
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        MainView window;
        /// <summary>
        /// Gets the <see cref="IServiceProvider"/> instance to resolve application services.
        /// </summary>
        public IServiceProvider Services { get; }

        /// <summary>
        /// Gets the current <see cref="App"/> instance in use
        /// </summary>
        public new static App Current => (App)Application.Current;

        public App()
        {            
            Services = ConfigureServices();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);            
            window = Services.GetService<MainView>();
            window.Show();
        }

        private static IServiceProvider ConfigureServices()
        {
            ServiceCollection services = new ServiceCollection();

            return services.AddViewsAndViewModels()
                .AddServices()
                .BuildServiceProvider();


            //return services.BuildServiceProvider();
        }
    }
}
