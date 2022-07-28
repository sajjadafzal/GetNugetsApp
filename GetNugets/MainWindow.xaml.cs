using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Ookii.Dialogs.Wpf;

namespace GetNugets
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string? SolutionFolderPath;
        List<NugetPackage> packages;
        NugetPackage? CurrentPackage;

       public MainWindow()
        {
            InitializeComponent();
            packages = new List<NugetPackage>();
        }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog FolderDialog = new VistaFolderBrowserDialog();
            FolderDialog.Description = "Select a Solution Folder";
            FolderDialog.UseDescriptionForTitle = true;
            if (!VistaFolderBrowserDialog.IsVistaFolderDialogSupported)
            {
                MessageBox.Show(this, "Because you are not using Windows Vista or later, the regular folder browser dialog will be used. Please use Windows Vista to see the new dialog.", "Sample folder browser dialog");
            }

            if ((bool)FolderDialog.ShowDialog(this)!)
            {
                SolutionFolderPath = FolderDialog.SelectedPath;
                Status.Text = "Selected Folder: " + SolutionFolderPath;
            }
        }

        private void btnShowPackages_Click(object sender, RoutedEventArgs e)
        {
            Packagelist.ItemsSource = null;
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
            Status.Text = process.StandardError.ReadToEnd();
            Output.Text = commandOutput;

            string[] commandOutputs = commandOutput.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

            foreach(string str in commandOutputs)
            {
                if (str.Contains(">"))
                {
                    string[] subStr = str.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                    NugetPackage package = new NugetPackage(subStr[1], subStr[2]);
                    packages.Add(package);
                }
            }
            
            Packagelist.ItemsSource = packages;

            
            

        }

        private async void btnGetPackages_Click(object sender, RoutedEventArgs e)
        {

            foreach (NugetPackage package in packages)
            {
                if (package.Select)
                {
                    await GetNugetPackageAsync(package);
                    Output.Text = package.Output;
                    Status.Text = package.Error;
                }
            }
        }


        private async Task GetNugetPackageAsync(NugetPackage package)
        {
            await Task.Run(() =>
            {
                using (Process nugetProcess = new Process())
                {
                    ProcessStartInfo nugetStartInfo = new ProcessStartInfo();
                    nugetStartInfo.FileName = "nuget.exe";
                    nugetStartInfo.RedirectStandardOutput = true;
                    nugetStartInfo.RedirectStandardError = true;
                    nugetStartInfo.WindowStyle = ProcessWindowStyle.Normal;
                    nugetProcess.StartInfo = nugetStartInfo;

                    nugetStartInfo.Arguments = @$"install {package.Package} -Version {package.Version} -OutputDirectory C:\packages";
                    //nugetStartInfo.RedirectStandardOutput = true;
                    nugetProcess.EnableRaisingEvents = true;
                    nugetProcess.Exited += (o, e) =>
                    {
                        Process? p = o as Process;
                        if (p.ExitCode == 0)
                            package.Exited = true;
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
        private void Packagelist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            NugetPackage? pkg = Packagelist.SelectedItem as NugetPackage;
            Output.Text = pkg?.Output;
            Status.Text = pkg?.Error;

        }
    }
}