using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetNugets.ViewModels
{
    public partial class ExistingDownloadsViewModel : ViewModelBase
    {
        private MessengerService messenger;
        private AppStore appStore;

        public ExistingDownloadsViewModel(MessengerService messenger, AppStore appStore)
        {
            this.messenger = messenger;
            this.appStore = appStore;
        }
    }
}
