using System;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Protos.Events;
using Grpc.Server.Resources;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace Grpc.Server.Services.Events
{
    internal sealed class LongRunningTaskService: Grpc.Protos.Events.LongRunningTaskService.LongRunningTaskServiceBase
    {
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly ILogger<LongRunningTaskService> _logger;

        public LongRunningTaskService(ILogger<LongRunningTaskService> logger, IStringLocalizer<SharedResource> localizer)
        {
            _localizer = localizer;
            _logger = logger;
        }

        public override async Task GetTaskResult(LongTaskResultRequest request, IServerStreamWriter<LongTaskResultResponse> responseStream, ServerCallContext context)
        {
            _logger.LogInformation(_localizer[ResourceKeys.MsgEnumerating]);
            for(int counter = 0; counter < 1_000; ++counter)
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
                context.CancellationToken.ThrowIfCancellationRequested();
                await responseStream.WriteAsync(new LongTaskResultResponse { Result = long.MinValue });
                await responseStream.WriteAsync(new LongTaskResultResponse { Result = long.MaxValue });
                await responseStream.WriteAsync(new LongTaskResultResponse { Result = 0L });
            }
        }
    }
}
