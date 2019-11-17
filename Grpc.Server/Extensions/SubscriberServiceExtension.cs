using System;
using Engine.Core;
using Grpc.Server.Services.Events;

namespace Grpc.Server.Extensions
{
    internal static class SubscriberServiceExtension
    {
        public static IDisposable AcquireTaskAutoRelease(this ISubscriberService service, Guid subscriberId)
        {
            service.Acquire(subscriberId, SubscriptionType.Task);
            return new DisposableAction(() => service.Release(subscriberId, SubscriptionType.Task));
        }
    }
}
