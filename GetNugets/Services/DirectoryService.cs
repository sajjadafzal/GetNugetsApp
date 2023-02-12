using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetNugets.Services
{
    public static class DirectoryService
    {
        public static string? Browse(string promptMessage)
        {
            string? path = null;
            VistaFolderBrowserDialog FolderDialog = new VistaFolderBrowserDialog();
            FolderDialog.Description = promptMessage;
            FolderDialog.UseDescriptionForTitle = true;
            if (!VistaFolderBrowserDialog.IsVistaFolderDialogSupported)
            {
                //StatusInfo = "Because you are not using Windows Vista or later, the regular folder browser dialog will be used. Please use Windows Vista to see the new dialog.";
            }

            if ((bool)FolderDialog.ShowDialog()!)
            {
                path =  FolderDialog.SelectedPath;
                //StatusInfo = "Selected Folder: " + SolutionFolderPath;
            }

            return path;
        }
    }
}
