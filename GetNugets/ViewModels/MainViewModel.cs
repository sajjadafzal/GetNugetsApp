using CommunityToolkit.Mvvm.ComponentModel;
using GetNugets.Messages;
using GetNugets.Store;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetNugets.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        private readonly NavigationService navigationService;
        private readonly MessengerService messenger;
        private readonly AppStore appStore;

        [ObservableProperty]
        private string? status;
        public ViewModelBase CurrentViewModel => navigationService.CurrentViewModel;

        public string? NugetsFolder
        {
            get => appStore.NugetsFolder;
            set
            {
                appStore.NugetsFolder = value;
                OnPropertyChanged(nameof(NugetsFolder));
            }
        }

        public MainViewModel(NavigationService navigationService, MessengerService messenger, AppStore appStore)
        {
            this.navigationService = navigationService;
            this.messenger = messenger;
            this.appStore = appStore;
            SubscribeMessenger();
        }

        /// <summary>
        /// Response methon to  <see cref="CurrentViewModelChangedMessage"/> message
        /// </summary>
        /// <param name="recipient"></param>
        /// <param name="message"></param>
        private void OnCurrentViewModelChanged(object recipient, CurrentViewModelChangedMessage message)
        {
            OnPropertyChanged(nameof(CurrentViewModel));
        }

        public override void Dispose()
        {
            base.Dispose();
            messenger.UnregisterAll(this);
        }

        private void SubscribeMessenger()
        {
            messenger.Subscribe<CurrentViewModelChangedMessage>(this, OnCurrentViewModelChanged);
            messenger.Subscribe<StatusMessage>(this, OnStatusMessageReceive);
        }

        private void OnStatusMessageReceive(object recipient, StatusMessage message)
        {
            Status = message.Value;
        }
    }
}
