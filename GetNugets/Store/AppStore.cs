using CommunityToolkit.Mvvm.ComponentModel;
using GetNugets.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetNugets.Store
{
    public class AppStore
    {
        public readonly string AppSettingsPath = @$"{AppDomain.CurrentDomain.BaseDirectory}appsettings.json";
        public string? NugetsFolder
        {
            get => appSettings.NugetsFolder;
            set
            {
                appSettings.NugetsFolder = value;
                GenericSerializer.Serialize<AppSettings>(appSettings, AppSettingsPath);
            }
        }

        AppSettings appSettings { get; set; }

        public ObservableCollection<NugetPackageViewModel> packages { get; set; }

        public AppStore()
        {          
            if (File.Exists(AppSettingsPath))
            {
                // appsettings.json exists
                appSettings = GenericSerializer.Deserialize<AppSettings>(AppSettingsPath);                
            }
        }
    }
}
