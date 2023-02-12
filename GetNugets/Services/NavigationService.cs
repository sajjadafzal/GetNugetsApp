using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetNugets.Services
{
    public class NavigationService
    {
        private ViewModelBase? _CurrentViewModel;
        private MessengerService messenger;

        public NavigationService(MessengerService messenger)
        {
            this.messenger = messenger;
            CurrentViewModel = App.Current.Services.GetService<DownloaderViewModel>()!;
        }

        public ViewModelBase? CurrentViewModel
        {
            get => _CurrentViewModel;
            set
            {
                if (object.Equals(_CurrentViewModel, value) || value == null) return;
                _CurrentViewModel?.Dispose();
                _CurrentViewModel = value;
                messenger.Send(CurrentViewModel);
            }
        }
        

        public void NavigateTo(ViewModelBase viewmodel)
        {
            CurrentViewModel = viewmodel;            
        }

    }
}
