
using GetNugets.Models;
using GetNugets.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.Design;

namespace GetNugets.Extensions
{
    public static class DIExtensions
    {
        public static IServiceCollection AddViewsAndViewModels(this IServiceCollection services)
        {
            services.AddSingleton<MainView>();
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<DownloaderView>();
            services.AddSingleton<DownloaderViewModel>();

            services.AddTransient<PackagesView>();
            services.AddTransient<PackagesViewModel>();  
            services.AddTransient<ExistingDownloadsView>();
            services.AddTransient<ExistingDownloadsViewModel>();

            return services;
        }

        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddSingleton<MessengerService>();
            services.AddSingleton<UIService>();
            services.AddSingleton<NavigationService>();
            services.AddSingleton<AppStore>();
            return services;
        }
    }
}
