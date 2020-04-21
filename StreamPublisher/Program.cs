using Core;
using StackExchange.Redis;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace StreamPublisher
{
    class Program
    {
        static void Main(string[] args)
        {
            var streamName = args[0];

            Console.WriteLine(RedisConfig.ConnectionString);
            Console.WriteLine($"Stream name: {streamName}");
            Console.WriteLine("Ready press any key to stop");

            CancellationTokenSource cts = new CancellationTokenSource();
            Generate(RedisConfig.ConnectionString, streamName, cts.Token);

            Console.ReadKey();
            cts.Cancel();
            Console.WriteLine("Done! Press any key to exit");
            Console.ReadKey();

        }

        static async Task Generate(string constring, string streamName, CancellationToken cancellationToken = default(CancellationToken))
        {
            var redis = ConnectionMultiplexer.Connect(constring);

            var counter = 0;
            while (!cancellationToken.IsCancellationRequested)
            {
                Console.WriteLine($"Sending stream - {++counter}");
                await redis.GetDatabase().StreamAddAsync(streamName, new NameValueEntry[] { new NameValueEntry("data", counter) });
                await Task.Delay(TimeSpan.FromSeconds(3), cancellationToken);
            }
            Console.WriteLine("Terminating generator");
            redis.Dispose();
        }
    }
}