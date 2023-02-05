using CommunityToolkit.Mvvm.ComponentModel;
using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Windows;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using System.IO;
using GetNugets.Models;
using System.Text.Json;

namespace GetNugets.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        private string? SolutionFolderPath;

        private AppSettings? CurrentAppSettings;
        
        private string CurrentApplicationPath;

        private string AppSettingsPath;
        public ObservableCollection<NugetPackageViewModel> packages { get; set; }

        [ObservableProperty]
        private bool forceVersion = false;

        [ObservableProperty]
        private string statusInfo = "-";

        [ObservableProperty]
        private string outputText = "";

        [ObservableProperty]
        private bool isVersionChecked = false;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(BrowseCommand))]
        private string? nugetFolder;

        [ObservableProperty]
        private NugetPackageViewModel selectedPackage;

        bool IsInProcess = false;

        public MainViewModel()
        {
            packages = new ObservableCollection<NugetPackageViewModel>();
            CurrentApplicationPath = AppDomain.CurrentDomain.BaseDirectory;
            AppSettingsPath = @$"{CurrentApplicationPath}appsettings.json";
            if (File.Exists(AppSettingsPath))
            {
                // appsettings.json exists
                string jsonString = File.ReadAllText(AppSettingsPath);
                CurrentAppSettings = JsonSerializer.Deserialize<AppSettings>(jsonString);
                NugetFolder = CurrentAppSettings.NugetsFolder;
            }
        }

        [RelayCommand(CanExecute = nameof(CanBrowse))]
        private void Browse()
        {
            VistaFolderBrowserDialog FolderDialog = new VistaFolderBrowserDialog();
            FolderDialog.Description = "Select a Solution Folder";
            FolderDialog.UseDescriptionForTitle = true;
            if (!VistaFolderBrowserDialog.IsVistaFolderDialogSupported)
            {
                MessageBox.Show("Because you are not using Windows Vista or later, the regular folder browser dialog will be used. Please use Windows Vista to see the new dialog.", "Sample folder browser dialog");
            }

            if ((bool)FolderDialog.ShowDialog()!)
            {
                SolutionFolderPath = FolderDialog.SelectedPath;
                StatusInfo = "Selected Folder: " + SolutionFolderPath;
            }
        }

        [RelayCommand]
        private void BrowseNugetFolder()
        {
            VistaFolderBrowserDialog FolderDialog = new VistaFolderBrowserDialog();
            FolderDialog.Description = "Select a Folder for Nuget Packages";            
            FolderDialog.UseDescriptionForTitle = true;
            if (!VistaFolderBrowserDialog.IsVistaFolderDialogSupported)
            {
                MessageBox.Show("Because you are not using Windows Vista or later, the regular folder browser dialog will be used. Please use Windows Vista to see the new dialog.", "Sample folder browser dialog");
            }

            if ((bool)FolderDialog.ShowDialog()!)
            {
              
                NugetFolder = FolderDialog.SelectedPath;
                StatusInfo = "Nugets Output Folder: " + NugetFolder;
                (CurrentAppSettings??=new AppSettings()).NugetsFolder = NugetFolder;
                JsonSerializerOptions options = new JsonSerializerOptions { WriteIndented = true };
                string jsonString = JsonSerializer.Serialize<AppSettings>(CurrentAppSettings, options);
                File.WriteAllText(AppSettingsPath, jsonString);
            }
        }

        private bool CanBrowse()
        {
            return !string.IsNullOrEmpty(NugetFolder);
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
            StatusInfo = process.StandardError.ReadToEnd();
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
                    StatusInfo = package.Error;
                }
                if (!IsInProcess)
                {
                    StatusInfo = "Process has been cancelled";
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
                        nugetStartInfo.Arguments = @$"install {package.Package} -Version {package.Version} -OutputDirectory {NugetFolder}";
                    else
                        nugetStartInfo.Arguments = @$"install {package.Package} -OutputDirectory {NugetFolder}";
                    //nugetStartInfo.RedirectStandardOutput = true;
                    nugetProcess.EnableRaisingEvents = true;
                    nugetProcess.Exited += (o, e) =>
                    {
                        Process? p = o as Process;
                        if (p.ExitCode == 0)
                            package.Exited= true;
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
            SelectedPackage= newlySelectedPackage;
            OutputText = SelectedPackage.Output;
        }

        [RelayCommand]
        public void EscapeKeyPressed()
        {
            IsInProcess = false;
        }
    }
}
