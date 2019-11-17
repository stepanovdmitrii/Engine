using System;
using System.Threading.Tasks;
using Engine.Core;
using Grpc.Core;
using Grpc.Protos.Events;
using Grpc.Server.Resources;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Grpc.Server.Extensions;
using Grpc.Server.Services.Tasks;

namespace Grpc.Server.Services.Events
{
    internal sealed class EventService: Grpc.Protos.Events.EventService.EventServiceBase
    {
        private readonly ISubscriberService _subscriberService;
        private readonly ILogger<EventService> _logger;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly ITaskService _taskService;

        public EventService(ISubscriberService subscriberService, ILogger<EventService> logger, IStringLocalizer<SharedResource> localizer, ITaskService taskService)
        {
            _subscriberService = subscriberService;
            _logger = logger;
            _localizer = localizer;
            _taskService = taskService;
        }

        public override async Task<SubscribeResponse> Subscribe(SubscribeRequest request, ServerCallContext context)
        {
            Verify.That(false == string.IsNullOrWhiteSpace(request.SubscriberName), _localizer[ResourceKeys.ErrSubscriberNameMustBeSpecified]);
            _logger.LogInformation(_localizer[ResourceKeys.MsgCreatingSubscriber0], request.SubscriberName, context.Host);
            _subscriberService.CreateSubscriber(request.SubscriberName, out Guid subscriberId);
            _logger.LogInformation(_localizer[ResourceKeys.MsgSubscriberCreated0], request.SubscriberName, subscriberId);
            return await Task.FromResult(new SubscribeResponse { SubscriberId = subscriberId.ToString() });
        }

        public override async Task ListCompletedTasks(Subscriber request, IServerStreamWriter<CompletedTask> responseStream, ServerCallContext context)
        {
            Verify.That(false == string.IsNullOrWhiteSpace(request.SubscriberId), _localizer[ResourceKeys.ErrSubscriberIdMustBeSpecified]);
            Verify.That(Guid.TryParse(request.SubscriberId, out Guid subscriberId), _localizer[ResourceKeys.ErrSubscriberNotFound]);

            using (_subscriberService.AcquireTaskAutoRelease(subscriberId))
            {
                while (true)
                {
                    _taskService.WaitNextCompletedTask(subscriberId, context.CancellationToken, out Guid taskId);
                    await responseStream.WriteAsync(new CompletedTask { TaskId = taskId.ToString() });
                }
            }
        }

        public override async Task<CancelTaskResponse> CancelTask(CancelTaskRequest request, ServerCallContext context)
        {
            Verify.That(false == string.IsNullOrWhiteSpace(request.SubscriberId), _localizer[ResourceKeys.ErrSubscriberIdMustBeSpecified]);
            Verify.That(false == string.IsNullOrWhiteSpace(request.TaskId), _localizer[ResourceKeys.ErrTaskIdMustBeSpecified]);
            Verify.That(Guid.TryParse(request.SubscriberId, out Guid subscriberId), _localizer[ResourceKeys.ErrSubscriberNotFound]);
            Verify.That(Guid.TryParse(request.TaskId, out Guid taskId), _localizer[ResourceKeys.ErrSubscriberNotFound]);
            _taskService.CancelTask(subscriberId, taskId);
            return await Task.FromResult(new CancelTaskResponse());
        }

    }
}
