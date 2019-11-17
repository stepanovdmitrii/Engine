using System;
using System.Threading;

namespace Grpc.Server.Services.Tasks
{
    internal interface ITaskService
    {
        void WaitNextCompletedTask(Guid subscriberId, CancellationToken cancellationToken, out Guid taskId);
        void StartNewTask<T>(Guid subscriberId, Func<CancellationToken, T> taskAction, out Guid taskId);
        void CancelTask(Guid subscriberId, Guid taskId);
        T GetTaskResult<T>(Guid subscriberId, Guid taskId);
    }
}
