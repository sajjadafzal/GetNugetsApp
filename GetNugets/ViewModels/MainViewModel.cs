using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GetNugets.Messages;
using GetNugets.Store;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace GetNugets.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        private readonly NavigationService navigationService;
        private readonly MessengerService messenger;
        private readonly AppStore appStore;

        [ObservableProperty]
        private string? status;

        public ViewModelBase CurrentViewModel => navigationService.CurrentViewModel;

        public string? NugetsFolder
        {
            get => appStore.NugetsFolder;
            set => appStore.NugetsFolder = value;
        }        

        public MainViewModel(NavigationService navigationService, MessengerService messenger, AppStore appStore)
        {
            this.navigationService = navigationService;
            this.messenger = messenger;
            this.appStore = appStore;
            SubscribeMessenger();
        }

        /// <summary>
        /// Response method to  <see cref="CurrentViewModelChangedMessage"/> message
        /// </summary>
        /// <param name="recipient"></param>
        /// <param name="message"></param>
        private void OnCurrentViewModelChanged(object recipient, CurrentViewModelChangedMessage message)
        {
            OnPropertyChanged(nameof(CurrentViewModel));
        }

        [RelayCommand]
        public void BrowseNugetFolder()
        {
            string nuget_folder = DirectoryService.BrowseFolder("Select a Folder for Nuget Packages");
            
            if ( !string.IsNullOrEmpty(nuget_folder))
            {
                NugetsFolder = nuget_folder;
                Status = "Nugets Output Folder: " + NugetsFolder;          
            }
        }

        [RelayCommand]
        public void SaveDownloadPackageList()
        {
            SaveDownloadedPackagesToJson();
        }

        [RelayCommand]
        public void ExistingDownloadsView()
        {
            navigationService.NavigateTo(App.Current.Services.GetService<ExistingDownloadsViewModel>());
        }

        [RelayCommand]
        public void DownloaderView()
        {
            navigationService.NavigateTo(App.Current.Services.GetService<DownloaderViewModel>());
        }

        public override void Dispose()
        {
            base.Dispose();
            messenger.UnregisterAll(this);
        }

        private void SubscribeMessenger()
        {
            messenger.Subscribe<CurrentViewModelChangedMessage>(this, OnCurrentViewModelChanged);
            messenger.Subscribe<StatusMessage>(this, OnStatusMessageReceive);
            messenger.Subscribe<SavePackageJsonMessage>(this, SaveDownloadedPackagesJson);
            messenger.Subscribe<NugetsFolderChangedMessage>(this, NugetFolderChangeMessageReceive);
        }

        /// <summary>
        /// Response method to <see cref="NugetsFolderChangedMessage"/>.
        /// </summary>
        /// <param name="recipient"></param>
        /// <param name="message"></param>
        private void NugetFolderChangeMessageReceive(object recipient, NugetsFolderChangedMessage message)
        {
            OnPropertyChanged(nameof(NugetsFolder));
        }

        /// <summary>
        /// Response method to <see cref="SavePackageJsonMessage"/>.
        /// </summary>
        /// <param name="recipient"></param>
        /// <param name="message"></param>
        private void SaveDownloadedPackagesJson(object recipient, SavePackageJsonMessage message)
        {
            SaveDownloadedPackagesToJson();
        }

        private void SaveDownloadedPackagesToJson()
        {
            List<NugetPackageViewModel> completedlist = appStore.Packages.Where(p => p.Downloaded == true).ToList();
            if (completedlist.Count <= 0) return;
            List<NugetPackage> downloadedPackages = new List<NugetPackage>();
            foreach (var pkg in completedlist)
            {
                downloadedPackages.Add(new NugetPackage(pkg.Package, pkg.Version));
            }

            GenericSerializer.Serialize<List<NugetPackage>>(downloadedPackages, NugetsFolder + @"\packages.json");
            Status = @$"Saved Download Packages to {NugetsFolder + @"\packages.json"}";
        }


        private void OnStatusMessageReceive(object recipient, StatusMessage message)
        {
            Status = message.Value;
        }
    }
}
