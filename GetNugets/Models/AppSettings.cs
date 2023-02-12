using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;

namespace GetNugets.Models
{
    public class AppSettings
    {
        private string? nugetFolder;
        public string? NugetsFolder
        {
            get => nugetFolder;
            set
            {
                nugetFolder = value;
            }
        }


        public AppSettings()
        {

        }
    }
}
