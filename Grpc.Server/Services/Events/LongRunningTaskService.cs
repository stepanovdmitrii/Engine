using System;
using System.Threading.Tasks;
using Engine.Core;
using Grpc.Core;
using Grpc.Protos.Events;
using Grpc.Server.Resources;
using Grpc.Server.Services.Tasks;
using Microsoft.Extensions.Localization;

namespace Grpc.Server.Services.Events
{
    internal sealed class LongRunningTaskService: Grpc.Protos.Events.LongRunningTaskService.LongRunningTaskServiceBase
    {
        private readonly ITaskService _taskService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public LongRunningTaskService(ITaskService taskService, IStringLocalizer<SharedResource> localizer)
        {
            _taskService = taskService;
            _localizer = localizer;
        }

        public override async Task<CreateTaskResponse> StartTask(CreateTaskRequest request, ServerCallContext context)
        {
            Verify.That(false == string.IsNullOrWhiteSpace(request.SubscriberId), _localizer[ResourceKeys.ErrSubscriberIdMustBeSpecified]);
            Verify.That(Guid.TryParse(request.SubscriberId, out Guid subscriberId), _localizer[ResourceKeys.ErrSubscriberNotFound]);

            _taskService.StartNewTask(subscriberId, token => {
                Task.Delay(TimeSpan.FromMinutes(1), token).Wait();
                return DateTime.UtcNow.Ticks;
            }, out Guid taskId);

            return await Task.FromResult(new CreateTaskResponse { TaskId = taskId.ToString() });
        }

        public override async Task<LongTaskResultResponse> GetTaskResult(LongTaskResultRequest request, ServerCallContext context)
        {
            Verify.That(false == string.IsNullOrWhiteSpace(request.SubscriberId), _localizer[ResourceKeys.ErrSubscriberIdMustBeSpecified]);
            Verify.That(false == string.IsNullOrWhiteSpace(request.TaskId), _localizer[ResourceKeys.ErrTaskIdMustBeSpecified]);
            Verify.That(Guid.TryParse(request.SubscriberId, out Guid subscriberId), _localizer[ResourceKeys.ErrSubscriberNotFound]);
            Verify.That(Guid.TryParse(request.TaskId, out Guid taskId), _localizer[ResourceKeys.ErrSubscriberNotFound]);

            long result = _taskService.GetTaskResult<long>(subscriberId, taskId);

            return await Task.FromResult(new LongTaskResultResponse { Result = result });
        }
    }
}
