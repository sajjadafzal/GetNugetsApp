using CommunityToolkit.Mvvm.ComponentModel;
using GetNugets.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetNugets
{
    public partial class NugetPackageViewModel : ViewModelBase
    {
        public string Package { get; set; }
        public string Version { get; set; }
        public bool Select { get; set; }

        public string Error { get; set; }
        public string Output { get; set; }

        [ObservableProperty]
        private bool downloaded;               

        public NugetPackageViewModel(string package, string version)
        {
            Package = package;
            Version = version;
            Select = true;
            Downloaded = false;
            Error = string.Empty;
            Output = string.Empty;
        }
    }
}
