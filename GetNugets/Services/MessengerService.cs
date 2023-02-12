using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetNugets.Services
{
    public class MessengerService
    {
        StrongReferenceMessenger Messenger;

        public MessengerService()
        {
            Messenger = new StrongReferenceMessenger();
        }

        public void Subscribe<TMessage>(object recipient, MessageHandler<object, TMessage> handler) where TMessage : class
        {
            Messenger.Register<TMessage>(recipient, handler);
        }

        public void Subscribe<TMessage, TToken>(object recipient, TToken token, MessageHandler<object, TMessage> handler)
            where TMessage : class
            where TToken : IEquatable<TToken>
        {
            Messenger.Register<TMessage, TToken>(recipient, token, handler);
        }

        public void Send<TMessage>(TMessage message) where TMessage : class
        {
            Messenger.Send<TMessage>(message);
        }

        public TMessage Send<TMessage, TToken>(TMessage message, TToken token)
            where TMessage : class
            where TToken : IEquatable<TToken>
        {
            return Messenger.Send<TMessage, TToken>(message, token);
        }


        public void Send<TMessage>() where TMessage : class, new()
        {
            Messenger.Send<TMessage>();
        }

        public void UnregisterAll(object recipient)
        {
            Messenger.UnregisterAll(recipient);
        }
    }
}
