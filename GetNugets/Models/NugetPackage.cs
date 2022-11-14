using CommunityToolkit.Mvvm.ComponentModel;
using GetNugets.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetNugets.Models;

public partial class NugetPackage : ViewModelBase
{
    public string Package { get; set; }
    public string Version { get; set; }
    public bool Select { get; set; }

    public string Error { get; set; }
    public string Output { get; set; }

    [ObservableProperty]
    private bool exited;    

    public NugetPackage(string package, string version)
    {
        Package = package;
        Version = version;
        Select = true;
        Exited = false;
        Error = string.Empty;
        Output = string.Empty;
    }
}
