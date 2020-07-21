using System;
using System.Threading;
using Grpc.Net.Client;
using Grpc.Protos.Events;
using System.Threading.Tasks;


namespace Grpc.Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Thread.Sleep(TimeSpan.FromSeconds(5));
            Console.WriteLine("Connecting...");
            using (GrpcChannel channel = GrpcChannel.ForAddress("https://localhost:5001"))
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
            {
                Console.WriteLine("Connected");
                var client = new LongRunningTaskService.LongRunningTaskServiceClient(channel);
                using (var asyncCall = client.GetTaskResult(new LongTaskResultRequest { }, null, null, cancellationTokenSource.Token))
                {
                    while(await asyncCall.ResponseStream.MoveNext(cancellationTokenSource.Token))
                    {
                        var size = asyncCall.ResponseStream.Current.CalculateSize();
                        var result = asyncCall.ResponseStream.Current.Result;
                        Console.WriteLine($"size {size}, result {result}");
                    }
                }
            }

            Console.WriteLine("Finished");
            Console.ReadKey();
        }
    }
}
