using CommunityToolkit.Mvvm.ComponentModel;
using GetNugets.Messages;
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
        private MessengerService messenger;
        private ObservableCollection<NugetPackageViewModel> existingPackages;

        /// <summary>
        /// Stores folder path to store downloded nugets. If appsettings.json exist then this
        /// variable is loaded from appsettings.json.
        /// </summary>
        public string? NugetsFolder
        {
            get => appSettings.NugetsFolder;
            set
            {
                if (appSettings.NugetsFolder == value) return;
                appSettings.NugetsFolder = value;
                messenger.Send<NugetsFolderChangedMessage>();
                GenericSerializer.Serialize<AppSettings>(appSettings, AppSettingsPath);
            }
        }

        /// <summary>
        /// <see cref="AppSettings"/> object containing setttigns for current application. 
        /// This file is loaded from appsettings.json file in the root directory
        /// </summary>
        AppSettings appSettings { get; set; }

        public ObservableCollection<NugetPackageViewModel> Packages { get; set; }

        /// <summary>
        /// Changing this property will send a <see cref="ExistingPackagesChangedMessage"/> message.
        /// </summary>
        public ObservableCollection<NugetPackageViewModel> ExistingPackages 
        { 
            get => existingPackages;
            set 
            { 
                if (existingPackages == value) return;
                existingPackages = value;
                messenger.Send(new ExistingPackagesChangedMessage());
            }
        }

        public AppStore(MessengerService messenger)
        {
            this.messenger = messenger;
            if (File.Exists(AppSettingsPath))
            {
                // appsettings.json exists
                appSettings = GenericSerializer.Deserialize<AppSettings>(AppSettingsPath);
            }
            else appSettings = new AppSettings();
        }
    }
}
