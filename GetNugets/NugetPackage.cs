using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetNugets
{
    public class NugetPackage : INotifyPropertyChanged
    {
        public string Package { get; set; }
        public string Version { get; set; }
        public bool Select { get; set; }

        public string Error { get; set; }
        public string Output { get; set; }

        private bool existed;

        public event PropertyChangedEventHandler? PropertyChanged;

        public bool Existed 
        { 
            get => existed; 
            set 
            {
                existed = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Existed)));
            } 
        }

        public NugetPackage(string package, string version)
        {
            Package = package;
            Version = version;
            Select = true;
            Existed = false;
            Error = string.Empty;
            Output = string.Empty;
        }
    }
}
