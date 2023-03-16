using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GetNugets.Messages;
using GetNugets.Models;
using GetNugets.Services;
using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetNugets.ViewModels
{
    public partial class DownloaderViewModel : ViewModelBase
    {
        private MessengerService messenger;
        private AppStore appStore;

        private string? SolutionFolderPath;

        private AppSettings? CurrentAppSettings;

        private string CurrentApplicationPath;

        private string AppSettingsPath;
        //public ObservableCollection<NugetPackageViewModel> packages { get; set; }

        public ObservableCollection<NugetPackageViewModel> packages
        {
            get => appStore.Packages; set => appStore.Packages = value;
        }

        [ObservableProperty]
        private bool forceVersion = false;

        [ObservableProperty]
        private string outputText = "";

        [ObservableProperty]
        private bool isVersionChecked = false;

        /// <summary>
        /// Change will be propagated through messenger's <see cref="NugetsFolderChangedMessage"/> message
        /// </summary>
        public string? NugetsFolder
        {
            get => appStore.NugetsFolder;
            set
            {
                appStore.NugetsFolder = value;
                OnPropertyChanged(nameof(NugetsFolder));
                BrowseCommand.NotifyCanExecuteChanged();
            }
        }

        [ObservableProperty]
        private NugetPackageViewModel selectedPackage;

        bool IsInProcess = false;

        public DownloaderViewModel(MessengerService messenger, AppStore appStore)
        {
            this.messenger = messenger;
            this.appStore = appStore;
            SubscribeMessenger();
            packages = new ObservableCollection<NugetPackageViewModel>();    
        }

        [RelayCommand(CanExecute = nameof(CanBrowse))]
        private void Browse()
        {

            SolutionFolderPath = DirectoryService.BrowseFolder("Select a Solution Folder");
            if (SolutionFolderPath != null)
                UpdateStatus("Selected Folder: " + SolutionFolderPath);
        }

        [RelayCommand]
        private void BrowseNugetFolder()
        {
            NugetsFolder = DirectoryService.BrowseFolder("Select a Folder for Nuget Packages");
            if (NugetsFolder != null)
            {
                UpdateStatus("Nugets Output Folder: " + NugetsFolder);
                (CurrentAppSettings ??= new AppSettings()).NugetsFolder = NugetsFolder;
            }
        }

        private bool CanBrowse()
        {
            return !string.IsNullOrEmpty(NugetsFolder);
        }

        [RelayCommand]
        private void ShowPackages()
        {
            packages.Clear();
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "dotnet";
            startInfo.Arguments = "list package --include-transitive"; //--include-transitive
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.WorkingDirectory = SolutionFolderPath;
            process.StartInfo = startInfo;
            process.Start();
            string commandOutput = process.StandardOutput.ReadToEnd();
            UpdateStatus(process.StandardError.ReadToEnd());
            outputText = commandOutput;

            string[] commandOutputs = commandOutput.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

            foreach (string str in commandOutputs)
            {
                if (str.Contains(">"))
                {
                    string[] subStr = str.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                    NugetPackageViewModel package = new NugetPackageViewModel(subStr[1], subStr[2]);
                    packages.Add(package);
                }
            }

        }

        [RelayCommand]
        private async void GetPackages()
        {
            IsInProcess = true;
            foreach (NugetPackageViewModel package in packages)
            {
                if (package.Select)
                {
                    await GetNugetPackageAsync(package, ForceVersion);
                    OutputText = package.Output;
                    UpdateStatus(package.Error);
                }
                if (!IsInProcess)
                {
                    UpdateStatus("Process has been cancelled");
                    break;
                }
            }
            IsInProcess = false;
        }

        private async Task GetNugetPackageAsync(NugetPackageViewModel package, bool getVersion)
        {
            await Task.Run(() =>
            {
                using (Process nugetProcess = new Process())
                {
                    ProcessStartInfo nugetStartInfo = new ProcessStartInfo();
                    nugetStartInfo.FileName = "nuget.exe";
                    nugetStartInfo.RedirectStandardOutput = true;
                    nugetStartInfo.RedirectStandardError = true;
                    nugetStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    nugetStartInfo.CreateNoWindow = true;
                    nugetProcess.StartInfo = nugetStartInfo;
                    if (getVersion)
                        nugetStartInfo.Arguments = @$"install {package.Package} -Version {package.Version} -OutputDirectory {NugetsFolder}";
                    else
                        nugetStartInfo.Arguments = @$"install {package.Package} -OutputDirectory {NugetsFolder}";
                    //nugetStartInfo.RedirectStandardOutput = true;
                    nugetProcess.EnableRaisingEvents = true;
                    nugetProcess.Exited += (o, e) =>
                    {
                        Process? p = o as Process;
                        if (p.ExitCode == 0)
                            package.Downloaded = true;
                    };
                    //nugetProcess.OutputDataReceived += (o, e) =>
                    //{
                    //    processOutputText.Append("\n" + e.Data);                            
                    //};
                    nugetProcess.Start();
                    //nugetProcess.BeginOutputReadLine();
                    package.Output = nugetProcess.StandardOutput.ReadToEnd();
                    package.Error = nugetProcess.StandardError.ReadToEnd();
                    nugetProcess.WaitForExit();
                    //Status.Text += " " + nugetProcess.ExitCode.ToString();
                    //return package;
                }
            });

        }

        [RelayCommand]
        public void PackageSelectionChanged(NugetPackageViewModel newlySelectedPackage)
        {
            SelectedPackage = newlySelectedPackage;
            OutputText = SelectedPackage.Output;
        }

        [RelayCommand]
        public void EscapeKeyPressed()
        {
            IsInProcess = false;
        }

        //[RelayCommand]
        //public void SaveDownloadPackageList()
        //{
        //    List<NugetPackageViewModel> completedlist = packages.Where(p => p.Downloaded == true).ToList();
        //    if (completedlist.Count <= 0) return;
        //    List<NugetPackage> downloadedPackages = new List<NugetPackage>();
        //    foreach (var pkg in completedlist)
        //    {
        //        downloadedPackages.Add(new NugetPackage(pkg.Package, pkg.Version));
        //    }

        //    GenericSerializer.Serialize<List<NugetPackage>>(downloadedPackages, NugetsFolder + @"\packages.json");
        //    StatusInfo = @$"Saved Download Packages to {NugetsFolder + @"\packages.json"}";

        //}

        private void SubscribeMessenger()
        {
            messenger.Subscribe<NugetsFolderChangedMessage>(this, OnNugetsFolderChanged);
        }

        private void OnNugetsFolderChanged(object recipient, NugetsFolderChangedMessage message)
        {
            OnPropertyChanged(nameof(NugetsFolder));
            BrowseCommand.NotifyCanExecuteChanged(); //Because it depends upon NugetsFolder
        }


        private void UpdateStatus(string message)
        {
            messenger.Send(new StatusMessage(message));
        }
    }
}
