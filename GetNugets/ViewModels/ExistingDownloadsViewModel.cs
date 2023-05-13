using CommunityToolkit.Mvvm.ComponentModel;
using GetNugets.Messages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace GetNugets.ViewModels
{
    public partial class ExistingDownloadsViewModel : ViewModelBase
    {
        private MessengerService messenger;
        private AppStore appStore;

        public ObservableCollection<NugetPackageViewModel> ExistingPackages => appStore.ExistingPackages;

        public ExistingDownloadsViewModel(MessengerService messenger, AppStore appStore)
        {
            this.messenger = messenger;
            this.appStore = appStore;
            SubscribeMessenger();
        }

        private void SubscribeMessenger()
        {
            messenger.Subscribe<ExistingPackagesChangedMessage>(this, OnExistingPackagesChanged);
        }

        private void OnExistingPackagesChanged(object recipient, ExistingPackagesChangedMessage message)
        {
            OnPropertyChanged(nameof(ExistingPackages));
        }

        public override void Dispose()
        {
            base.Dispose();
            messenger.UnregisterAll(this);
        }
    }
}
