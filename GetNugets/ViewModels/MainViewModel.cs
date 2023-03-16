using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GetNugets.Messages;
using GetNugets.Store;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO.Packaging;
using System.Linq;
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
            set
            {
                appStore.NugetsFolder = value;
                OnPropertyChanged(nameof(NugetsFolder));
            }
        }

        public MainViewModel(NavigationService navigationService, MessengerService messenger, AppStore appStore)
        {
            this.navigationService = navigationService;
            this.messenger = messenger;
            this.appStore = appStore;
            SubscribeMessenger();
        }

        /// <summary>
        /// Response methon to  <see cref="CurrentViewModelChangedMessage"/> message
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

        public void ExistingDownloadView()
        {
            navigationService.NavigateTo(App.Current.Services.GetService<ExistingDownloadsViewModel>());
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
        }

        private void OnStatusMessageReceive(object recipient, StatusMessage message)
        {
            Status = message.Value;
        }
    }
}
