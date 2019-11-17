using System;

namespace Grpc.Server.Services.Events
{
    internal interface ISubscriberService
    {
        void CreateSubscriber(string name, out Guid subscriberId);
        void Acquire(Guid subscriberId, SubscriptionType type);
        void Release(Guid subscriberId, SubscriptionType type);
    }
}
