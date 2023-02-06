using CommunityToolkit.Mvvm.ComponentModel;
using GetNugets.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetNugets.Models;

public partial class NugetPackage
{
    public string Package { get; set; }
    public string Version { get; set; }

    public NugetPackage()
    {
        Package = string.Empty;
        Version = string.Empty;
    }
    public NugetPackage(string package, string version)
    {
        Package = package;
        Version = version;
    }
}
