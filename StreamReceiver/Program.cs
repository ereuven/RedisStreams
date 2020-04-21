using Core;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StreamReceiver
{
    class Program
    {
        static void Main(string[] args)
        {
            var streamName = args[0];
            var consumerGroup = args[1];

            Console.WriteLine(RedisConfig.ConnectionString);
            Console.WriteLine($"Stream name: {streamName}");
            Console.WriteLine($"Consumer group: {consumerGroup}");

            var cts = new CancellationTokenSource();
            Console.WriteLine("Ready to consume, Press any key to stop");
            Consume(RedisConfig.ConnectionString, streamName, consumerGroup, cts.Token);

            Console.ReadKey();
            cts.Cancel();

            Console.WriteLine("Done");
            Console.ReadKey();
        }

        static async Task Consume(string conString, string streamName, string consumerGroup, CancellationToken cancellationToken)
        {
            var redis = ConnectionMultiplexer.Connect(conString);
            try
            {
                redis.GetDatabase().StreamCreateConsumerGroup(streamName, consumerGroup, StreamPosition.Beginning);
            }catch(Exception ex)
            {
                //Console.WriteLine(ex);
            }

            while (!cancellationToken.IsCancellationRequested)
            {
                var msgs = await redis.GetDatabase().StreamReadGroupAsync(streamName, consumerGroup, StreamPosition.NewMessages, count: 4);
                foreach (var msg in msgs)
                {
                    var count = (int)msg.Values[0].Value;
                    await redis.GetDatabase().StreamAcknowledgeAsync(streamName, consumerGroup, msg.Id);
                    Console.WriteLine($"Received counter: {count}");

                }

                await Task.Delay(TimeSpan.FromSeconds(0.5), cancellationToken);
            }
            Console.WriteLine("A");
            redis.Dispose();
        }
    }
}