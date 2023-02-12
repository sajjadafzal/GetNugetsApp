using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetNugets.Services
{
    public class UIService
    {
        private StrongReferenceMessenger messenger;

        public UIService()
        {
            messenger = new StrongReferenceMessenger();
        }
        //public void SubscribeToOpenPopupMessage(object recipient, MessageHandler<object, OpenPopupWindowMessage> openPopupMessageHandler)
        //{
        //    messenger.Register<OpenPopupWindowMessage>(recipient, openPopupMessageHandler);
        //}

        //public void SubscribeToClosePopupMessage(object recipient, MessageHandler<object, ClosePopupWindowMessage> closePopupMessageHandler)
        //{
        //    messenger.Register<ClosePopupWindowMessage>(recipient, closePopupMessageHandler);
        //}

        public void Unsubscribe(object recipient)
        {
            messenger.UnregisterAll(recipient);
        }

        //public void OpenPopupWindow(string title, string message)
        //{
        //    MessagePopupViewModel messagePopupViewModel = new(title, message);
        //    messenger.Send(new OpenPopupWindowMessage(messagePopupViewModel));
        //}
        //public void OpenResultDialog(string title, string message, string token, bool yes, bool no, bool ok, bool cancel)
        //{
        //    DialogResultViewModel resultDialogViewModel = new(title, message, token, yes, no, ok, cancel);
        //    messenger.Send(new OpenPopupWindowMessage(resultDialogViewModel));
        //    ;
        //}
        //public void OpenSettings() => messenger.Send(new OpenPopupWindowMessage(DI.Resolve<SettingsPopupViewModel>()));
        //public void OpenAbout() => messenger.Send(new OpenPopupWindowMessage(DI.Resolve<AboutViewModel>()));
        //public void OpenNewProject() => messenger.Send(new OpenPopupWindowMessage(DI.Resolve<NewProjectPopupViewModel>()));
        //public void ClosePopupWindow() => messenger.Send<ClosePopupWindowMessage>();
    }
}
