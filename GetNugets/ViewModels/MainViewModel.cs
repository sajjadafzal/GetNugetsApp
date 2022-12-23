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

namespace GetNugets.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        string? SolutionFolderPath;
        public ObservableCollection<NugetPackage> packages { get; set; }

        [ObservableProperty]
        private bool forceVersion = false;

        [ObservableProperty]
        private string statusInfo = "-";

        [ObservableProperty]
        private string outputText = "";

        [ObservableProperty]
        private bool isVersionChecked = false;


        bool IsInProcess = false;

        public MainViewModel()
        {
            packages = new ObservableCollection<NugetPackage>();
        }

        [RelayCommand]
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
                    NugetPackage package = new NugetPackage(subStr[1], subStr[2]);
                    packages.Add(package);
                }
            }
            
        }

        [RelayCommand]
        private async void GetPackages()
        {
            //IsInProcess = true;
            foreach (NugetPackage package in packages)
            {
                if (package.Select)
                {
                    await GetNugetPackageAsync(package, ForceVersion);
                    outputText = package.Output;
                    StatusInfo = package.Error;
                }
                if (!IsInProcess)
                {
                    StatusInfo = "Process has been cancelled";
                    break;
                }
            }
            //IsInProcess = false;
        }

        private async Task GetNugetPackageAsync(NugetPackage package, bool getVersion)
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
                        nugetStartInfo.Arguments = @$"install {package.Package} -Version {package.Version} -OutputDirectory D:\NugetPackages";
                    else
                        nugetStartInfo.Arguments = @$"install {package.Package} -OutputDirectory D:\NugetPackages";
                    //nugetStartInfo.RedirectStandardOutput = true;
                    nugetProcess.EnableRaisingEvents = true;
                    nugetProcess.Exited += (o, e) =>
                    {
                        Process? p = o as Process;
                        if (p.ExitCode == 0)
                            package.Existed = true;
                    };
                    //nugetProcess.OutputDataReceived += (o, e) =>
                    //{
                    //    processOutputText.Append("\n" + e.Data);                            
                    //};
                    nugetProcess.Start();
                    //nugetProcess.BeginOutputReadLine();
                    package.Output = nugetProcess.StandardOutput.ReadToEnd();
                    package.Error = nugetProcess.StandardError.ReadToEnd();
                    nugetProcess.WaitForExit(3000);
                    //Status.Text += " " + nugetProcess.ExitCode.ToString();
                    //return package;
                }
            });

        }
    }
}
