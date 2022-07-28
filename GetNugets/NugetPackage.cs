using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetNugets
{
    internal class NugetPackage : INotifyPropertyChanged
    {
        public string Package { get; set; }
        public string Version { get; set; }
        public bool Select { get; set; }

        public string Error { get; set; }
        public string Output { get; set; }

        private bool exited;

        public event PropertyChangedEventHandler? PropertyChanged;

        public bool Exited 
        { 
            get => exited; 
            set 
            {
                exited = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Exited)));
            } 
        }

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
}
